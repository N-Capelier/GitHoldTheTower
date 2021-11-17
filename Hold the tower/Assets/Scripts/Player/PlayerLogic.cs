using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerLogic : NetworkBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private Transform selfCamera;
    [SerializeField]
    private PlayerMovement selfMovement;

    [SyncVar]
    public LobbyPlayerLogic.nameOfTeam teamName;

    private float yRotation, xRotation;

    private float timeStampRunAccel, timeStampRunDecel;
    private float timeAttack, ratioAttack;

    [HideInInspector]
    public Vector3 normalWallJump;

    //State
    [HideInInspector]
    public bool isGrounded, isJumping, isAttachToWall;
    // Start is called before the first frame update
    void Start()
    {
        selfCamera.gameObject.SetActive(false);
        if (hasAuthority)
        {
            selfCamera.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            fpsView();
            VerticalMovement();
            HorizontalMovement();
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {

        }

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

    }

    private void HorizontalMovement()
    {
        
        if (!selfMovement.isClimbingMovement && !isAttachToWall && !selfMovement.isAttacking)
        {
            if (Input.GetKey(selfParams.left) || Input.GetKey(selfParams.right) || Input.GetKey(selfParams.front) || Input.GetKey(selfParams.back))
            {
                timeStampRunDecel = Time.time;

                if (Input.GetKey(selfParams.front))
                {
                    if (isGrounded)
                    {
                        selfMovement.Move(selfCamera.forward, Time.time - timeStampRunAccel);
                    }
                    else
                    {
                        selfMovement.Move(selfCamera.forward, Time.time - timeStampRunAccel);
                    }
                    
                    selfMovement.CanClimb();
                }

                if (Input.GetKey(selfParams.back))
                {
                    selfMovement.Move(-selfCamera.forward, Time.time - timeStampRunAccel);
                }

                if (Input.GetKey(selfParams.left))
                {
                    selfMovement.Move(-selfCamera.right, Time.time - timeStampRunAccel);
                }

                if (Input.GetKey(selfParams.right))
                {
                    selfMovement.Move(selfCamera.right, Time.time - timeStampRunAccel);
                }

            }
            else //Decelerate hspd
            {
                selfMovement.Decelerate(Time.time - timeStampRunDecel);
                timeStampRunAccel = Time.time;
            }

        }
        else
        {

        }

        //Attack Logic
        if (!selfMovement.isClimbingMovement && !selfMovement.isAttacking)
        {
            AttackInput();
        }
    }

    private void VerticalMovement()
    {
        if (!selfMovement.isClimbingMovement)
        {
            if (!isGrounded)
            {
                if (selfMovement.IsSomethingCollide() && Input.GetMouseButton(selfParams.wallMouseInput))
                {
                    selfMovement.NoGravity();
                    isAttachToWall = true;
                    if (Input.GetKeyDown(selfParams.jump))
                    {
                       selfMovement.StopMovement();
                       selfMovement.WallJump(normalWallJump);
                    }
                }
                else
                {
                    selfMovement.ApplyGravity();
                    isAttachToWall = false;
                }

            }
            else
            {
                if (!isJumping)
                {
                    //selfMovement.NoGravity();
                }

                if (Input.GetKeyDown(selfParams.jump) && !isJumping)
                {
                    selfMovement.Jump();
                }

                if (selfMovement.isAttacking) //Enable attack if on ground to change maybe a timer
                {
                    selfMovement.isAttacking = false;
                }
            }
        }

    }

    #endregion

    #region Collision
    public void OnCollisionEnter(Collision info)
    {
        Vector3 point = info.contacts[0].point;
        normalWallJump = info.contacts[0].normal;

        Debug.DrawRay(point, (normalWallJump + new Vector3(0,0.5f,0)) * 10,Color.red,2.5f);
    }

    #endregion

    #region AttackLogic
    public void getHit()
    {

    }

    public void cantBeHit()
    {

    }

    public void AttackInput()
    {
        //Attack load
        if (Input.GetMouseButton(selfParams.attackMouseInput))
        {
            timeAttack += Time.deltaTime;
            ratioAttack = selfMovement.AttackLoad(timeAttack);
        }
        //Attack lauch
        if (Input.GetMouseButtonUp(selfParams.attackMouseInput))
        {
            selfMovement.Attack(ratioAttack);
            timeAttack = 0;
            ratioAttack = 0;
        }
    }
    #endregion

    #region Network logic
    #endregion
}



