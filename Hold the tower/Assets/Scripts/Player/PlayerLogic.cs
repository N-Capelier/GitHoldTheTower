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

    private List<GameObject> noAuthorityPlayer;
    public GameObject authorityPlayer;

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
    [SerializeField] SkinnedMeshRenderer playerMeshRenderer;
    [SerializeField] GameObject playerRenderObject;
    [SerializeField]
    private GameObject flagRenderer;
    [SerializeField]
    private GameObject selfCollisionParent;
    [SerializeField]
    private SmoothSyncMirror selfSmoothSync;
    [SerializeField]
    private PlayerMenu selfMenu;

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
    public Image punchCooldownSecondDisplay;
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
    public GameObject punchChargeDistancePreview;
    [SerializeField]
    public GameObject punchChargeDistancePreview2;
    [SerializeField]
    public GameObject punchChargeSliderLine;

    [SerializeField]
    private GameObject FlagObject;
    [SerializeField]
    private GameObject FlagInGame;

    [SerializeField]
    private Material redTeamMaterial;
    [SerializeField]
    private Material blueTeamMaterial;

    [SerializeField]
    private Image loadingScreen;
    [SerializeField]
    private GameObject player3dPseudo;
    [SerializeField]
    public GameObject hud;
    [SerializeField]
    public GameObject playerModel;

    [SyncVar]
    public LobbyPlayerLogic.TeamName teamName;
    [SyncVar]
    public int spawnPosition;
    [SyncVar(hook = nameof(SetPseudo))]
    public string pseudoPlayer;

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
    public bool isGrounded, isJumping, isAttachToWall, isTouchingTheGround, isTouchingWall, isInControl, isSpawning;

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

    //charge preview
    Vector3 chargePreviewStartPos;

    void Start()
    {
        Debug.Log("Start");
        matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>(); //Ne pas bouger
        levelTransition = GameObject.Find("GameManager").GetComponent<LevelTransition>();

        chargePreviewStartPos = punchChargeDistancePreview.transform.localPosition;

        //Analytics
        if (GameObject.Find("Analytics") != null)
        {
            GameObject.Find("Analytics").GetComponent<PA_Position>().analyticGameObjectPosition.Add(this.transform);
        }
        //

        if (FlagObject != null)
        {
            FlagObject = GameObject.Find("Flag");
        }


        isInControl = true;
        selfCamera.gameObject.SetActive(false);
        hud.SetActive(false);
        if (hasAuthority)
        {
            selfCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

            loadingScreen.gameObject.SetActive(true);
            hud.SetActive(true);

            if (teamName == LobbyPlayerLogic.TeamName.Blue)
            {
                teamColorIndicator.color = Color.blue;
                
            }
            else
            {
                teamColorIndicator.color = Color.red;
                matchManager.GetComponent<LandMarkManager>().SwapColor();
            }
            //Own player is blue
            playerMeshRenderer.material = blueTeamMaterial;
            playerRenderObject.SetActive(false);
        }
        else
        {
            selfFirstPersonView.SetActive(false);

            //Make blue if ally, else make red him red
            if(GameObject.Find("ServerManager").GetComponent<MyNewNetworkManager>().playerTeamName == teamName)
            {
                playerMeshRenderer.material = blueTeamMaterial;
            }
            else
            {
                playerMeshRenderer.material = redTeamMaterial;
            }

        }

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        noAuthorityPlayer = new List<GameObject>();
        foreach (GameObject objPlayer in allPlayers)
        {
            if (objPlayer.GetComponent<PlayerLogic>() != null)
            {
                if (objPlayer.GetComponent<NetworkIdentity>().hasAuthority)
                {
                    authorityPlayer = objPlayer;
                    Debug.Log(authorityPlayer);
                }
                else
                {

                    noAuthorityPlayer.Add(objPlayer);
                }
            }

        }

    }

    void Update()
    {
        //Stop Movement if in the menu
        if (!selfMenu.menuIsOpen && !isSpawning)
        {
            fpsView();

        }

        UpdateNextTransitionTime();

        if (hasAuthority && roundStarted)
        {
            //Stop Movement if in the menu
          
            VerticalMovement();
            HorizontalMovement();
            CmdRotateModel(selfCamera.rotation.eulerAngles.y);

            //Respawn player
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdForceRespawn(3f);
            }
        }

        if(!hasAuthority)
        {
            player3dPseudo.transform.rotation = Quaternion.LookRotation(transform.position - authorityPlayer.transform.position);
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
            mouseX = Input.GetAxis("Mouse X") * selfParams.mouseSensivity * Time.deltaTime * 2f;
            mouseY = Input.GetAxis("Mouse Y") * selfParams.mouseSensivity * Time.deltaTime * 2f;
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
                CmdPlayerFootstepSource("PlayerFootstep");
                footStepFlag = false;
            }
        }
        else
        {
            if(!footStepFlag)
            {
                CmdStopPlayerFootstepSource();
                footStepFlag = true;
            }
        }

        if(isTouchingTheGround)
        {
            if(touchingGroundFlag)
            {
                CmdPlayerSource("PlayerJumpOff");
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
        if (!selfMovement.isClimbingMovement && !selfMovement.isAttacking && selfMovement.isAttackReset && !selfMovement.isAttackInCooldown && !selfMenu.menuIsOpen)
        {
            AttackInput();
        }
    }

    public Vector3 GetHorizontalVector(Vector3 originVector)
    {
        Vector3 horizontalVector = new Vector3(originVector.x, 0, originVector.z);
        return horizontalVector.normalized;
    }

    private bool isWallSliding;
    private void VerticalMovement()
    {
        if (Input.GetJoystickNames().Length > 0)
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
                        Vector3 hSpeed = new Vector3(selfMovement.selfRbd.velocity.x, 0, selfMovement.selfRbd.velocity.z);
                        if (!isAttachToWall)
                        {
                            isAttachToWall = true;
                            if (!IsLookingInWall() && hSpeed.magnitude > selfParams.minHorizontalSpeedToStartWallRide)
                            {
                                if (selfMovement.SetWallSlideDirection())
                                {
                                    isWallSliding = true;
                                }
                            }
                        }


                        if (isWallSliding && hSpeed.magnitude > selfParams.minHorizontalSpeedToStartWallRide)
                        {
                            selfMovement.ApplyWallSlideForces();
                        }
                        else
                        {
                            selfMovement.ApplyWallAttachForces();
                            isWallSliding = false;
                        }
                    }
                    else if (Input.GetKeyUp(selfParams.jump) && isWallSliding)
                    {
                        if (GetNearbyWallNormal() != Vector3.zero)
                        {
                            selfMovement.WallJump(GetNearbyWallNormal());
                            isAttachToWall = false;
                            isWallSliding = false;
                        }
                    }
                    else
                    {
                        isAttachToWall = false;
                        isWallSliding = false;
                        selfMovement.ApplyGravity();
                    }
                }
                else
                {
                    isAttachToWall = false;
                    selfMovement.ApplyGravity();
                    isTouchingWall = false;
                    isWallSliding = false;
                }

            }
            else
            {
                selfMovement.ApplyGravity();
                if (Input.GetKeyDown(selfParams.jump) && !isJumping && !isAttachToWall)
                {
                    CmdPlayerSource("PlayerJump");
                    selfMovement.Jump();
                }

                if (isGrounded && !selfMovement.isAttacking)
                {
                    selfMovement.isAttackReset = true;
                }
                isAttachToWall = false;
                isWallSliding = false;
                isTouchingWall = false;
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
                    //SoundManager.Instance.PlaySoundEvent("PlayerJump", playerSource);
                    CmdPlayerSource("PlayerJump");
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

        Collider[] nearbyWalls = Physics.OverlapBox(transform.position, new Vector3(1f, 0.2f, 1f), Quaternion.identity, LayerMask.GetMask("Outlined"));
        if (nearbyWalls.Length > 0)
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

    public bool IsLookingInWall()
    {
        Vector3 wallNormal = GetNearbyWallNormal();
        if (wallNormal != Vector3.zero)
        {
            float wallAngle = Vector3.SignedAngle(Vector3.right, wallNormal, Vector3.up);
            float lookAngle = Vector3.SignedAngle(Vector3.right, GetHorizontalVector(selfCamera.forward).normalized, Vector3.up);

            float angleDist = lookAngle - wallAngle;
            angleDist = selfMovement.GetClampedAngle(angleDist);

            if (Mathf.Abs(angleDist) > selfParams.wallJumpMinAngleToCancelDeviation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
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
            //SoundManager.Instance.PlaySoundEvent("PlayerPunchCharge", playerSource);
            CmdPlayerSource("PlayerPunchCharge");
            hasStartedCharge = true;
        }
        //Attack load
        if (Input.GetMouseButton(selfParams.attackMouseInput) && hasStartedCharge)
        {
            CmdShowLoadingPunchStart();
            timeAttack += Time.deltaTime;

            punchChargeDistancePreview.SetActive(true);
            punchChargeDistancePreview2.SetActive(true);
            punchChargeSliderLine.SetActive(true);
            if (timeAttack > 0.2f && !hasFlag)
            {
                punchChargeDisplay.gameObject.SetActive(true);
            }

            if(hasFlag)
            {
                ratioAttack = 0;
                punchChargeSlider1.anchoredPosition = Vector2.Lerp(new Vector2(-punchSliderStartOffset, 0), new Vector2(-punchSliderEndOffset, 0), 0);
                punchChargeSlider2.anchoredPosition = Vector2.Lerp(new Vector2(punchSliderStartOffset, 0), new Vector2(punchSliderEndOffset, 0), 0);

                punchChargeDistancePreview.transform.localPosition = chargePreviewStartPos + Vector3.forward * 0.0046923076923077f * selfParams.punchBaseSpeed * selfParams.punchSpeedByCharge.Evaluate(0) / selfParams.punchSpeedByCharge.Evaluate(1);
                punchChargeDistancePreview2.transform.localPosition = punchChargeDistancePreview.transform.localPosition;

                punchChargeSliderLine.transform.localPosition = (punchChargeDistancePreview.transform.localPosition + chargePreviewStartPos) / 2;
                punchChargeSliderLine.transform.localScale = new Vector3(punchChargeSliderLine.transform.localScale.x, punchChargeSliderLine.transform.localScale.y, punchChargeDistancePreview.transform.localPosition.z);

            }
            else
            {
                ratioAttack = selfMovement.AttackLoad(timeAttack);
                punchChargeSlider1.anchoredPosition = Vector2.Lerp(new Vector2(-punchSliderStartOffset, 0), new Vector2(-punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);
                punchChargeSlider2.anchoredPosition = Vector2.Lerp(new Vector2(punchSliderStartOffset, 0), new Vector2(punchSliderEndOffset, 0), timeAttack / selfParams.punchMaxChargeTime);

                punchChargeDistancePreview.transform.rotation = Quaternion.Euler(0, selfCamera.rotation.eulerAngles.y, 0);
                punchChargeDistancePreview.transform.rotation = Quaternion.Euler(0, 0, 0);
                punchChargeDistancePreview2.transform.rotation = Quaternion.Inverse(selfCamera.rotation);
                punchChargeDistancePreview.transform.localPosition = chargePreviewStartPos + Vector3.forward * 0.0046923076923077f * selfParams.punchBaseSpeed * selfParams.punchSpeedByCharge.Evaluate(ratioAttack) / selfParams.punchSpeedByCharge.Evaluate(1); // alors ce chiffre bizarre je l'ai calculer rapport à la courbe de velocité, c'est le coefficient de la distance par rapport à la vitesse du punch
                punchChargeDistancePreview2.transform.localPosition = punchChargeDistancePreview.transform.localPosition;
                punchChargeSliderLine.transform.localPosition = (punchChargeDistancePreview.transform.localPosition + chargePreviewStartPos) / 2;
                if(selfMovement.isPunchInstantDestroy)
                {
                    punchChargeSliderLine.transform.localScale = new Vector3(0.3f, 0.3f, punchChargeDistancePreview.transform.localPosition.z);
                }
                else
                {
                    punchChargeSliderLine.transform.localScale = new Vector3(0.15f, 0.15f, punchChargeDistancePreview.transform.localPosition.z);
                }
            }

        }
        //Attack lauch
        if (Input.GetMouseButtonUp(selfParams.attackMouseInput) && hasStartedCharge)
        {
            CmdShowLoadingPunchEnd();
            hasStartedCharge = false;
            punchChargeDisplay.gameObject.SetActive(false);
            punchChargeDistancePreview.SetActive(false);
            punchChargeSliderLine.SetActive(false);
            punchChargeDistancePreview2.SetActive(false);
            //SoundManager.Instance.PlaySoundEvent("PlayerPunch", playerSource);
            //SoundManager.Instance.StopSoundWithDelay(playerSource, 0.2f);
            CmdPlayerSource("PlayerPunch");
            selfMovement.Attack(ratioAttack);
            timeAttack = 0;
            ratioAttack = 0;
        }
    }

    void AttackInputFromJoystick()
	{
        if (Input.GetAxis("RT") > 0 && attackTriggerValueDelta == 0f && !selfMovement.isAttacking && !selfMovement.isAttackInCooldown)
        {
            //SoundManager.Instance.PlaySoundEvent("PlayerPunchCharge", playerSource);
            CmdPlayerSource("PlayerPunchCharge");
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
            //SoundManager.Instance.PlaySoundEvent("PlayerPunch", playerSource);
            //SoundManager.Instance.StopSoundWithDelay(playerSource, 0.2f);
            CmdPlayerSource("PlayerPunch");
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
        punchCooldownSecondDisplay.fillAmount = cdTime / selfParams.punchCooldown;
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

            if (loadingScreen.color.a == 1)
            {
                loadingScreen.CrossFadeAlpha(0, 0.5f, false);
            }

            transform.position = spawnPoint.position; //Obligatoire, sinon ne trouve pas le spawner à la premirèe frame
            selfCollisionParent.transform.localRotation = spawnPoint.rotation;
            selfCamera.localRotation = spawnPoint.rotation;


            //Tp player to the spwan point
            selfSmoothSync.teleportOwnedObjectFromOwner();

            Quaternion startRot = selfCamera.localRotation;
            xRotation = startRot.eulerAngles.x;
            yRotation = startRot.eulerAngles.y;

            //Create timer before restart player
            hudTextPlayer.gameObject.SetActive(true);

            //Lock caméra
            isSpawning = true;

            while (NetworkTime.time - timerToStart <= timerMaxToStart)
            {
                selfMovement.ResetVelocity();
                selfMovement.ResetVerticalVelocity();
                hudTextPlayer.text = System.Math.Round(timerMaxToStart -(NetworkTime.time - timerToStart)).ToString();
                yield return new WaitForEndOfFrame();

            }
            roundStarted = true;
            hudTextPlayer.gameObject.SetActive(false);

            //Unlock Camera
            isSpawning = false;

            //adjust Camera rotation
            xRotation = startRot.eulerAngles.x;
            yRotation = startRot.eulerAngles.y;

            if (GameObject.Find("Analytics") != null)
            {
                GameObject.Find("Analytics").GetComponent<PA_Position>().startWrite = true;
            }
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

        if (GameObject.Find("Analytics") != null)
        {
            GameObject.Find("Analytics").GetComponent<PA_Position>().WriteNewRound();
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
            MyNewNetworkManager.singleton.StopHost();
        }
        else
        {
            MyNewNetworkManager.singleton.StopClient();
        }

        //Destroy GameManager because Already exist on LobbyScene
        DestroyImmediate(GameObject.Find("ServerManager"));

        //Stop Music
        SoundManager.Instance.StopMusic();

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("LobbyScene");
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
        CmdPlayEquipTeamSound("LevelTakenTeam", "LevelTakenEnemy");
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

    [Command(requiresAuthority = false)]
    public void CmdPropulse(Vector3 directedForce)
    {
        RpcPropulse(directedForce);
    }

    [TargetRpc]
    public void RpcPropulse(Vector3 directedForce)
    {
        StartCoroutine(NoControl(0.1f));
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

    [Command]
    private void CmdRotateModel(float rotation)
    {
        RpcRotateModel(rotation);
    }

    [ClientRpc]
    private void RpcRotateModel(float rotation)
    {
        playerModel.transform.rotation = Quaternion.Euler(0,rotation,0);
    }

    //Sound in network

    //Use this for playing audio over network with PlayerSource AudioSource
    [Command(requiresAuthority =false)]
    private void CmdPlayerSource(string thisEventName)
    {
        RpcPlayerSource(thisEventName);
    }
    [ClientRpc]
    private void RpcPlayerSource(string thisEventName)
    {
        SoundManager.Instance.PlaySoundEvent(thisEventName, playerSource);
        
    }

    //Use this for playing audio over network with PlayerFootstepSource AudioSource
    [Command(requiresAuthority = false)]
    private void CmdPlayerFootstepSource(string thisEventName)
    {
        RpcPlayerFootstepSource(thisEventName);
    }
    [ClientRpc]
    private void RpcPlayerFootstepSource(string thisEventName)
    {
        SoundManager.Instance.PlaySoundEvent(thisEventName, playerFootstepSource);
        
    }

    //Use this to stop audio over network with PlayerSource AudioSource
    [Command]
    private void CmdStopPlayerSource()
    {
        RpcStopPlayerSource();
    }

    [ClientRpc]
    public void RpcStopPlayerSource()
    {
        playerSource.Stop();
    }

    //Use this to stop audio over network with PlayerFootstepSource AudioSource
    [Command]
    private void CmdStopPlayerFootstepSource()
    {
        RpcStopPlayerFootstepSource();
    }

    [ClientRpc]
    private void RpcStopPlayerFootstepSource()
    {
        playerFootstepSource.Stop();
    }

    [Command]
    private void CmdPlayGlobalSound(string thisEventName)
    {
        RpcPlayGlobalSound(thisEventName);
    }

    [ClientRpc]
    private void RpcPlayGlobalSound(string thisEventName)
    {
        SoundManager.Instance.PlaySoundEvent(thisEventName);
    }

    //Send a different song for each differente team
    [Command(requiresAuthority = false)]
    public void CmdPlayEquipTeamSound(string eventAllyTeam,string eventEnemyTeam)
    {
        RpcPlayEquipTeamSound(eventAllyTeam, eventEnemyTeam);
    }

    [ClientRpc]
    private void RpcPlayEquipTeamSound(string eventAllyTeam, string eventEnemyTeam)
    {
        if(authorityPlayer.GetComponent<PlayerLogic>().teamName == teamName)
        {
            SoundManager.Instance.PlaySoundEvent(eventAllyTeam);
        }
        else
        {
            SoundManager.Instance.PlaySoundEvent(eventEnemyTeam);
        }
    }

    //Pseudo

    public void SetPseudo(string oldString, string newString)
    {
        player3dPseudo.transform.GetChild(0).GetComponent<Text>().text = newString;
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



