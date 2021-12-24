using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using Smooth;

//smoothnetworktransform a des bugs, ils faut les corrigers pour lancer le client en �diteur windows

public class PlayerLogic : NetworkBehaviour
{
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private Transform selfCamera;
    [SerializeField] GameObject selfFirstPersonView;
    [SerializeField]
    private PlayerMovement selfMovement;
    [SerializeField]
    private AudioSource playerSource;
    [SerializeField]
    private AudioSource playerFootstepSource;
    [SerializeField]
    private Collider playerCollider;
    [SerializeField]
    private GameObject flagRenderer;
    [SerializeField]
    private GameObject selfCollisionParent;
    [SerializeField]
    private SmoothSyncMirror selfSmoothSync;

    private MatchManager matchManager;
    private LevelTransition levelTransition;

    [SerializeField]
    public RectTransform punchChargeDisplay;
    [SerializeField]
    public RectTransform punchChargeSlider1;
    [SerializeField]
    public RectTransform punchChargeSlider2;
    [SerializeField]
    public float punchSliderStartOffset;
    [SerializeField]
    public float punchSliderEndOffset;
    [SerializeField]
    public Image punchCooldownDisplay;
    [SerializeField]
    private Text hudTextPlayer;
    [SerializeField]
    private Text scoreTextBlue;
    [SerializeField]
    private Text scoreTextRed;
    [SerializeField]
    private Text nextRotationTimeText;
    [SerializeField]
    public Image teamColorIndicator;
    [SerializeField]
    public GameObject punchHitUi;
    [SerializeField]
    private GameObject punchGetHitUi;
    [SerializeField]
    private GameObject punchLoadingEffect;
    [SerializeField]
    private GameObject punchHitEffect;

    [SerializeField]
    private GameObject FlagObject;
    [SerializeField]
    private GameObject FlagInGame;

    [SerializeField]
    private Material redTeamMaterial;
    [SerializeField]
    private Material blueTeamMaterial;

    [SyncVar]
    public LobbyPlayerLogic.TeamName teamName;
    [SyncVar]
    public int spawnPosition;

    private float yRotation, xRotation;

    private float timeStampRunAccel, timeStampRunDecel;
    private float timeAttack;
    [HideInInspector]
    public float ratioAttack;

    [HideInInspector]
    public Vector3 normalWallJump;

    private bool footStepFlag;
    private bool touchingGroundFlag;
    private bool hasStartedCharge;

    //State
    [HideInInspector]
    public bool isGrounded, isJumping, isAttachToWall, isTouchingTheGround, isTouchingWall, isInControl;

    [SyncVar]
    public bool hasFlag;

    //timer to start a round
    private double timerToStart;

    [SerializeField]
    private double timerMaxToStart = 3d;

    //Move if the round is start
    public bool roundStarted = false;

    //Joystick
    float attackTriggerValueDelta = 0f;
    float jumpTriggerValueDelta = 0f;

    void Start()
    {
        Debug.Log("Start");
        matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>(); //Ne pas bouger
        levelTransition = GameObject.Find("GameManager").GetComponent<LevelTransition>();
        if (FlagObject != null)
        {
            FlagObject = GameObject.Find("Flag");
        }
        isInControl = true;
        selfCamera.gameObject.SetActive(false);
        if (hasAuthority)
        {
            selfCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

            if (teamName == LobbyPlayerLogic.TeamName.Blue)
            {
                teamColorIndicator.color = Color.blue;
            }
            else
            {
                teamColorIndicator.color = Color.red;
            }
        }
        else
        {
            selfFirstPersonView.SetActive(false);
        }

        if (teamName == LobbyPlayerLogic.TeamName.Blue)
        {
            playerCollider.transform.GetComponent<MeshRenderer>().material = blueTeamMaterial;
        }
        else
        {
            playerCollider.transform.GetComponent<MeshRenderer>().material = redTeamMaterial;
        }

    }

