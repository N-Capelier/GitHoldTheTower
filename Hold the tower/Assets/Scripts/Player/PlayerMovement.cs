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

    private bool leftWall, rightWall, backWall, frontTopWall, frontBotWall;

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

    #region VerticalPhysics

    #endregion

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
            Debug.Log("Jump");
            vspd += transform.up * selfParams.topForceJump*Time.fixedDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        selfLogic.isJumping = false;
    }

    //manage jump
    #endregion

    #region Climb
    public bool CanClimb()
    {
        if(frontBotWall && !frontTopWall && !isClimbingMovement)
        {
            isClimbingMovement = true;
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
       


        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + (selfCamera.forward * 5f) + transform.up * 5f; // front and up
        float time = Time.deltaTime;

        while ((endPosition - transform.position).magnitude > 5f)
        {
            //Debug.Log((endPosition - transform.position).magnitude);
            transform.position = Vector3.Lerp(startPosition,endPosition, time);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        vspd = Vector3.zero;
        frontBotWall = false;
        frontTopWall = true;
        selfRbd.isKinematic = false;
        isClimbingMovement = false;
        selfRbd.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Debug.Log("End climb");
    }
    #endregion

    #region Sensor Event

    public void onLeftCollide()
    {
        leftWall = true;
    }

    public void onLeftUnCollide()
    {
        leftWall = false;
    }

    public void onRightCollide()
    {
        rightWall = true;
    }

    public void onRightUnCollide()
    {
        rightWall = false;
    }

    public void onBackCollide()
    {
        backWall = true;
    }

    public void onBackUnCollide()
    {
        backWall = false;
    }

    public void onFrontTopCollide()
    {
        frontTopWall = true;
    }

    public void onFrontTopUnCollide()
    {
        frontTopWall = false;
        CanClimb();
    }

    public void onFrontBotCollide()
    {
        frontBotWall = true;
        CanClimb();
    }

    public void onFrontBotUnCollide()
    {
        frontBotWall = false;
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
