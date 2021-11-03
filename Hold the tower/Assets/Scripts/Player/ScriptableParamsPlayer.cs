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
    public AnimationCurve hspdAcceleration;
    public AnimationCurve hspdDeceleration;
    public float hspdForce;

    [Header("Vspd")]
    public float gravity;

    public float forwardForceJump;
    public float topForceJump;
    [Range(1,100)] public int jumpNumberToApply;

    [Header("Climb")]
    public float climbHeight = 5f;
    public float climbWidth = 5f;
    public float climbPrecision = 5f;

    [Header("Attack")]
    public float forceAttack = 20f;

    [Range(1, 100)] public int forceAttackNumberToApply;
}
