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

    private void fpsView()
    {
        float mouseX = Input.GetAxis("Mouse X") * selfParams.mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * selfParams.mouseSensivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);

        selfCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        selfCamera.Rotate(Vector3.up * mouseX);
    }

    private void HorizontalMovement()
    {
        if (!selfMovement.isClimbingMovement)
        {
            if (Input.GetKey(selfParams.left) || Input.GetKey(selfParams.right) || Input.GetKey(selfParams.front) || Input.GetKey(selfParams.back))
            {
                timeStampRunDecel = Time.time;

                if (Input.GetKey(selfParams.front))
                {
                    selfMovement.Move(selfCamera.forward, Time.time - timeStampRunAccel);
                    selfMovement.CanClimb();
                    /*if (selfMovement.CanClimb())
                        Debug.Log("climb");*/
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
                }
                else
                {
                    selfMovement.ApplyGravity();
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
            }
        }
        
    }
}
