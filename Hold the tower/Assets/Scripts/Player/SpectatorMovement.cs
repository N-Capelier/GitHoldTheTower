using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SpectatorMovement : NetworkBehaviour
{

    #region Input Key
    private KeyCode front = KeyCode.Z;
    private KeyCode back = KeyCode.S;
    private KeyCode up = KeyCode.Space;
    private KeyCode down = KeyCode.LeftShift;
    private KeyCode left = KeyCode.Q;
    private KeyCode right = KeyCode.D;
    #endregion

    #region var
    private bool isfront, isback, isup, isdown, isleft, isright;

    private Transform selfTransform;
    private float mouseSensivity = 100f;

    private float speed = 0.5f;

    //Camera
    public Transform selfCamera;
    private float yRotation;
    private float xRotation;
    #endregion


    void Start()
    {
        selfTransform = this.transform;
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
            fpsView();
            if (Input.GetKey(front))
            {
                isfront = true;
            }

            if (Input.GetKey(back))
            {
                isback = true;
            }

            if (Input.GetKey(left))
            {
                isleft = true;
            }

            if (Input.GetKey(right))
            {
                isright = true;
            }

            if (Input.GetKey(up))
            {
                isup = true;
            }

            if (Input.GetKey(down))
            {
                isdown = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isfront)
        {
            isfront = false;
            selfTransform.position += selfCamera.forward * speed;
        }

        if (isback)
        {
            isback = false;
            selfTransform.position += -selfCamera.forward * speed;
        }

        if (isleft)
        {
            isleft = false;
            selfTransform.position += -selfCamera.right * speed;
        }

        if (isright)
        {
            isright = false;
            selfTransform.position += selfCamera.right * speed;
        }

        if (isup)
        {
            isup = false;
            selfTransform.position += transform.up * speed;
        }

        if (isdown)
        {
            isdown = false;
            selfTransform.position += -transform.up * speed;
        }

    }

    private void fpsView()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        #if UNITY_EDITOR
        mouseX = Input.GetAxis("Mouse X") * mouseSensivity * 4f * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * 4f * Time.deltaTime;
        #endif


        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);

        selfCamera.Rotate(Vector3.up * mouseX);
        selfCamera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
