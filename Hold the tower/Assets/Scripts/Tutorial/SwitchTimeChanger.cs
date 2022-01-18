using UnityEngine;

public class SwitchTimeChanger : MonoBehaviour
{
    [SerializeField] LevelTransition levelTransition;
    [SerializeField] double newTime;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            levelTransition.timerChange = newTime;
        }
    }
}
