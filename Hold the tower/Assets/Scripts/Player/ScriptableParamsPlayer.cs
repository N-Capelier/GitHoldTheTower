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

    [Header("Attack")]
    public AnimationCurve velocityCurve;
    public float forceAttack = 20f;
    public float cooldownAttack = 0.5f; //en seconde
    public float timePerfectAttack = 0.8f;
    public float timeTreshold = 0.15f;
    [Range(0f,1f)] public float slowMovementRatio = 0.5f;

}