    void Update()
    {
        UpdateNextTransitionTime();

        if (hasAuthority && roundStarted)
        {
            fpsView();
            VerticalMovement();
            HorizontalMovement();

            //Respawn player
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdForceRespawn(3f);
            }
        }
        ShowFlagToAllPlayer();
    }

    #region Movement Logic

    private void fpsView()
    {
        float mouseX;
        float mouseY;
        if (Input.GetJoystickNames().Length > 0)
		{
            mouseX = Input.GetAxis("RHorizontal") * Time.deltaTime * selfParams.aimJoystickSensitivity;
            mouseY = Input.GetAxis("RVertical") * Time.deltaTime * selfParams.aimJoystickSensitivity;
		}
        else
		{
            mouseX = Input.GetAxis("Mouse X") * selfParams.mouseSensivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * selfParams.mouseSensivity * Time.deltaTime;
        }

#if UNITY_EDITOR

        if (Input.GetJoystickNames().Length <= 0)
		{
            mouseX = Input.GetAxis("Mouse X") * selfParams.mouseSensivity * 4f * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * selfParams.mouseSensivity * 4f * Time.deltaTime;
        }
        #endif


        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);

        selfCamera.Rotate(Vector3.up * mouseX);
        selfCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        selfCollisionParent.transform.localRotation = Quaternion.Euler(0, selfCamera.rotation.eulerAngles.y, 0);

    }

    private void HorizontalMovement()
    {
        isGrounded = isTouchingTheGround && !selfMovement.isAttacking;

        if(isGrounded && Time.time - timeStampRunAccel > 0.2f)
        {
            if(footStepFlag)
            {
                SoundManager.Instance.PlaySoundEvent("PlayerFootstep", playerFootstepSource);
                footStepFlag = false;
            }
        }
        else
        {
            if(!footStepFlag)
            {
                playerFootstepSource.Stop();
                footStepFlag = true;
            }
        }

        if(isTouchingTheGround)
        {
            if(touchingGroundFlag)
            {
                SoundManager.Instance.PlaySoundEvent("PlayerJumpOff", playerSource);
                touchingGroundFlag = false;
                footStepFlag = false;
            }
        }
        else
        {
            touchingGroundFlag = true;
        }

        if (!selfMovement.isClimbingMovement && !isAttachToWall && !selfMovement.isAttacking && isInControl)
        {
            Vector3 keyDirection = Vector3.zero;

            if (Input.GetJoystickNames().Length > 0)
			{
                float _horizontal = Input.GetAxis("Horizontal");
                float _vertical = Input.GetAxis("Vertical");
                if(_vertical > 0)
				{
                    keyDirection += GetHorizontalVector(selfCollisionParent.transform.forward);//a changer ici
                    selfMovement.CanClimb();
                }
                else if(_vertical < 0)
				{
                    keyDirection += GetHorizontalVector(-selfCollisionParent.transform.forward);
                }
                if(_horizontal > 0)
				{
                    keyDirection += GetHorizontalVector(selfCollisionParent.transform.right);
                }
                else if(_horizontal < 0)
				{
                    keyDirection += GetHorizontalVector(-selfCollisionParent.transform.right);
                }

                keyDirection.Normalize();

                if(keyDirection != Vector3.zero)
				{
                    if (isGrounded)
                    {
                        selfMovement.Move(keyDirection, Time.time - timeStampRunAccel);
                    }
                    else
                    {
                        selfMovement.AirMove(keyDirection);
                    }
                }
                else
				{
                    if (isGrounded)
                    {
                        selfMovement.Decelerate(Time.time - timeStampRunDecel);
                        timeStampRunAccel = Time.time;
                    }
                    else
                    {
                        timeStampRunDecel = Time.time;
                    }
                }
            }
            else if (Input.GetKey(selfParams.left) || Input.GetKey(selfParams.right) || Input.GetKey(selfParams.front) || Input.GetKey(selfParams.back))
            {
                timeStampRunDecel = Time.time;

                if (Input.GetKey(selfParams.front))
                {
                    keyDirection += GetHorizontalVector(selfCollisionParent.transform.forward);//a changer ici
                    selfMovement.CanClimb();
                }

                if (Input.GetKey(selfParams.back))
                {
                    keyDirection += GetHorizontalVector(-selfCollisionParent.transform.forward);
                }

                if (Input.GetKey(selfParams.left))
                {
                    keyDirection += GetHorizontalVector(-selfCollisionParent.transform.right);
                }

                if (Input.GetKey(selfParams.right))
                {
                    keyDirection += GetHorizontalVector(selfCollisionParent.transform.right);
                }
                keyDirection.Normalize();

                if (isGrounded)
                {
                    selfMovement.Move(keyDirection, Time.time - timeStampRunAccel);
                }
                else
                {
                    selfMovement.AirMove(keyDirection);
                }

            }
            else //Decelerate hspd
            {
                if (isGrounded)
                {
                    selfMovement.Decelerate(Time.time - timeStampRunDecel);
                    timeStampRunAccel = Time.time;
                }
                else
                {
                    timeStampRunDecel = Time.time;
                }
            }

        }

        //Attack Logic
        if (!selfMovement.isClimbingMovement && !selfMovement.isAttacking && selfMovement.isAttackReset && !selfMovement.isAttackInCooldown)
        {
            AttackInput();
        }
    }


    public Vector3 GetHorizontalVector(Vector3 originVector)
    {
        Vector3 horizontalVector = new Vector3(originVector.x, 0, originVector.z);
        return horizontalVector.normalized;
    }

    private void VerticalMovement()
    {
        if(Input.GetJoystickNames().Length > 0)
		{
            VerticalMovementFromJoystick();
            return;
		}

        if (!selfMovement.isClimbingMovement && !selfMovement.isAttacking)
        {
            if (!isGrounded)
            {
                if (selfMovement.IsSomethingCollide())
                {
                    isTouchingWall = true;
                    if (Input.GetKey(selfParams.jump))
                    {
                        isAttachToWall = true;
                        selfMovement.ApplyWallSlideForces();
                    }
                    else if (Input.GetKeyUp(selfParams.jump))
                    {
                        if (GetNearbyWallNormal() != Vector3.zero)
                        {
                            selfMovement.WallJump(GetNearbyWallNormal());
                        }
                    }
                    else
                    {
                        isAttachToWall = false;
                        selfMovement.ApplyGravity();
                    }
                }
                else
                {
                    isAttachToWall = false;
                    selfMovement.ApplyGravity();
                    isTouchingWall = false;
                }

            }
            else
            {
                selfMovement.ApplyGravity();
                if (Input.GetKeyDown(selfParams.jump) && !isJumping && !isAttachToWall)
                {
                    SoundManager.Instance.PlaySoundEvent("PlayerJump", playerSource);
                    selfMovement.Jump();
                }

                if (isGrounded && !selfMovement.isAttacking)
                {
                    selfMovement.isAttackReset = true;
                }
                isAttachToWall = false;
            }
        }

    }

    void VerticalMovementFromJoystick()
	{
        if (!selfMovement.isClimbingMovement && !selfMovement.isAttacking)
        {
            if (!isGrounded)
            {
                if (selfMovement.IsSomethingCollide())
                {
                    isTouchingWall = true;
                    if (Input.GetAxis("LT") > 0f)
                    {
                        isAttachToWall = true;
                        selfMovement.ApplyWallSlideForces();
                    }
                    else if (Input.GetAxis("LT") == 0f && jumpTriggerValueDelta != 0f)
                    {
                        if (GetNearbyWallNormal() != Vector3.zero)
                        {
                            selfMovement.WallJump(GetNearbyWallNormal());
                        }
                    }
                    else
                    {
                        isAttachToWall = false;
                        selfMovement.ApplyGravity();
                    }
                }
                else
                {
                    isAttachToWall = false;
                    selfMovement.ApplyGravity();
                    isTouchingWall = false;
                }

            }
            else
            {
                selfMovement.ApplyGravity();
                if (Input.GetAxis("LT") > 0f && jumpTriggerValueDelta == 0f && !isJumping && !isAttachToWall)
                {
                    SoundManager.Instance.PlaySoundEvent("PlayerJump", playerSource);
                    selfMovement.Jump();
                }

                if (isGrounded && !selfMovement.isAttacking)
                {
                    selfMovement.isAttackReset = true;
                }
                isAttachToWall = false;
            }
        }

        jumpTriggerValueDelta = Input.GetAxis("LT");
    }

    #endregion

    #region Collision
    public void OnCollisionEnter(Collision info)
    {
        Vector3 point = info.contacts[0].point;
        normalWallJump = info.contacts[0].normal;

        //Debug.DrawRay(point, (normalWallJump + new Vector3(0, 0.5f, 0)) * 10, Color.red, 2.5f);
    }

    public Vector3 GetNearbyWallNormal()
    {
        Vector3 wallNormal = Vector3.zero;

        Collider[] nearbyWalls = Physics.OverlapBox(transform.position, new Vector3(0.7f, 0.2f, 0.7f), Quaternion.identity, LayerMask.GetMask("Outlined"));
        if(nearbyWalls.Length > 0)
        {
            RaycastHit wallHit;
            Vector3 wallPosDir = nearbyWalls[0].transform.position - transform.position;
            wallPosDir = new Vector3(wallPosDir.x, 0, wallPosDir.z);
            wallPosDir.Normalize();
            Physics.Raycast(transform.position, wallPosDir, out wallHit, 20, LayerMask.GetMask("Outlined"));

            if (wallHit.collider != null)
            {
                wallNormal = wallHit.normal;
            }
        }

        return wallNormal;
    }

    #endregion

    #region AttackLogic

    public void AttackInput()
    {
        if(Input.GetJoystickNames().Length > 0)
		{
            AttackInputFromJoystick();
            return;
		}
        if (Input.GetMouseButtonDown(selfParams.attackMouseInput) && !selfMovement.isAttacking && !selfMovement.isAttackInCooldown)
        {
            SoundManager.Instance.PlaySoundEvent("PlayerPunchCharge", playerSource);
            hasStartedCharge = true;
        }
        //Attack load
        if (Input.GetMouseButton(selfParams.attackMouseInput) && hasStartedCharge)
        {
            CmdShowLoadingPunchStart();
            timeAttack += Time.deltaTime;
            if(timeAttack > 0.2f)
            {
                punchChargeDisplay.gameObject.SetActive(true);
            }
            ratioAttack = selfMovement.AttackLoad(timeAttack);
            punchChargeSlider1.anchoredPosition = Vector2.Lerp(new Vector2(-punchSliderStartOffset, 0), new Vector2(-punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);
            punchChargeSlider2.anchoredPosition = Vector2.Lerp(new Vector2(punchSliderStartOffset, 0), new Vector2(punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);

        }
        //Attack lauch
        if (Input.GetMouseButtonUp(selfParams.attackMouseInput) && hasStartedCharge)
        {
            CmdShowLoadingPunchEnd();
            hasStartedCharge = false;
            punchChargeDisplay.gameObject.SetActive(false);
            SoundManager.Instance.PlaySoundEvent("PlayerPunch", playerSource);
            SoundManager.Instance.StopSoundWithDelay(playerSource, 0.2f);
            selfMovement.Attack(ratioAttack);
            timeAttack = 0;
            ratioAttack = 0;
        }
    }

    void AttackInputFromJoystick()
	{
        if (Input.GetAxis("RT") > 0 && attackTriggerValueDelta == 0f && !selfMovement.isAttacking && !selfMovement.isAttackInCooldown)
        {
            SoundManager.Instance.PlaySoundEvent("PlayerPunchCharge", playerSource);
            hasStartedCharge = true;
        }
        //Attack load
        if (Input.GetAxis("RT") > 0f && hasStartedCharge)
        {
            CmdShowLoadingPunchStart();
            timeAttack += Time.deltaTime;
            if (timeAttack > 0.2f)
            {
                punchChargeDisplay.gameObject.SetActive(true);
            }
            ratioAttack = selfMovement.AttackLoad(timeAttack);
            punchChargeSlider1.anchoredPosition = Vector2.Lerp(new Vector2(-punchSliderStartOffset, 0), new Vector2(-punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);
            punchChargeSlider2.anchoredPosition = Vector2.Lerp(new Vector2(punchSliderStartOffset, 0), new Vector2(punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);

        }
        //Attack lauch
        if (Input.GetAxis("RT") == 0 && attackTriggerValueDelta != 0f && hasStartedCharge)
        {
            CmdShowLoadingPunchEnd();
            hasStartedCharge = false;
            punchChargeDisplay.gameObject.SetActive(false);
            SoundManager.Instance.PlaySoundEvent("PlayerPunch", playerSource);
            SoundManager.Instance.StopSoundWithDelay(playerSource, 0.2f);
            selfMovement.Attack(ratioAttack);
            timeAttack = 0;
            ratioAttack = 0;
        }

        //Refresh attack input value for next frame
        attackTriggerValueDelta = Input.GetAxis("RT");
    }

    // faire fonction de d�lai d'affichage de la charge du punch

    public void UpdatePunchCooldown(float cdTime)
    {
        punchCooldownDisplay.fillAmount = cdTime / selfParams.punchCooldown;
    }

    #endregion


    #region Network logic
    [Command]
    public void CmdSwitchCollider(bool isTrigger)
    {
        playerCollider.isTrigger = isTrigger;
        RpcSwitchCollider(isTrigger);
    }

    [ClientRpc]
    public void RpcSwitchCollider(bool isTrigger)
    {
        playerCollider.isTrigger = isTrigger;
    }

    [Command]
    public void CmdForceRespawn(float maxTimer)
    {
        NetworkConnection conn = null;
        RpcRespawn(conn, maxTimer);
    }

    [TargetRpc]
    public void RpcRespawn(NetworkConnection conn, float maxTimer)
    {
        roundStarted = false;
        timerToStart = NetworkTime.time;

        if (hasFlag)
        {
            SoundManager.Instance.PlaySoundEvent("LevelOverdriveDroped");
            CmdDropFlag();
            CmdShowFlagInGame();
        }

        StartCoroutine(RespawnManager());
        
    }


    public IEnumerator RespawnManager()
    {
        //Find respawn and set spawn
        if (hasAuthority)
        {
            Transform spawnPoint;
            spawnPoint = GameObject.FindWithTag("Spawner").transform.GetChild(spawnPosition);



            transform.position = spawnPoint.position; //Obligatoire, sinon ne trouve pas le spawner à la premirèe frame
            selfCollisionParent.transform.localRotation = spawnPoint.rotation;
            selfCamera.localRotation = spawnPoint.rotation;

            //Debug.Log(spawnPoint.position);
            //Debug.Log("Spawn");

            //Tp player to the spwan point
            selfSmoothSync.teleportOwnedObjectFromOwner();

            //Create timer before restart player
            hudTextPlayer.gameObject.SetActive(true);
            while (NetworkTime.time - timerToStart <= timerMaxToStart)
            {
                selfMovement.ResetVelocity();
                selfMovement.ResetVerticalVelocity();
                if (System.Math.Round(NetworkTime.time - timerToStart).ToString() != hudTextPlayer.text)
                    hudTextPlayer.text = System.Math.Round(NetworkTime.time - timerToStart).ToString();
                yield return new WaitForEndOfFrame();

            }
            roundStarted = true;
            hudTextPlayer.gameObject.SetActive(false);
        }
    }

    [TargetRpc]
    public void RpcShowGoal(NetworkConnection conn,string text)
    {
        timerToStart = NetworkTime.time;
        StartCoroutine(GoalMessageManager(text));
        CmdShowScoreHud();
    }

    public IEnumerator GoalMessageManager(string text)
    {
        hudTextPlayer.gameObject.SetActive(true);
        while (NetworkTime.time - timerToStart <= timerMaxToStart)
        {
            if (text != hudTextPlayer.text)
                hudTextPlayer.text = text;
            yield return new WaitForEndOfFrame();

        }
        selfMovement.ResetVelocity();
        roundStarted = false;
        timerToStart = NetworkTime.time;
        StartCoroutine(RespawnManager());
        FlagObject.SetActive(true);

    }

    [TargetRpc]
    public void RpcEndGame(NetworkConnection conn,string text)
    {
        timerToStart = NetworkTime.time;
        StartCoroutine(EndGameManager(text));
    }

    public IEnumerator EndGameManager(string text)
    {
        CmdShowScoreHud();
        hudTextPlayer.gameObject.SetActive(true);
        while (NetworkTime.time - timerToStart <= timerMaxToStart)
        {
            if (text != hudTextPlayer.text)
                hudTextPlayer.text = text;
            yield return new WaitForEndOfFrame();

        }

        if (isServer)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        MyNewNetworkManager.singleton.ServerChangeScene("LobbyScene"); // Need to be rework
    }

    //Punch and getPunch Logic

    [Command]
    public void CmdAttackCollider(bool isActive)
    {
        selfMovement.selfAttackCollider.SetActive(isActive);
        RpcAttackCollider(isActive);
    }

    [ClientRpc]
    public void RpcAttackCollider(bool isActive)
    {
        selfMovement.selfAttackCollider.SetActive(isActive);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetFlag()
    {
        hasFlag = true;
        SoundManager.Instance.PlaySoundEvent("LevelOverdriveTaken");
    }

    [Command(requiresAuthority = false)]
    public void CmdDropFlag()
    {
        hasFlag = false;        
    }

    [Command(requiresAuthority = false)]
    public void CmdGetPunch(NetworkIdentity netid,Vector3 directedForce)
    {
        if (hasFlag)
        {
            hasFlag = false;
        }

        if(netid.connectionToClient != null)
        {
            RpcGetPunch(netid.connectionToClient, directedForce);
        }
        else
        {
            Vector3 correctingDirectedForce;
            if(directedForce.y >= 0)
            {

            }
            else
            {

            }
            Debug.Log(directedForce);
            GetPunch(directedForce); //For debugging
        }
        
    }

    [TargetRpc]
    public void RpcGetPunch(NetworkConnection conn,Vector3 directedForce)
    {
        StartCoroutine(GetHitUi(1f));
        StartCoroutine(NoControl(selfParams.punchedNoControlTime));
        selfMovement.Propulse(directedForce);
    }

    public void GetPunch(Vector3 directedForce)
    {
        StartCoroutine(NoControl(selfParams.punchedNoControlTime));
        selfMovement.Propulse(directedForce);
    }

    public IEnumerator NoControl(float time)
    {
        isInControl = false;
        yield return new WaitForSeconds(time);
        isInControl = true;
    }

    [Command]
    public void CmdCreateParticulePunch(Vector3 position)
    {
        StartCoroutine(ParticulePunchManage(position));
    }

    public IEnumerator ParticulePunchManage(Vector3 position)
    {
        GameObject objEffect = Instantiate(punchHitEffect, position,Quaternion.identity);
        NetworkServer.Spawn(objEffect);
        yield return new WaitForSeconds(4);
        NetworkServer.Destroy(objEffect);
    }


    //Loading network particule loading 

    [Command]
    public void CmdShowLoadingPunchStart()
    {
        RpcShowLoadingPunchStart();
    }

    [ClientRpc]
    public void RpcShowLoadingPunchStart()
    {
        if (!hasAuthority)
        {
            punchLoadingEffect.SetActive(true);
        }
        
    }

    [Command]
    public void CmdShowLoadingPunchEnd()
    {
        RpcShowLoadingPunchEnd();
    }

    [ClientRpc]
    public void RpcShowLoadingPunchEnd()
    {
        if (!hasAuthority)
        {
            punchLoadingEffect.SetActive(false);
        }
        
    }


    #endregion

    #region Ui

    //Manage ui hit getHit
    public void StartHitUi(float timelife)
    {
        StartCoroutine(HitUi(timelife));
    }

    private IEnumerator HitUi(float timelife)
    {
        if (hasAuthority)
        {
            punchHitUi.SetActive(true);
            float time = 0;
            while (time < timelife)
            {
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            punchHitUi.SetActive(false);
        }
        yield return null;
    }

    public IEnumerator GetHitUi(float timelife)
    {
        punchGetHitUi.SetActive(true);
        Color temp = punchGetHitUi.GetComponent<Image>().color;
        temp.a = 1;
        float time = 0;
        while (time < timelife)
        {
            Debug.Log(time);
            time += Time.deltaTime;
            temp.a = 1 - time;
            punchGetHitUi.GetComponent<Image>().color = temp;
            yield return new WaitForEndOfFrame();
        }
        punchGetHitUi.SetActive(false);
        yield return null;
    }

    #endregion

    #region Flag Logic

    private void ShowFlagToAllPlayer()
    {
        if (hasFlag)
        {
            if (hasAuthority)
            {
                flagRenderer.SetActive(true);
            }
            FlagInGame.SetActive(true);
        }
        else
        {
            flagRenderer.SetActive(false);
            FlagInGame.SetActive(false);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdHideFlagInGame()
    {
        RpcHideFlagInGame();
    }
    [ClientRpc]
    public void RpcHideFlagInGame()
    {
        FlagObject.SetActive(false);
    }

    [Command]
    private void CmdShowFlagInGame()
    {
        RpcShowFlagInGame();
    }

    [ClientRpc]
    private void RpcShowFlagInGame()
    {
        FlagObject.SetActive(true);
    }

    #endregion

    #region matchLogic
    [Command]
    private void CmdShowScoreHud()
    {
        RpcShowScoreHud();
    }

    [ClientRpc]
    public void RpcShowScoreHud()
    {
        scoreTextRed.text = matchManager.redScore.ToString();
        scoreTextBlue.text = matchManager.blueScore.ToString();
    }

    private void UpdateNextTransitionTime()
    {
        float timerBeforeNextTransition = Mathf.Ceil((float)(levelTransition.timerChange - (NetworkTime.time - levelTransition.networkTime)));
        nextRotationTimeText.text = timerBeforeNextTransition.ToString();
    }

    #endregion
}



