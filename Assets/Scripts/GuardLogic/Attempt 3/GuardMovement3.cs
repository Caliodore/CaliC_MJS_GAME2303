using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement3 : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] Transform patrolMarkerPrefab;
    [SerializeField] Transform[] patrolPoints;
    GameObject guardObj, playerObj;
    NavMeshAgent guardNavAgent;
    GuardBrain_3 attachedBrain;
    GuardPlayerTrackerCoRo attachedCoRoScript;

    [Header("Movement Values")]
    public float moveSpeed, searchingDuration, guardToTargetDist, patrolPauseDuration, distanceNormalized;
    public int currentPatrolIndex;
    public Vector3 currentPatrolTarget, lastKnownPlayerPos, currentPlayerPos, currentGuardPos, currentTargetVector, guardPos, targetPos;

    [Header("Bools")]
    public bool withinRange, travellingToDestination, updatingPlayerLastKnown, doneInitializing = false, isWaiting;

    public GuardState activeGuardState;
    public GuardState previousGuardState;
    private float visualReactTime, audioReactTime, reactionTime, elapsedTravelTime;
    private int activeStateInt;
    private int previousStateInt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPatrolIndex = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(doneInitializing)
        {
            guardPos = attachedBrain.transform.position;
            targetPos = patrolPoints[currentPatrolIndex].transform.position;

            guardToTargetDist = Vector3.Distance(guardPos, targetPos);
            if(guardToTargetDist < 1)
            {
                withinRange = true;    
            }
            else
            {
                withinRange = false;    
            }
        }
        
    }

    public void InitializeMoveValues(float ms, float sd, float ppd)
    {
        moveSpeed = ms;
        searchingDuration = sd;
        patrolPauseDuration = ppd;
        doneInitializing = true;
    }

    public void AssignMoveRefs(GameObject guardObjRef, GameObject playerObjRef, NavMeshAgent guardNavAgentRef, GuardBrain_3 attachedBrainRef, GuardPlayerTrackerCoRo attachedCoRo)
    { 
        guardObj = guardObjRef;
        playerObj = playerObjRef;
        guardNavAgent = guardNavAgentRef;
        attachedBrain = attachedBrainRef;
        attachedCoRoScript = attachedCoRo;

        visualReactTime = attachedBrain.visualReactionTime;
        audioReactTime = attachedBrain.audioReactionTime;
        currentTargetVector = guardNavAgent.destination;
        currentGuardPos = guardObj.transform.position;
        guardToTargetDist = Mathf.Abs(Vector3.Distance(currentGuardPos, currentTargetVector));
    }

    public void MovementChangeState(GuardState changingState)
    {
        Debug.Log($"MoveScript States Pre-Update| P: {previousGuardState} | A: {activeGuardState} | N: {changingState}.");

        previousGuardState = activeGuardState;
        activeGuardState = changingState;

        Debug.Log($"MoveScript States Post-Update| P: {previousGuardState} | A: {activeGuardState} | N: {changingState}.");

        if(previousGuardState == GuardState.Waiting && changingState == GuardState.ActivePatrol)
        {
            Debug.Log("Called index iterator to get updated index for next patrol point.");
            WaitingToAP();    
        }

        currentPatrolTarget = patrolPoints[currentPatrolIndex].transform.position;

        switch(changingState)
        { 
            case GuardState.ResumePatrol:
                isWaiting = false;
                TargetUpdate(currentPatrolTarget);
                break; 
 
            case GuardState.ActivePatrol:
                isWaiting = false;
                Debug.Log($"Current patrol index: {currentPatrolIndex}.");
                TargetUpdate(currentPatrolTarget);
                break;

            case GuardState.Waiting:
                isWaiting = true;
                //Nothing. StateChange or Brain should handle the waiting timer.
                break; 

            case GuardState.Investigating:
            case GuardState.Pursuing:
                isWaiting = false;
                StartCoroutine(PlayerTargetUpdates());
                break;
    
        }
    }
    
    public void TargetUpdate(Vector3 destination)
    {
        Debug.Log($"TargetUpdate called");
        guardNavAgent.SetDestination(destination);
        if(!travellingToDestination)
            StartCoroutine(HeadingToDestination());
    }

    IEnumerator PlayerTargetUpdates()
    {
        while(activeGuardState == GuardState.Investigating)
        { 
            lastKnownPlayerPos = attachedCoRoScript.SendPlayerPosition();
            TargetUpdate(lastKnownPlayerPos);
            yield return new WaitForSeconds(audioReactTime);
        }
        while(activeGuardState == GuardState.Pursuing)
        { 
            lastKnownPlayerPos = attachedCoRoScript.SendPlayerPosition();
            TargetUpdate(lastKnownPlayerPos);
            yield return new WaitForSeconds(visualReactTime);
        }
        yield return null;    
    }

    IEnumerator HeadingToDestination()
    {
        Debug.Log($"HTD called");
        elapsedTravelTime = 0;
        while((guardToTargetDist > 1f) || (!withinRange))
        { 
            travellingToDestination = true;
            elapsedTravelTime += Time.deltaTime;
            yield return null;
        }
        if((guardToTargetDist <= 1f) || (withinRange))
        {
            travellingToDestination = false;
            if(((int)activeGuardState < 2) && !isWaiting)
                ArrivalLogic();
            yield return null;
        }
        yield break;
    }

    private void ArrivalLogic()
    {
        Debug.Log($"Arrived at location, asking Brain to change state from {activeGuardState} to Waiting.");
        switch(activeGuardState)
        { 
            case(GuardState.ResumePatrol):
            case(GuardState.ActivePatrol):
                Debug.LogWarning($"MovementLogic requests Brain to change to Waiting. \n Guard distance to target: {guardToTargetDist}");
                attachedBrain.OnRequestStateUpdate(GuardState.Waiting);
                currentPatrolIndex++;
                break;

            case(GuardState.Waiting):
            case(GuardState.Investigating):
            case(GuardState.Pursuing):
                Debug.Log("Oops! Unintended consequence!");
                break;
        }
    }

    private void WaitingToAP()
    {
        if(currentPatrolIndex >= patrolPoints.Length)
        { 
            currentPatrolIndex = currentPatrolIndex % patrolPoints.Length;
            Debug.Log($"Tested if within index, and did remainder operation if so. \n New patrol point at index: {currentPatrolIndex}.");
        }
    }
}
