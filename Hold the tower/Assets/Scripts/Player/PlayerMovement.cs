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
    [SerializeField]
    private GameObject selfAttackCollider;

    private Vector3 moveDirection = new Vector3(0, 0, 0);

    private Vector3 hspd;
    private Vector3 vspd;
    private Vector3 attackspd = new Vector3();

    public Vector3 groundCorrection;

    private bool leftCollide, rightCollide, backCollide, frontTopCollide, frontBotCollide;

    [HideInInspector]
    public bool isClimbingMovement;
    [HideInInspector]
    public bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (selfLogic.isGrounded)
        {
            NoGravity();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isClimbingMovement)
        {

        }
        else
        {
            if(vspd.y > 0 && hspd != Vector3.zero) //if apply positive force ( jump ...)
            {
                if(isSomethingCollide())
                {
                    vspd.y = 0;
                    ApplyGravity();
                }
            }

            Debug.Log(" attackspd : "+attackspd);
            Debug.Log("hspd : " + hspd);
            selfRbd.velocity = hspd + vspd + attackspd;
        }
    }
   

    #region HorizontalPhysics
    public void Move(Vector3 direction, float timeStamp)
    {
        moveDirection += new Vector3(direction.x, 0, direction.z);
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
        StartCoroutine(JumpManage());
    }

    public IEnumerator JumpManage()
    {
        selfLogic.isJumping = true;

        for(int i = 0;i< selfParams.jumpNumberToApply; i++)
        {
            if (isSomethingCollide())
            {

            }

            vspd += transform.up * selfParams.topForceJump*Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
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
        isClimbingMovement = true;

        //Vector3 startPosition = transform.position;
        //Vector3 endPosition = transform.position + (selfCamera.forward * selfParams.climbWidth) + transform.up * selfParams.climbHeight; // front and up

        //Vector3 startVelocity = Vector3.zero;
        //Vector3 endVelocity = selfCamera.forward * selfParams.climbWidth + transform.up * selfParams.climbHeight;
        
        Vector3 startVelocityY = Vector3.zero;
        Vector3 EndVelocityY = transform.up * selfParams.climbHeight;

        Vector3 startVelocityX = Vector3.zero;
        Vector3 EndVelocityX = selfCamera.forward * selfParams.climbWidth;

        //Velocity for Y
        float time = 0;
        while (time < selfParams.timeToClimb/2)
        {
            selfRbd.velocity = Vector3.Lerp(startVelocityY, EndVelocityY, time / selfParams.timeToClimb/2); // WIP
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Reset timer
        /* time = 0;

         //Velocity for X
         while (time < selfParams.timeToClimb/2)
         {
             selfRbd.velocity = Vector3.Lerp(startVelocityX, EndVelocityX, time/ selfParams.timeToClimb/2); // WIP
             time += Time.deltaTime;
             yield return new WaitForEndOfFrame();
         }*/

        isClimbingMovement = false;
        selfRbd.collisionDetectionMode = CollisionDetectionMode.Continuous;
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

    #region Attack

    public float AttackLoad(float time)
    {
        float ratio = 0;
        
        //Slow player
        attackspd = hspd * -1;
        attackspd *= selfParams.slowMovementRatio;

        if (!isAttacking) // Will create bugs
        {
            isAttacking = true;
        }

        //Si est en dessous du pickTime
        if(time <= selfParams.timePerfectAttack)
        {
            ratio = time;
        }

        //Si est au pickTime
        if(time <= selfParams.timePerfectAttack + selfParams.timeTreshold && time >= selfParams.timePerfectAttack)
        {
            ratio = 1;
        }

        //Si supérieur au pickTime + treshHold
        if(time > selfParams.timePerfectAttack + selfParams.timeTreshold)
        {
            ratio = 0.8f;
        }

        Debug.Log(ratio);
        return ratio;
    }

    public void Attack(float time)
    {
        StartCoroutine(AttackManage(time));
    }

    public IEnumerator AttackManage(float ratio) //Coroutine gérant le mouvement d'attaque
    {
        selfAttackCollider.SetActive(true);

        Vector3 directionAttack = selfCamera.forward;

        attackspd = Vector3.zero; //init attackSpd Important

        for (int i =0; i< selfParams.forceAttackNumberToApply; i++)
        {
            attackspd += directionAttack * selfParams.forceAttack * Time.fixedDeltaTime* ratio;
            yield return new WaitForFixedUpdate();
        }
        float attackTimeStamp = Time.time;

        isAttacking = false;
        //Decelerate attack speed
        while (attackspd != Vector3.zero)
        {
            DecelerateAttack(Time.time - attackTimeStamp);
            yield return new WaitForFixedUpdate();
        }

        selfAttackCollider.SetActive(false);
    }

    public void DecelerateAttack(float timeStamp)
    {
        if (attackspd != Vector3.zero)
        {
            attackspd = attackspd * selfParams.hspdDeceleration.Evaluate(timeStamp);
        }
    }

    #endregion

    #region Sensor Event

    //All sensors logic concerning Wall Collider
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
