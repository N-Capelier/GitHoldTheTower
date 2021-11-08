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
    public string teamName;

    private float yRotation, xRotation;

    private float timeStampRunAccel, timeStampRunDecel;

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
                    selfMovement.Move(selfCamera.forward, Time.time - timeStampRunAccel);
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
            else
            {
                selfMovement.Decelerate(Time.time - timeStampRunDecel);
                timeStampRunAccel = Time.time;
            }

            if (Input.GetMouseButtonDown(selfParams.attackMouseInput))
            {
                selfMovement.Attack();
            }
        }
        else
        {

        }
    }

    private void VerticalMovement()
    {
        if (!selfMovement.isClimbingMovement)
        {
            if (!isGrounded)
            {
                if (selfMovement.isSomethingCollide() && Input.GetMouseButton(selfParams.wallMouseInput))
                {
                    selfMovement.NoGravity();
                    isAttachToWall = true;
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
                    selfMovement.NoGravity();
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

    #region Network logic

    #endregion
}
