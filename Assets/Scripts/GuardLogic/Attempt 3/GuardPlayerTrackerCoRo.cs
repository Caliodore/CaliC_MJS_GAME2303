using System;
using System.Collections;
using UnityEngine;

public class GuardPlayerTrackerCoRo : MonoBehaviour
{
    GameObject playerObj;
    GameObject attachedGuardObj;

    GuardBrain_3 attachedBrain;
    [SerializeField] GuardPresetValues presetValues;

    Vector3 playerCurrentPos;

    float guardVisionRange;
    int guardInstanceID;
    bool seeingPlayer;

    private void Start()
    {
        playerObj = FindAnyObjectByType<PlayerMovement>().gameObject;
        guardVisionRange = presetValues.visionRange;
        //CreateCoroScript();
    }

    private void FixedUpdate()
    {
        playerCurrentPos = playerObj.GetComponentInChildren<Rigidbody>().transform.position;
    }

    public Vector3 SendPlayerPosition()
    { 
        return playerCurrentPos;
    }

    public void CreateCoroScript(int guardID)
    {
        attachedBrain = gameObject.GetComponentInParent<GuardBrain_3>();
        attachedGuardObj = attachedBrain.gameObject;
        guardInstanceID = guardID;
        if(attachedBrain.doneInitializing)
        {
            StartCoroutine(TrackPlayerRange(guardID));
        }
        else
            StartCoroutine(WaitingForInitialization());
    }

    IEnumerator WaitingForInitialization()
    {
        while(!attachedBrain.doneInitializing)
        { 
            yield return null;
        }
        CreateCoroScript(guardInstanceID);
        yield break;    
    }

    IEnumerator TrackPlayerRange(int guardNumber)
    {
        Debug.Log($"This coroutine is assigned to this guard number: {guardNumber}.");
        while(true)
        {
            Vector3 guardPosition = attachedGuardObj.GetComponentInChildren<Collider>().transform.position;
            Vector3 guardPlayerDistanceVector = playerCurrentPos - guardPosition;
            Debug.DrawLine(guardPosition, playerCurrentPos, Color.red);
            if(guardPlayerDistanceVector.magnitude <= guardVisionRange)
            {
                seeingPlayer = true;
                Debug.DrawLine(guardPosition, playerCurrentPos, Color.green);
            }
            else
            { 
                seeingPlayer = false;
            }
            attachedBrain.PlayerRangeUpdate(seeingPlayer);
            yield return new WaitForSeconds(attachedBrain.visualReactionTime);
        }
    }
}
