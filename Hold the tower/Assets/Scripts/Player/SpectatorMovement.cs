using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SpectatorMovement : NetworkBehaviour
{

    #region Input Key
    private KeyCode down = KeyCode.LeftShift;
    public ScriptableParamsPlayer spectatorParams;
    #endregion

    #region var

    public Transform targetTransform;
    public Transform transformToMove;
    private bool isfront, isback, isup, isdown, isleft, isright;

    private float speed = 0.5f;

    //Camera
    public Transform selfCamera;
    private float yRotation;
    private float xRotation;

    public AnimationCurve curveSpeed;

    private SpectatorMenu selfMenu;
    #endregion


    void Start()
    {
        selfMenu = GetComponent<SpectatorMenu>();
        Cursor.lockState = CursorLockMode.Locked;
        selfCamera.gameObject.SetActive(false);
        if (hasAuthority)
        {
            selfCamera.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            if (!selfMenu.menuActive)
            {
                if (!selfMenu.spectatorIsFocus)
                {
                    fpsView();
                }
                else
                {
                    
                }

                if (Input.GetKey(spectatorParams.front))
                {
                    isfront = true;
                    StopFocus();
                }

                if (Input.GetKey(spectatorParams.back))
                {
                    isback = true;
                    StopFocus();
                }

                if (Input.GetKey(spectatorParams.left))
                {
                    isleft = true;
                    StopFocus();
                }

                if (Input.GetKey(spectatorParams.right))
                {
                    isright = true;
                    StopFocus();
                }

                if (Input.GetKey(spectatorParams.jump))
                {
                    isup = true;
                    StopFocus();
                }

                if (Input.GetKey(down))
                {
                    isdown = true;
                    StopFocus();
                }
                
            }

            //Si ne focus pas un jouer, donne sa position
            if (!selfMenu.spectatorIsFocus)
            {
                transformToMove.position = Vector3.Lerp(transformToMove.position, targetTransform.position, 0.05f);
                transformToMove.rotation = Quaternion.Lerp(transformToMove.localRotation, targetTransform.rotation, 0.05f);
                transformToMove.rotation = Quaternion.Euler(transformToMove.rotation.eulerAngles.x, transformToMove.rotation.eulerAngles.y, 0);
            }
            else
            {
                if(selfMenu.playerToFocus != null)

                {   if ((transformToMove.position - selfMenu.playerToFocus.transform.position).magnitude > 10f)
                    {
                        transformToMove.position = Vector3.Lerp(transformToMove.position, selfMenu.playerToFocus.transform.position, 0.05f);
                    }
                    else
                    {
                        Debug.Log((transformToMove.position - selfMenu.playerToFocus.transform.position).magnitude);
                        selfMenu.nearFocusX = true;
                    }

                    if(selfMenu.nearFocusX)
                    {
                        transformToMove.LookAt(selfMenu.playerToFocus.transform.position);
                        transformToMove.RotateAround(selfMenu.playerToFocus.transform.position, selfMenu.playerToFocus.transform.up, 20 * Time.deltaTime);
                    }
                }

                

                //transformToMove.position = selfMenu.playerToFocus.transform.position;
                //transformToMove.rotation = selfMenu.playerToFocus.transform.rotation;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isfront)
        {
            isfront = false;
            targetTransform.position += selfCamera.forward * speed;
        }

        if (isback)
        {
            isback = false;
            targetTransform.position += -selfCamera.forward * speed;
        }

        if (isleft)
        {
            isleft = false;
            targetTransform.position += -selfCamera.right * speed;
        }

        if (isright)
        {
            isright = false;
            targetTransform.position += selfCamera.right * speed;
        }

        if (isup)
        {
            isup = false;
            targetTransform.position += transform.up * speed;
        }

        if (isdown)
        {
            isdown = false;
            targetTransform.position += -transform.up * speed;
        }

    }

    private void StopFocus()
    {
        selfMenu.nearFocusX = false;
        selfMenu.nearFocusY = false;
        selfMenu.nearFocusZ = false;

        selfMenu.playerToFocus = null;
        selfMenu.spectatorIsFocus = false;
    }

    private void fpsView()
    {
        float mouseX = Input.GetAxis("Mouse X") * (spectatorParams.mouseSensivity/2) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * (spectatorParams.mouseSensivity/2) * Time.deltaTime;

        #if UNITY_EDITOR
        mouseX = Input.GetAxis("Mouse X") * (spectatorParams.mouseSensivity/2) * 4f * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * (spectatorParams.mouseSensivity/2) * 4f * Time.deltaTime;
        #endif


        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);


        targetTransform.Rotate(Vector3.up * mouseX);
        targetTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
