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
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private Transform selfCamera;
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
    private Text hudTextPlayer;
    [SerializeField]
    private Text scoreTextBlue;
    [SerializeField]
    private Text scoreTextRed;

    [SerializeField]
    private GameObject FlagObject;
    [SerializeField]
    private GameObject FlagInGame;

    [SyncVar]
    public LobbyPlayerLogic.nameOfTeam teamName;
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


    private void Awake()
    {

    }


    void Start()
    {
       
        if (FlagObject != null)
        {
            FlagObject = GameObject.Find("Flag");
        }
        isInControl = true;
        
        selfCamera.gameObject.SetActive(false);
        if (hasAuthority)
        {
           
            matchManager = GameObject.Find("GameManager").GetComponent<MatchManager>();
            selfCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            ShowScoreHud();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority && roundStarted)
        {
            fpsView();
            VerticalMovement();
            HorizontalMovement();
        }
        else
        {
            if (roundStarted)
            {
                
            }
        }
        ShowFlagToAllPlayer();
    }

    #region Movement Logic

    private void fpsView()
    {
        float mouseX = Input.GetAxis("Mouse X") * selfParams.mouseSensivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * selfParams.mouseSensivity * Time.fixedDeltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);

        selfCamera.Rotate(Vector3.up * mouseX);
        selfCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        selfCollisionParent.transform.localRotation = Quaternion.Euler(new Vector3(0, selfCamera.rotation.eulerAngles.y, 0));

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
            if (Input.GetKey(selfParams.left) || Input.GetKey(selfParams.right) || Input.GetKey(selfParams.front) || Input.GetKey(selfParams.back))
            {
                timeStampRunDecel = Time.time;
                Vector3 keyDirection = Vector3.zero;

                if (Input.GetKey(selfParams.front))
                {
                    keyDirection += GetHorizontalVector(selfCamera.forward);
                    selfMovement.CanClimb();
                }

                if (Input.GetKey(selfParams.back))
                {
                    keyDirection += GetHorizontalVector(-selfCamera.forward);
                }

                if (Input.GetKey(selfParams.left))
                {
                    keyDirection += GetHorizontalVector(-selfCamera.right);
                }

                if (Input.GetKey(selfParams.right))
                {
                    keyDirection += GetHorizontalVector(selfCamera.right);
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
        else
        {

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
        if (!selfMovement.isClimbingMovement)
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
        if (Input.GetMouseButtonDown(selfParams.attackMouseInput) && !selfMovement.isAttacking && !selfMovement.isAttackInCooldown)
        {
            SoundManager.Instance.PlaySoundEvent("PlayerPunchCharge", playerSource);
            hasStartedCharge = true;
        }
        //Attack load
        if (Input.GetMouseButton(selfParams.attackMouseInput) && hasStartedCharge)
        {
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
            hasStartedCharge = false;
            punchChargeDisplay.gameObject.SetActive(false);
            SoundManager.Instance.PlaySoundEvent("PlayerPunch", playerSource);
            SoundManager.Instance.StopSoundWithDelay(playerSource, 0.2f);
            selfMovement.Attack(ratioAttack);
            timeAttack = 0;
            ratioAttack = 0;
        }
    }

    // faire fonction de d�lai d'affichage de la charge du punch

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
        transform.position = GameObject.FindWithTag("Spawner").transform.GetChild(spawnPosition).position; //Obligatoire, sinon ne trouve pas le spawner à la premirèe frame
        selfSmoothSync.teleportOwnedObjectFromOwner();
        hudTextPlayer.gameObject.SetActive(true);
        while (NetworkTime.time - timerToStart <= timerMaxToStart)
        {
            selfMovement.StopPlayer();
            selfMovement.StopMovement();
            selfMovement.NoGravity();
            if (System.Math.Round(NetworkTime.time - timerToStart).ToString() != hudTextPlayer.text)
                hudTextPlayer.text = System.Math.Round(NetworkTime.time - timerToStart).ToString();
            yield return new WaitForEndOfFrame();

        }
        roundStarted = true;
        hudTextPlayer.gameObject.SetActive(false);
    }

    [TargetRpc]
    public void RpcShowGoal(NetworkConnection conn,string text)
    {
        timerToStart = NetworkTime.time;
        StartCoroutine(GoalMessageManager(text));
    }

    public IEnumerator GoalMessageManager(string text)
    {
        ShowScoreHud();
        hudTextPlayer.gameObject.SetActive(true);
        while (NetworkTime.time - timerToStart <= timerMaxToStart)
        {
            if (text != hudTextPlayer.text)
                hudTextPlayer.text = text;
            yield return new WaitForEndOfFrame();

        }
        selfMovement.StopMovement();
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
        ShowScoreHud();
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
        Debug.Log("Recoit le punch");
        StartCoroutine(NoControl(selfParams.punchedNoControlTime));
        selfMovement.Propulse(directedForce);
    }

    public void GetPunch(Vector3 directedForce)
    {
        Debug.Log("Recoit le punch");
        StartCoroutine(NoControl(selfParams.punchedNoControlTime));
        selfMovement.Propulse(directedForce);
    }

    public IEnumerator NoControl(float time)
    {
        isInControl = false;
        yield return new WaitForSeconds(time);
        isInControl = true;
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
    private void ShowScoreHud()
    {
        scoreTextRed.text = matchManager.redScore.ToString();
        scoreTextBlue.text = matchManager.blueScore.ToString();

    }
    #endregion
}



