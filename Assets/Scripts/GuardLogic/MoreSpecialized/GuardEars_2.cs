using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GuardEars : MonoBehaviour
{
    [SerializeField] Collider hearingRadius;
    [SerializeField] GameObject parentObject;
    
    public UnityEvent HeardPlayer;
    public UnityEvent LostPlayer;

    public bool currentlyHearingPlayerTrigger;
    
    public void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            currentlyHearingPlayerTrigger = true;
            HeardPlayer?.Invoke();
        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            currentlyHearingPlayerTrigger = false;
            LostPlayer?.Invoke();
        }
    }
}
