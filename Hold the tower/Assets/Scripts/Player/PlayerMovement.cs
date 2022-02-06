using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private ScriptableParamsPlayer selfParams;
    [SerializeField]
    private PlayerLogic selfLogic;
    [SerializeField]
    public Rigidbody selfRbd;
    [SerializeField]
    private Transform selfCamera;
    [SerializeField]
    public GameObject selfAttackCollider;
    public FPVAnimatorManager FPVAnimator;

    private Vector3 moveDirection = new Vector3(0, 0, 0);

    //private Vector3 hspd;
    //private Vector3 vspd;
    //private Vector3 attackspd = new Vector3();

    public Vector3 groundCorrection;


    private bool leftCollide, rightCollide, backCollide, frontTopCollide, frontBotCollide;

    private bool canWallJump = true;
    private bool stopPunchFlag;

    [HideInInspector] public bool isPerfectTiming = false;
    [HideInInspector] public bool isPunchInstantDestroy = false;

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

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    Coroutine walkTransitionCoroutine;

    [HideInInspector] public float punchRatio;
    [HideInInspector] public float punchAngle;

    [SerializeField] Animator characterAnimator;

    bool isMoving = false;

    private void FixedUpdate()
    {
        //if (!isClimbingMovement)
        //{
        //    selfRbd.velocity += hspd + vspd + attackspd;
        //}

        if (isMoving && GetHorizontalVelocity().magnitude < 0.02f)
        {
            isMoving = false;
            if(walkTransitionCoroutine != null)
                StopCoroutine(walkTransitionCoroutine);
            walkTransitionCoroutine = StartCoroutine(WalkTransition());
        }
        else if (!isMoving && GetHorizontalVelocity().magnitude > 0.02f)
        {
            isMoving = true;
            if (walkTransitionCoroutine != null)
                StopCoroutine(walkTransitionCoroutine);
            walkTransitionCoroutine = StartCoroutine(WalkTransition());
        }
    }

    IEnumerator WalkTransition()
	{
        float _elapsedTime = 0f, _completion = 0f;

        while(_completion < .5f)
		{
            //Debug.LogWarning(_completion);
            _elapsedTime += Time.deltaTime;
            _completion = _elapsedTime * 2f;
            if (isMoving)
                characterAnimator.SetFloat("CharacterSpeed", Mathf.Lerp(0f, 2f, _completion));
            else
                characterAnimator.SetFloat("CharacterSpeed", Mathf.Lerp(2f, 0f, 1f -_completion));
            yield return waitForEndOfFrame;
        }
        yield return waitForEndOfFrame;
	}

	private void Update()
	{
        if (characterAnimator.gameObject.activeSelf)
        {
            characterAnimator.SetFloat("CharacterSpeed", GetHorizontalVelocity().magnitude);
        }

        if(selfLogic.hasAuthority)
        {
            SetSmoothCameraTilt();
        }
    }


	#region HorizontalPhysics
	public void Move(Vector3 direction, float timeStamp)
    {
        moveDirection = direction;

        selfRbd.velocity += direction.normalized * selfParams.runningForce * Time.deltaTime;

        if (GetHorizontalVelocity().magnitude > selfParams.maxRunningSpeed * (isChargingPunch ? selfParams.chargeSlowMovementRatio : 1) * moveDirection.magnitude)
        {
            SetHorizontalVelocity(selfRbd.velocity.normalized * selfParams.maxRunningSpeed * (isChargingPunch ? selfParams.chargeSlowMovementRatio : 1) * moveDirection.magnitude);
        }
    }

    public void AirMove(Vector3 direction)
    {
        moveDirection = direction;

        float hSpeed = GetHorizontalVelocity().magnitude;

        selfRbd.velocity += direction * selfParams.airControlForce * Time.deltaTime;

        if(hSpeed > selfParams.maxRunningSpeed * (isChargingPunch ? selfParams.chargeSlowMovementRatio : 1))
        {
            SetHorizontalVelocity(selfLogic.GetHorizontalVector(selfRbd.velocity) * hSpeed);
        }
        else
        {
            if (GetHorizontalVelocity().magnitude > selfParams.maxRunningSpeed * (isChargingPunch ? selfParams.chargeSlowMovementRatio : 1))
            {
                SetHorizontalVelocity(selfLogic.GetHorizontalVector(selfRbd.velocity) * selfParams.maxRunningSpeed * (isChargingPunch ? selfParams.chargeSlowMovementRatio : 1));
            }
        }
    }

    public void Decelerate(float timeStamp)
    {
        if(GetHorizontalVelocity().magnitude > selfParams.minSpeedToStop * selfParams.groundFriction)
        {
            //Debug.LogError("Velocity : " + GetHorizontalVelocity().magnitude);
            selfRbd.velocity -= selfLogic.GetHorizontalVector(selfRbd.velocity).normalized * selfParams.groundFriction * Time.deltaTime;
        }
        else
        {
            SetHorizontalVelocity(Vector3.zero);
            moveDirection = Vector3.zero;
        }
    }

    public void ResetVelocity()
    {
        selfRbd.velocity = Vector3.zero;
    }

    public void SetHorizontalVelocity(Vector3 hVel)
    {
        selfRbd.velocity = new Vector3(hVel.x, selfRbd.velocity.y, hVel.z);
    }
    public Vector3 GetHorizontalVelocity()
    {
        return new Vector3(selfRbd.velocity.x, 0, selfRbd.velocity.z);
    }
    #endregion

    #region VerticalPhysics

    public void ApplyGravity()
    {
        //vspd += new Vector3(0, -selfParams.gravity, 0) * Time.deltaTime;
        selfRbd.velocity -= new Vector3(0, selfParams.gravityForce, 0) * Time.deltaTime;
    }

    [HideInInspector] public Vector3 wallSlideDirection;
    [HideInInspector] public Vector3 wallSlideNormal;

    public bool SetWallSlideDirection()
    {
        wallSlideNormal = selfLogic.GetNearbyWallNormal();

        if(wallSlideNormal != Vector3.zero)
        {
            float wallAngle = Vector3.SignedAngle(Vector3.right, wallSlideNormal, Vector3.up);
            //float lookAngle = Vector3.SignedAngle(Vector3.right, selfLogic.GetHorizontalVector(selfCamera.forward).normalized, Vector3.up);
            float lookAngle = Vector3.SignedAngle(Vector3.right, selfLogic.GetHorizontalVector(selfRbd.velocity).normalized, Vector3.up);
            if (Mathf.Abs(GetClampedAngle(lookAngle - GetClampedAngle(wallAngle + 90))) < 90)
            {
                wallSlideDirection = new Vector3(Mathf.Cos(GetClampedAngle(wallAngle + 90) * Mathf.Deg2Rad), 0, -Mathf.Sin(GetClampedAngle(wallAngle + 90) * Mathf.Deg2Rad));
            }
            else
            {
                wallSlideDirection = new Vector3(Mathf.Cos(GetClampedAngle(wallAngle - 90) * Mathf.Deg2Rad), 0, -Mathf.Sin(GetClampedAngle(wallAngle - 90) * Mathf.Deg2Rad));
            }


            //Debug.DrawRay(transform.position, wallSlideDirection * 3, Color.green, 4);
            //Debug.DrawRay(transform.position, Vector3.up * 0.3f, Color.red, 4);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ApplyWallSlideForces()
    {
        Vector3 wallRideVelocity = wallSlideDirection * selfParams.wallRideSpeed;
        if (selfRbd.velocity.y < -selfParams.wallSlideMaxGravitySpeed)
        {
            selfRbd.velocity = new Vector3(wallRideVelocity.x, -selfParams.wallSlideMaxGravitySpeed, wallRideVelocity.z);
        }
        else
        {
            selfRbd.velocity = new Vector3(wallRideVelocity.x, selfRbd.velocity.y - selfParams.wallSlideGravity * Time.deltaTime, wallRideVelocity.z);
        }
    }
    float cameraTargetTilt;
    public void UpdateWallSlideCameraTilt()
    {
        float wallAngle = Vector3.SignedAngle(Vector3.right, wallSlideNormal, Vector3.up);
        //float lookAngle = Vector3.SignedAngle(Vector3.right, selfLogic.GetHorizontalVector(selfCamera.forward).normalized, Vector3.up);
        float lookAngle = Vector3.SignedAngle(Vector3.right, selfLogic.GetHorizontalVector(selfRbd.velocity).normalized, Vector3.up);
        float angleDist = 0;
        angleDist = wallAngle - lookAngle;
        if (angleDist > 180)
        {
            angleDist -= 360;
        }
        else if (angleDist < -180)
        {
            angleDist += 360;
        }
        cameraTargetTilt = Mathf.Lerp(selfParams.cameraTiltAngle, -selfParams.cameraTiltAngle, (angleDist + 90) / 180);
    }

    public void SetSmoothCameraTilt()
    {
        selfCamera.rotation = Quaternion.Lerp(selfCamera.rotation, Quaternion.Euler(selfCamera.rotation.eulerAngles.x, selfCamera.rotation.eulerAngles.y, cameraTargetTilt), selfParams.cameraTiltLerpSpeed * Time.deltaTime);
    }

    public void ResetCameraTilt()
    {
        cameraTargetTilt = 0;
    }

    public void ApplyWallAttachForces()
    {
        selfRbd.velocity -= new Vector3(0, selfParams.gravityForce, 0) * Time.deltaTime;
        
        /*selfRbd.velocity -= new Vector3(0, selfParams.wallSlideGravity, 0) * Time.deltaTime;

        if (selfRbd.velocity.y < -selfParams.wallSlideMaxGravitySpeed)
        {
            selfRbd.velocity = new Vector3(0, -selfParams.wallSlideMaxGravitySpeed, 0);
        }
        float verticalVelocity = selfRbd.velocity.y;
        selfRbd.velocity -= selfRbd.velocity * selfParams.wallSlideSpeedDampening * Time.deltaTime;
        selfRbd.velocity = new Vector3(selfRbd.velocity.x, verticalVelocity, selfRbd.velocity.z);*/
    }

    public void ResetVerticalVelocity()
    {
        selfRbd.velocity = new Vector3(selfRbd.velocity.x, 0, selfRbd.velocity.z);
    }

    public void Jump()
    {
        ResetVerticalVelocity();
        selfRbd.velocity += Vector3.up * selfParams.jumpForce;
        if (Input.GetKey(selfParams.front))
        {
            selfRbd.velocity += selfLogic.GetHorizontalVector(selfCamera.forward) * selfParams.jumpForwardForce * Time.fixedDeltaTime;
        }
        FPVAnimator.AnimateJump();
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
        bool cancelJump = false;
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
                cancelJump = true;
            }
            else if(Mathf.Abs(angleDist) < selfParams.wallJumpMaxAngleToCancelDeviation)
            {
                cancelJump = true;
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

        adjustDirection += new Vector3(0, selfParams.wallJumpUpwardForce, 0);

        if(!cancelJump)
        {
            selfLogic.CmdPlayerSource("PlayerJump");
            ResetVelocity();
            ResetVerticalVelocity();
            selfRbd.velocity += adjustDirection * selfParams.wallJumpNormalForce;
        }
        else
        {

        }

        canWallJump = true;
        /*
        hspd = new Vector3(WallJumpspd.x, 0, WallJumpspd.z);
        vspd = new Vector3(0, WallJumpspd.y, 0);*/
        yield return new WaitForEndOfFrame();
    }


    public float GetClampedAngle(float angle)
    {
        float newAngle = angle;
        if (newAngle > 180)
        {
            newAngle -= 360;
        }
        else if (newAngle < -180)
        {
            newAngle += 360;
        }
        return newAngle;
    }
    #endregion

    #region Climb
    public bool CanClimb()
    {
        if (frontBotCollide && !frontTopCollide && !isClimbingMovement)
        {
            Climb();
            return true;
        }
        return false;
    }
    public bool CanPlayerClimb()
    {
        if (frontBotCollide && !frontTopCollide && !isClimbingMovement)
        {
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
            ResetVerticalVelocity();
            ResetVelocity();
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
        moveDirection = selfLogic.GetHorizontalVector(selfRbd.velocity);
        selfLogic.CmdSwitchCollider(false);
        isClimbingMovement = false;
    }

    #endregion

    #region WallDetection

    private bool IsFrontCollide()
    {
        if (frontTopCollide/* && frontBotCollide*/)
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

    [HideInInspector] public bool isChargingPunch;
    public float AttackLoad(float time)
    {
        float ratio = 0;
        punchAngle = Vector3.Angle(Vector3.up, selfCamera.forward);
        //Slow player
        //if(selfLogic.isGrounded)
        //{
        //    attackspd = hspd * -1;
        //    attackspd *= selfParams.chargeSlowMovementRatio;
        //}

        //Si est en dessous du pickTime
        if (time <= selfParams.punchMaxChargeTime)
        {
            //isPerfectTiming = false;
            isPunchInstantDestroy = false;
             //ratio = selfParams.punchSpeedByCharge.Evaluate(time/selfParams.punchMaxChargeTime);
             //ratio = time / (selfParams.punchPerfectTiming + selfParams.punchPerfectTimingTreshold);
             ratio = time / selfParams.punchMaxChargeTime;
        }

        /*
        //Si est au pickTime
        if(time <= selfParams.punchPerfectTiming + selfParams.punchPerfectTimingTreshold && time >= selfParams.punchPerfectTiming)
        {
            isPerfectTiming = true;
            //ratio = selfParams.punchPerfectTimingPropulsionMultiplier;
            ratio = time / (selfParams.punchPerfectTiming + selfParams.punchPerfectTimingTreshold);
        }*/

        //Si sup�rieur au pickTime + treshHold
        if(time > selfParams.punchMaxChargeTime && time <= selfParams.punchMaxChargeTime + selfParams.punchChargeTimeToInstantDestroy)
        {
            isPerfectTiming = false;
            isPunchInstantDestroy = false;
            ratio = 1;
        }

        if(time > selfParams.punchMaxChargeTime + selfParams.punchChargeTimeToInstantDestroy)
        {
            isPunchInstantDestroy = true;
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
        StartCoroutine(TimerAttack());
        FPVAnimator.AnimatePunch(true);
        selfAttackCollider.SetActive(true);
        //StopMovement(); ////////////////////////////////////////////////////////////////////////////// Check merge conflict ///////////////////////////////////////////////////
        selfLogic.CmdAttackCollider(true);

        directionAttack = selfCamera.forward;
        isAttacking = true;
        isAttackReset = false;
        float finalBaseSpeed = selfParams.punchBaseSpeed * selfParams.punchSpeedByCharge.Evaluate(ratio) * GetPunchAngleRatio(punchAngle);
        if(selfLogic.hasFlag)
        {
            //finalBaseSpeed = selfParams.punchBaseSpeed * Mathf.Clamp(selfParams.punchSpeedByCharge.Evaluate(0), 0, GetPunchAngleRatio(punchAngle));
        }
        punchRatio = ratio;

        //Vector3 startPos = transform.position; pour calculer la distance du punch

        float _time = 0;
        while(_time <selfParams.velocityCurve[selfParams.velocityCurve.length - 1].time && !stopPunchFlag)
        {
            selfRbd.velocity = directionAttack * selfParams.velocityCurve.Evaluate(_time) * Time.fixedDeltaTime * finalBaseSpeed;
            _time += Time.deltaTime;
            //ResetVerticalVelocity();
            yield return waitForFixedUpdate;
        }
        stopPunchFlag = false;
        //float attackTimeStamp = Time.time;


        isAttackInCooldown = true;
        selfAttackCollider.SetActive(false);
        selfLogic.CmdAttackCollider(false);

        //active Wall Jump if player punch
        canWallJump = true;
        selfRbd.velocity = directionAttack * selfParams.velocityCurve.Evaluate(selfParams.velocityCurve[selfParams.velocityCurve.length - 1].time) * Time.fixedDeltaTime * finalBaseSpeed;
        isAttacking = false;
        FPVAnimator.AnimatePunch(false);
        isPerfectTiming = false;
        //startPos = transform.position - startPos;
        //Debug.Log("Punch Distance : " + startPos.magnitude);
        yield return null;
    }

    public IEnumerator TimerAttack()
    {
        float timerPunchCD = 0;
        bool soundOnce = false;
        while(timerPunchCD < (selfLogic.hasFlag ? selfParams.punchCooldownWithOverdrive : selfParams.punchCooldown))
        {
            timerPunchCD += Time.deltaTime;
            selfLogic.UpdatePunchCooldown(timerPunchCD);
            if (timerPunchCD >= (selfLogic.hasFlag ? selfParams.punchCooldownWithOverdrive : selfParams.punchCooldown) - 0.15f && soundOnce == false)
            {
                SoundManager.Instance.PlaySoundEvent("PlayerPunchAvailable");
                soundOnce = true;
            }
            yield return new WaitForEndOfFrame();
        }        
        isAttackInCooldown = false;
    }

    public float GetPunchAngleRatio(float punchVerticalAngle)
    {
        return selfParams.punchPropulsionForceByAngle.Evaluate(punchVerticalAngle / 180);
    }

    #endregion

    #region Punched
    //Propulse opposite direction with player velocity that punch(feature that could be cool)
    public void Propulse(Vector3 directedForce)
    {
        ResetVerticalVelocity();
        ResetVelocity();
        //hspd = new Vector3(directedForce.x, 0, directedForce.z);
        //vspd = new Vector3(0, directedForce.y, 0);
        //Debug.Log("Propulse");
        PropulseManager(directedForce);
        SoundManager.Instance.PlaySoundEvent("PlayerPunched");
        
    }

    public void PropulseManager(Vector3 directedForce)
    {
        selfRbd.velocity += directedForce * selfParams.punchPropulsionForceByCharge.Evaluate(punchRatio);
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
        SoundManager.Instance.PlaySoundEvent("PlayerOverdriveTaken");
    }

    #endregion

    #region Getter/Setter
    public Vector3 GetVelocity()
    {
        return selfRbd.velocity;
    }
    #endregion
}
