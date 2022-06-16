using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BindManager : MonoBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer playerInput;
    private bool isBindingForward, isBindingBehind, isBindingLeft, isBindingRight, isBindingJump;

    public UnityEvent showNewTextKey;
    public UnityEvent showNewSensi;

    public BindDisplay bindDisplay;

    private void Update()
    {
        if (isBindingForward)
        {
            if(bindDisplay != null)
                bindDisplay.textForwardBind.text = "...";
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("front");
                isBindingForward = false;
            }

            isBindingBehind = false;
            isBindingLeft = false;
            isBindingRight = false;
            isBindingJump = false;
        }

        if (isBindingBehind)
        {
            if (bindDisplay != null)
                bindDisplay.textBehindBind.text = "...";
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("behind");
                isBindingBehind = false;
            }
            isBindingLeft = false;
            isBindingRight = false;
            isBindingJump = false;
            isBindingForward = false;
        }

        if (isBindingLeft)
        {
            if (bindDisplay != null)
                bindDisplay.textLeftBind.text = "...";
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("left");
                isBindingLeft = false;
            }

            isBindingBehind = false;
            isBindingForward = false;
            isBindingRight = false;
            isBindingJump = false;
        }

        if (isBindingRight)
        {
            if (bindDisplay != null)
                bindDisplay.textRightBind.text = "...";
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("right");
                isBindingRight = false;
            }

            isBindingBehind = false;
            isBindingLeft = false;
            isBindingForward = false;
            isBindingJump = false;
        }

        if (isBindingJump)
        {
            if (bindDisplay != null)
                bindDisplay.textJumpBind.text = "...";
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("jump");
                isBindingJump = false;
            }

            isBindingBehind = false;
            isBindingLeft = false;
            isBindingForward = false;
            isBindingRight = false;
        }
    }
    public void BindForward()
    {
        isBindingForward = true;
    }

    public void BindBehind()
    {
        isBindingBehind = true;
    }

    public void BindLeft()
    {
        isBindingLeft = true;
    }

    public void BindRight()
    {
        isBindingRight = true;
    }

    public void BindJump()
    {
        isBindingJump = true;
    }

    public void BindSensivity(float sensivity)
    {
        playerInput.mouseSensivity = (int)sensivity;
        SaveManager.SaveParams(ref playerInput);
        showNewSensi.Invoke();
    }


    private void BindSave(string key)
    {
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(vKey))
            {
                switch (key)
                {
                    case "front":
                        playerInput.front = vKey;
                        break;
                    case "behind":
                        playerInput.back = vKey;
                        break;
                    case "left":
                        playerInput.left = vKey;
                        break;
                    case "right":
                        playerInput.right = vKey;
                        break;
                    case "jump":
                        playerInput.jump = vKey;
                        break;
                }
            }
        }
        SaveManager.SaveParams(ref playerInput);
        showNewTextKey.Invoke();
    }

    private bool IgnoreKey()
    {
        return !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) && !Input.GetKeyDown(KeyCode.Escape);
    }
}
