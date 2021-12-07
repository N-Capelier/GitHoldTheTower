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
    public GameObject selfAttackCollider;

    private Vector3 moveDirection = new Vector3(0, 0, 0);

    private Vector3 hspd;
    private Vector3 vspd;
    private Vector3 attackspd = new Vector3();
    private Vector3 WallJumpspd;

    public Vector3 groundCorrection;


    private bool leftCollide, rightCollide, backCollide, frontTopCollide, frontBotCollide;

    private bool canWallJump = true;
    private bool stopPunchFlag;

    [HideInInspector]
    public bool isClimbingMovement;
    [HideInInspector]
    public bool isAttacking;
    [HideInInspector]
    public bool isAttackReset;
    [HideInInspector]
    public bool isAttackInCooldown;
    [HideInInspector]
    public Vector3 directionAttack;

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
            if(hspd != Vector3.zero && !selfLogic.isGrounded) //if apply positive force ( jump ...)
            {
                if(IsSomethingCollide())
                {
                    /*
                    //WIP a changer
                    if (Mathf.Sign(hspd.x) == Mathf.Sign(-selfLogic.normalWallJump.x))
                    {
                        hspd.x = 0;
                    }

                    if (Mathf.Sign(hspd.z) == Mathf.Sign(-selfLogic.normalWallJump.z))
                    {
                        hspd.z = 0;
                    }

                    if (Mathf.Sign(hspd.x) == Mathf.Sign(-selfLogic.normalWallJump.x) || Mathf.Sign(hspd.z) == Mathf.Sign(-selfLogic.normalWallJump.z))
                    {
                        attackspd = Vector3.zero;
                        WallJumpspd = Vector3.zero;
                    }*/
                    
                }
            }

            selfRbd.velocity = hspd + vspd + attackspd;
        }
    }
   

    #region HorizontalPhysics
    public void Move(Vector3 direction, float timeStamp)
    {
        moveDirection += new Vector3(direction.x, 0, direction.z);
        moveDirection = moveDirection.normalized;

        hspd = moveDirection * selfParams.maxRunningSpeed * selfParams.runningAcceleration.Evaluate(timeStamp);
        hspd = new Vector3(hspd.x, 0, hspd.z);
    }

    public void AirMove(Vector3 direction)
    {
        moveDirection += new Vector3(direction.x, 0, direction.z);
        moveDirection = moveDirection.normalized;

        float hspdMagnitude = hspd.magnitude;
        Vector3 newHspd = hspd + moveDirection * selfParams.airControlForce * Time.deltaTime;

        hspd += moveDirection * selfParams.airControlForce * Time.deltaTime;

        if (hspd.magnitude > selfParams.maxRunningSpeed)
        {
            hspd = hspd.normalized * hspdMagnitude;
        }
    }

    public void Decelerate(float timeStamp)
    {
        if(hspd != Vector3.zero)
        {
            hspd = hspd * selfParams.runningDeceleration.Evaluate(timeStamp);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }

    public void StopMovement()
    {
        hspd = Vector3.zero;
    }

    public void StopPlayer()
    {
        selfRbd.velocity = Vector3.zero;
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
        NoGravity();
        for(int i = 0;i< selfParams.jumpNumberToApply; i++)
        {
            if(isClimbingMovement)
            {
                break;
            }
            vspd += transform.up * selfParams.topForceJump * Time.fixedDeltaTime;
            if(Input.GetKey(selfParams.front))
            {
                hspd += selfLogic.GetHorizontalVector(selfCamera.forward) * selfParams.forwardForceJump * Time.fixedDeltaTime;
            }
            yield return new WaitForFixedUpdate();
        }
        
        selfLogic.isJumping = false;
    }

    //start WallJump
    public void WallJump(Vector3 direction)
    {
        if (canWallJump)
        {
            StartCoroutine(WallJumpManage(direction));
            canWallJump = true;
        }
        
    }

    //Manage walljump movement
    public IEnumerator WallJumpManage(Vector3 wallDirection)
    {
        isAttackReset = true;
        Vector3 adjustDirection = wallDirection;

        Vector3 moveKeyDirection = Vector3.zero;
        float angleDist = 0;

        moveKeyDirection = selfCamera.forward;
        moveKeyDirection = new Vector3(moveKeyDirection.x, 0, moveKeyDirection.z);
        moveKeyDirection.Normalize();

        if (moveKeyDirection != Vector3.zero)
        {
            moveKeyDirection.Normalize();
            float jumpAngle = Vector3.SignedAngle(Vector3.right, moveKeyDirection, Vector3.up);
            float wallAngle = Vector3.SignedAngle(Vector3.right, wallDirection, Vector3.up);

            angleDist = jumpAngle - wallAngle;
            if(angleDist > 180)
            {
                angleDist -= 360;
            }
            else if (angleDist < -180)
            {
                angleDist += 360;
            }

            if (Mathf.Abs(angleDist) > selfParams.wallJumpMinAngleToCancelDeviation)
            {
                jumpAngle = wallAngle;
            }
            else
            {
                if (angleDist > selfParams.maxWallJumpAngleDeviation)
                {
                    jumpAngle = wallAngle + selfParams.maxWallJumpAngleDeviation;
                }

                if (angleDist < -selfParams.maxWallJumpAngleDeviation)
                {
                    jumpAngle = wallAngle - selfParams.maxWallJumpAngleDeviation;
                }
            }

            adjustDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * jumpAngle), 0, -Mathf.Sin(Mathf.Deg2Rad * jumpAngle));

            adjustDirection.Normalize();
        }

        adjustDirection += new Vector3(0, selfParams.upWardWallJumpForce, 0);


        //Add force
        /*
        while (_timer < selfParams.forceToWallJumpCurve[selfParams.forceToWallJumpCurve.length-1].time)
        {
            WallJumpspd = adjustDirection * (selfParams.forceToWallJumpCurve.Evaluate(_timer) * Time.fixedDeltaTime * selfParams.forceToWallJump);
            _timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }*/

        WallJumpspd = adjustDirection * selfParams.forceToWallJump;

        StopMovement();
        NoGravity();
        canWallJump = true;
        hspd = new Vector3(WallJumpspd.x, 0, WallJumpspd.z);
        vspd = new Vector3(0, WallJumpspd.y, 0);

        WallJumpspd = Vector3.zero;
        yield return new WaitForEndOfFrame();
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
        isClimbingMovement = true;
        Vector3 climbEndGroundPos = transform.position;
        climbEndGroundPos += (selfParams.climbHeight * Vector3.up) + (selfLogic.GetHorizontalVector(selfCamera.forward) * selfParams.climbWidth);
        RaycastHit endPosHit;
        Physics.Raycast(climbEndGroundPos, Vector3.down,out endPosHit, 2f, LayerMask.GetMask("Outlined"));
        if (endPosHit.collider != null)
        {
            NoGravity();
            StopMovement();
            climbEndGroundPos = endPosHit.point + Vector3.up;
            float timer = selfParams.timeToClimb;
            Vector3 startClimbPos = transform.position;
            Vector3 currentPos = startClimbPos;
            Vector3 lastPos = startClimbPos;
            Vector3 currentVelocity = Vector3.zero;
            selfLogic.CmdSwitchCollider(true);
            while (timer > 0)
            {
                lastPos = currentPos;
                currentPos = Vector3.Lerp(startClimbPos, climbEndGroundPos, selfParams.climbSpeedOverTime.Evaluate(1 - (timer / selfParams.timeToClimb)));
                currentVelocity = (currentPos - lastPos)/Time.fixedDeltaTime;
                selfRbd.velocity = currentVelocity;

                timer -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            //transform.position = climbEndGroundPos;
        }
        //hspd = new Vector3(selfRbd.velocity.x, 0, selfRbd.velocity.z);
        moveDirection = selfLogic.GetHorizontalVector(hspd);
        selfLogic.CmdSwitchCollider(false);
        isClimbingMovement = false;
    }

    #endregion

    #region WallDetection

    private bool IsFrontCollide()
    {
        if (/*frontTopCollide && */frontBotCollide)
        {
            return true;
        }

        return false;
    }

    public bool IsSomethingCollide() //is there any wall collide
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
        if(selfLogic.isGrounded)
        {
            attackspd = hspd * -1;
            attackspd *= selfParams.chargeSlowMovementRatio;
        }

        //Si est en dessous du pickTime
        if(time <= selfParams.punchPerfectTiming)
        {
            ratio = selfParams.punchSpeedByCharge.Evaluate(time/selfParams.punchMaxChargeTime);
        }

        //Si est au pickTime
        if(time <= selfParams.punchPerfectTiming + selfParams.punchPerfectTimingTreshold && time >= selfParams.punchPerfectTiming)
        {
            ratio = selfParams.punchPerfectTimingPropulsionMultiplier;
        }

        //Si sup�rieur au pickTime + treshHold
        if(time > selfParams.punchPerfectTiming + selfParams.punchPerfectTimingTreshold)
        {
            ratio = 1;
        }

        return ratio;
    }

    public void Attack(float time)
    {
        if(!isAttacking)
        {
            StartCoroutine(AttackManage(time));
        }
    }

    public IEnumerator AttackManage(float ratio) //Coroutine g�rant le mouvement d'attaque
    {
        selfAttackCollider.SetActive(true);
        //StopMovement(); ////////////////////////////////////////////////////////////////////////////// Check merge conflict ///////////////////////////////////////////////////
        selfLogic.CmdAttackCollider(true);

        directionAttack = selfCamera.forward;
        attackspd = Vector3.zero; //init attackSpd Important
        isAttacking = true;
        isAttackReset = false;

        float _time = 0;
        while(_time <selfParams.velocityCurve[selfParams.velocityCurve.length - 1].time && !stopPunchFlag)
        {
            attackspd = directionAttack * selfParams.velocityCurve.Evaluate(_time) * Time.fixedDeltaTime * ratio * selfParams.punchBaseSpeed;
            _time += Time.deltaTime;
            NoGravity();
            yield return new WaitForFixedUpdate();
        }
        stopPunchFlag = false;
        float attackTimeStamp = Time.time;

        StartCoroutine(timerAttack());

        isAttackInCooldown = true;
        selfAttackCollider.SetActive(false);
        selfLogic.CmdAttackCollider(false);

        //active Wall Jump if player punch
        canWallJump = true;
        attackspd = directionAttack * selfParams.velocityCurve.Evaluate(selfParams.velocityCurve[selfParams.velocityCurve.length - 1].time) * Time.fixedDeltaTime * ratio * selfParams.punchBaseSpeed;

        hspd = new Vector3(attackspd.x, 0, attackspd.z);
        vspd = new Vector3(0, attackspd.y, 0);
        attackspd = Vector3.zero;
        isAttacking = false;
    }

    public IEnumerator timerAttack()
    {
        yield return new WaitForSeconds(selfParams.punchCooldown);
        isAttackInCooldown = false;
    }

    public void DecelerateAttack(float timeStamp)
    {
        if (attackspd != Vector3.zero)
        {
            attackspd = attackspd * selfParams.runningDeceleration.Evaluate(timeStamp);
        }
    }

    #endregion

    #region Punched
    //Propulse opposite direction with player velocity that punch(feature that could be cool)
    public void Propulse(Vector3 directedForce)
    {
        NoGravity();
        StopMovement();
        //hspd = new Vector3(directedForce.x, 0, directedForce.z);
        //vspd = new Vector3(0, directedForce.y, 0);
        //Debug.Log("Propulse");
        PropulseManager(directedForce);
    }

    public void PropulseManager(Vector3 directedForce)
    {
        hspd = new Vector3(directedForce.x, 0, directedForce.z);
        vspd = new Vector3(0, directedForce.y + selfParams.punchBaseUpwardPropulsionForce, 0);
    }

    public void StopPunch()
    {
        stopPunchFlag = true;
        //StartCoroutine(StopPunchManage());
    }
    /*
    private IEnumerator StopPunchManage()
    {
        float _time = 0;
        Vector3 attackspdbefore = attackspd;
        while (_time < selfParams.punchDecelerateHit[selfParams.punchDecelerateHit.length-1].time)
        {
            _time += Time.deltaTime;
            attackspd = selfParams.punchDecelerateHit.Evaluate(_time) * attackspdbefore;
            hspd = Vector3.zero;
            vspd = Vector3.zero;
            yield return new WaitForEndOfFrame();
        }
    }
    */
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
        selfLogic.isTouchingTheGround = true;
        canWallJump = true;
    }

    public void isNotGrounded()
    {
        selfLogic.isTouchingTheGround = false;
    }

    public void takeFlag()
    {
        selfLogic.CmdGetFlag();
        selfLogic.CmdHideFlagInGame();
    }

    #endregion

    #region Getter/Setter
    public Vector3 GetVelocity()
    {
        return selfRbd.velocity;
    }
    #endregion
}
