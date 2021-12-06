using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Player Params",menuName ="Player/Params")]
public class ScriptableParamsPlayer : ScriptableObject
{

    #region Key
    [Header("Input")]
    public KeyCode front = KeyCode.Z;
    public KeyCode left = KeyCode.Q;
    public KeyCode right = KeyCode.D;
    public KeyCode back = KeyCode.S;

    public KeyCode jump = KeyCode.Space;

    [Range(0, 2)] public int wallMouseInput;
    [Range(0, 2)] public int attackMouseInput;
    public float mouseSensivity;


    #endregion

    [Header("Hspd")]
    public AnimationCurve runningAcceleration;
    public AnimationCurve runningDeceleration;
    public float maxRunningSpeed;
    public float airControlForce;

    [Header("Vspd")]
    public float gravity;

    [Header("Jump and WallJump")]
    public float forwardForceJump;
    public float topForceJump;
    [Range(1,100)] public int jumpNumberToApply;
    public float forceToWallJump;
    public float upWardWallJumpForce;
    public float maxWallJumpAngleDeviation;
    public float wallJumpMinAngleToCancelDeviation;

    [Header("Climb")]
    public float climbHeight = 5f;
    public float climbWidth = 5f;
    public float timeToClimb = 0.5f;
    public AnimationCurve climbMovement;
    public AnimationCurve climbSpeedOverTime;

    [Header("Punch")]
    public AnimationCurve velocityCurve;
    public float punchBaseSpeed = 2600f;
    public AnimationCurve punchSpeedByCharge;
    public float punchPerfectTimingSpeedMultiplier;
    public float punchCooldown = 0.5f;
    public float punchPerfectTiming = 0.8f;
    public float punchPerfectTimingTreshold = 0.15f;
    public float punchMaxChargeTime = 1f;
    [Range(0f,1f)] public float chargeSlowMovementRatio = 0.5f;
    public float punchBasePropulsionForce = 5f;
    public float punchBaseUpwardPropulsionForce = 1f;
    public float punchedNoControlTime;
    public AnimationCurve punchPropulsionForceByCharge;

}
