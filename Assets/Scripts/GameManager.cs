using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject guardPrefab;
    public bool guardHearingPlayer = false;

    private bool sawPlayer = false;
    private bool heardPlayer = false;
    private bool arrivedAtLastKnown = false;
    private Vector3 playerLastKnownLocation;
    private Vector3 playerCurrentPosition;

    private void Update()
    {
        playerCurrentPosition = playerObj.transform.position;
    }

    public void GuardHeardPlayer(GameObject alertedGuard)
    {
        Debug.LogWarning("Player was heard.");
        guardHearingPlayer = true;
        GuardTrackingPlayer(alertedGuard);
    }

    public void GuardCantHearPlayer(GameObject alertedGuard)
    {
        Debug.LogWarning("Player was no longer heard.");
        guardHearingPlayer = false;    
        GuardTrackingPlayer(alertedGuard);
    }

    private void GuardTrackingPlayer(GameObject alertedGuard)
    {
        if(guardHearingPlayer == true)
        {
            Debug.Log("Run OnHeard with true");
            alertedGuard.GetComponent<GuardSenses>().OnHeard(playerCurrentPosition, guardHearingPlayer);
        }
        else
        { 
            Debug.Log("Run OnHeard with false");
            alertedGuard.GetComponent<GuardSenses>().OnHeard(playerLastKnownLocation, guardHearingPlayer);        
        }
    }

}
