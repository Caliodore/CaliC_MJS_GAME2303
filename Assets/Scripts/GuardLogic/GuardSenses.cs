using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GuardSenses : MonoBehaviour
{
    [SerializeField] Rigidbody playerRB;
    [SerializeField] LayerMask obscuringLayers;
    [SerializeField] Collider guardHearing;

    GuardMovement attachedMoveScript;

    public enum GuardStates
    { 
        Idle = 0,
        Patrolling = 1,
        Investigating = 2,
        Pursuit = 3
    }

    public GuardStates currentGuardState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGuardState = GuardStates.Patrolling;
        attachedMoveScript = gameObject.GetComponent<GuardMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StateUpdater(string inputState)
    {
        switch (inputState) 
        {
            case "Patrolling":
            { 
                currentGuardState = GuardStates.Patrolling;
                break;
            }
            case "Investigating":
            {
                currentGuardState = GuardStates.Investigating;
                break;
            }
            case "Pursuit":
            { 
                currentGuardState = GuardStates.Pursuit;
                break;
            }
        }
        Debug.Log($"Guard is now set to: {currentGuardState}");
    }

    public void OnHeard(Vector3 lastHeardLocation, bool guardHearingPlayer)
    {
        Vector3 suspectedPosition = lastHeardLocation;

        StateUpdater("Investigating");
        attachedMoveScript.PatrolUpdate(currentGuardState);

        if(guardHearingPlayer)
        {
            Debug.LogWarning("I HEAR YOU LITTLE ONE");
            attachedMoveScript.isCurrentlyHearingPlayer = false;
        }
        else if(!guardHearingPlayer)
        {
            Debug.LogWarning("I dont HEAR YOU LITTLE ONE");
            attachedMoveScript.isCurrentlyHearingPlayer = true;
        }
    }

}
