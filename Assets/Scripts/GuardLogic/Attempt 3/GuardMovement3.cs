using System;
using System.Collections;
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
    public float moveSpeed, searchingDuration, guardToTargetDist, patrolPauseDuration;
    public int currentPatrolIndex;
    public Vector3 currentPatrolTarget, lastKnownPlayerPos, currentPlayerPos, currentGuardPos, currentTargetVector;

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
            currentTargetVector = new Vector3(guardNavAgent.destination.x, 0, guardNavAgent.destination.z);
            currentGuardPos = new Vector3(guardObj.transform.position.x, 0, guardObj.transform.position.z);
            guardToTargetDist = Mathf.Abs(Vector3.Distance(currentGuardPos, currentTargetVector));
            if(guardToTargetDist < 1)
            {
                travellingToDestination = false;
                withinRange = true;    
            }
            else
            {
                travellingToDestination = true;
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
        currentPatrolTarget = patrolPoints[currentPatrolIndex].transform.position;
        switch(changingState)
        { 
            case GuardState.ResumePatrol:
                isWaiting = false;
                TargetUpdate(currentPatrolTarget);
                break; 
 
            case GuardState.ActivePatrol:
                isWaiting = false;
                currentPatrolIndex++;
                Debug.Log($"Current patrol index: {currentPatrolIndex}.");
                if(currentPatrolIndex >= patrolPoints.Length)
                    currentPatrolIndex = currentPatrolIndex % patrolPoints.Length;
                Debug.Log($"Tested if within index, and did remainder operation if so. \n Target Update sent patrol point at index: {currentPatrolIndex}.");
                currentPatrolTarget = patrolPoints[currentPatrolIndex].transform.position;
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
        elapsedTravelTime = 0;
        while(guardToTargetDist > 1f)
        { 
            travellingToDestination = true;
            elapsedTravelTime += Time.deltaTime;
            yield return null;
        }
        if(guardToTargetDist <= 1f)
        {
            travellingToDestination = false;
            if(((int)activeGuardState < 2) && isWaiting)
                ArrivalLogic();
        }
        yield return null;
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
                break;

            case(GuardState.Waiting):
            case(GuardState.Investigating):
            case(GuardState.Pursuing):
                Debug.Log("Oops! Unintended consequence!");
                break;
        }
    }
}
