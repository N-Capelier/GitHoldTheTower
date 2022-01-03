using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindManager : MonoBehaviour
{
    [SerializeField]
    private ScriptableParamsPlayer playerInput;
    private bool isBindingForward, isBindingBehind, isBindingLeft, isBindingRight, isBindingJump;

    private void Update()
    {
        if (isBindingForward)
        {
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("front");
                isBindingForward = true;
            }

            isBindingBehind = false;
            isBindingLeft = false;
            isBindingRight = false;
            isBindingJump = false;
        }

        if (isBindingBehind)
        {
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("behind");
                isBindingBehind = true;
            }
            isBindingLeft = false;
            isBindingRight = false;
            isBindingJump = false;
            isBindingForward = false;
        }

        if (isBindingLeft)
        {
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("left");
                isBindingLeft = true;
            }

            isBindingBehind = false;
            isBindingForward = false;
            isBindingRight = false;
            isBindingJump = false;
        }

        if (isBindingRight)
        {
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("right");
                isBindingRight = true;
            }

            isBindingBehind = false;
            isBindingLeft = false;
            isBindingForward = false;
            isBindingJump = false;
        }

        if (isBindingJump)
        {
            if (Input.anyKeyDown && IgnoreKey())
            {
                BindSave("jump");
                isBindingJump = true;
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

    }

    private bool IgnoreKey()
    {
        return !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2) && !Input.GetKeyDown(KeyCode.Escape);
    }
}
