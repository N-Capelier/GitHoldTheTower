using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_Effect : MonoBehaviour
{
    public float targetScaleMultiplier;
    public float movementTime;
    public AnimationCurve movementSpeed;

    private float timer;
    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(startScale, startScale * targetScaleMultiplier, movementSpeed.Evaluate(timer / movementTime));
        timer += Time.deltaTime;
        if(timer > movementTime)
        {
            Destroy(gameObject);
        }
    }
}
