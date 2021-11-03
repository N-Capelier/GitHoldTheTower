using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    private Rigidbody selfRbd;
    [SerializeField]
    private Transform selfCamera;

    private Vector3 moveDirection = new Vector3(0, 0, 0);

    private Vector3 hspd;
    private Vector3 vspd;

    private bool leftCollide, rightCollide, backCollide, frontTopCollide, frontBotCollide;

    [SerializeField]
    public bool isClimbingMovement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isClimbingMovement)
        {

        }
        else
        {
            selfRbd.velocity = hspd + vspd;
        }

    }

   

    #region HorizontalPhysics
    public void Move(Vector3 direction, float timeStamp)
    {
        moveDirection += direction;
        moveDirection = moveDirection.normalized;

        hspd = moveDirection * selfParams.hspdForce * selfParams.hspdAcceleration.Evaluate(timeStamp);
        hspd = new Vector3(hspd.x, 0, hspd.z);
    }

    public void Decelerate(float timeStamp)
    {
        if(hspd != Vector3.zero)
        {
            hspd = hspd * selfParams.hspdDeceleration.Evaluate(timeStamp);
        }
    }
    #endregion

    #region VerticalPhysics

    public void ApplyGravity()
    {
        vspd += new Vector3(0, -selfParams.gravity, 0) * Time.deltaTime;
    }

    public void NoGravity()
    {
        vspd = Vector3.zero;
    }
    //WIP need to adjust jump to interact with wall
    public void Jump()
    {
        vspd = Vector3.zero;
        StartCoroutine("JumpManage");
    }

    public IEnumerator JumpManage()
    {
        selfLogic.isJumping = true;

        for(int i = 1;i< selfParams.jumpNumberToApply; i++)
        {
            if(IsFrontCollide() || leftCollide || rightCollide || backCollide)
            {
                if (IsFrontCollide() || backCollide) 
                {
                    hspd.z = 0;
                }

                if (leftCollide || rightCollide)
                {
                    hspd.x = 0;
                }

            }
            vspd += transform.up * selfParams.topForceJump*Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        selfLogic.isJumping = false;
    }

    public void WallJump()
    {

    }

    public IEnumerator WallJumpManage()
    {
        return null;
    }
    //manage jump
    #endregion

    #region Climb
    public bool CanClimb()
    {
        if(frontBotCollide && !frontTopCollide && !isClimbingMovement)
        {
            Climb();
            return true;
        }
        return false;
    }

    public void Climb()
    {
        StartCoroutine("ClimbManage");
    }

    public IEnumerator ClimbManage()
    {
        selfRbd.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        selfRbd.isKinematic = true;
        isClimbingMovement = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + (selfCamera.forward * selfParams.climbWidth) + transform.up * selfParams.climbHeight; // front and up
        float time = Time.deltaTime;

        while ((endPosition - transform.position).magnitude > selfParams.climbPrecision)
        {
            //Debug.Log((endPosition - transform.position).magnitude);
            transform.position = Vector3.Lerp(startPosition,endPosition, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        vspd = Vector3.zero;

        selfRbd.isKinematic = false;
        isClimbingMovement = false;
        selfRbd.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Debug.Log("End climb");
    }
    #endregion

    #region WallDetection

    private bool IsFrontCollide()
    {
        if (frontTopCollide && frontBotCollide)
        {
            return true;
        }

        return false;
    }

    public bool isSomethingCollide() //is there any wall collide
    {
        if(IsFrontCollide() || leftCollide || rightCollide || backCollide){
            return true;
        }

        return false;
    }

    #endregion

    #region Sensor Event

    public void onLeftCollide()
    {
        leftCollide = true;
    }

    public void onLeftUnCollide()
    {
        leftCollide = false;
    }

    public void onRightCollide()
    {
        rightCollide = true;
    }

    public void onRightUnCollide()
    {
        rightCollide = false;
    }

    public void onBackCollide()
    {
        backCollide = true;
    }

    public void onBackUnCollide()
    {
        backCollide = false;
    }

    public void onFrontTopCollide()
    {
        frontTopCollide = true;
    }

    public void onFrontTopUnCollide()
    {
        frontTopCollide = false;
    }

    public void onFrontBotCollide()
    {
        frontBotCollide = true;
    }

    public void onFrontBotUnCollide()
    {
        frontBotCollide = false;
    }

    public void isGrounded()
    {
        selfLogic.isGrounded = true;
    }

    public void isNotGrounded()
    {
        selfLogic.isGrounded = false;
    }

    #endregion
}
