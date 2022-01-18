using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverviewMovementHandler : MonoBehaviour
{
    public List<Transform> overviewPosition;
    public float lerpSpeed;

    private int posIndex;
    private int lastPosIndex;
    private float currentlerpRatio;

    private void Start()
    {
        lastPosIndex = 0;
        posIndex = 1;
        transform.position = overviewPosition[0].position;
        transform.rotation = overviewPosition[0].rotation;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(overviewPosition[lastPosIndex].position, overviewPosition[posIndex].position, currentlerpRatio);
        transform.rotation = Quaternion.Lerp(overviewPosition[lastPosIndex].rotation, overviewPosition[posIndex].rotation, currentlerpRatio);

        currentlerpRatio += Time.deltaTime * lerpSpeed;
        if(currentlerpRatio > 1)
        {
            currentlerpRatio = 0;
            lastPosIndex = posIndex;
            posIndex++;
            if(posIndex >= overviewPosition.Count)
            {
                posIndex = 0;
            }
        }
    }
}
