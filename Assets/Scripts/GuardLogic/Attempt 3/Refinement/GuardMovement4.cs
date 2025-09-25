using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement4 : MonoBehaviour
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
    public bool withinRange, htdRunning, updatingPlayerLastKnown, doneInitializing = false, isWaiting, headingToPatrolPoint, arrivedToLastKnown, patrolInterrupted, playerSensed;

    public GuardState activeGuardState;
    public GuardState previousGuardState;
    private float visualReactTime, audioReactTime, reactionTime, elapsedTravelTime;
    private int activeStateInt;
    private int previousStateInt, resumeInt, waitingInt, activeInt, investInt, pursueInt;
    private bool playerSneaking, playerInRange, targetOverwrite, movementInterrupted, playerHeard, playerSeen, overwriteNormalPatrol;
    private Coroutine patrolCoRo, playerCoRo;
    Transform guardTransform;
    GuardState resumeState, waitingState, activeState, investState, pursueState;

    public void Awake()
    {
        resumeState = GuardState.ResumePatrol;
        waitingState = GuardState.Waiting;
        activeState = GuardState.ActivePatrol;
        investState = GuardState.Investigating;
        pursueState = GuardState.Pursuing;

        resumeInt = (int)GuardState.ResumePatrol;
        waitingInt = (int)GuardState.Waiting;
        activeInt = (int)GuardState.ActivePatrol;
        investInt = (int)GuardState.Investigating;
        pursueInt = (int)GuardState.Pursuing;
    }

    private void FixedUpdate()
    {
        if(doneInitializing)
        {
            currentGuardPos = guardTransform.position;
            guardToTargetDist = Vector3.Distance(currentGuardPos, targetPos);
            if(guardToTargetDist <= 1)
            {
                withinRange = true;
                if(overwriteNormalPatrol)
                { 
                    arrivedToLastKnown = true;    
                }
            }
            else
            { 
                withinRange = false;
                arrivedToLastKnown = false;
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
        guardPos = attachedBrain.transform.position;
        patrolPoints = attachedBrain.patrolPointArray;
        targetPos = patrolPoints[currentPatrolIndex].transform.position;
        
        guardTransform = guardObj.transform;
    }

    public void MovementChangeState(GuardState changingState)
    { 
        previousGuardState = activeGuardState;

        int stateInt = (int)changingState;

        if (stateInt != 1) isWaiting = false;
        else isWaiting = true;

        switch (stateInt)
        { 
            case(0):    //ResumePatrol
            { 
                playerHeard = false;
                playerSeen = false;
                playerSensed = false;
                overwriteNormalPatrol = false;
                patrolInterrupted = false;
                targetPos = PatPos();
            }
                break; 
            case(1):    //Waiting
            {
                patrolInterrupted = false;
                //Target = currentPosition
                if(targetPos == patrolPoints[currentPatrolIndex].transform.position)
                { 
                    currentPatrolIndex++;
                    if(currentPatrolIndex >= patrolPoints.Length)
                    { 
                        currentPatrolIndex %= patrolPoints.Length;        
                    }
                }
                else if (targetPos == lastKnownPlayerPos)
                { 
                    Debug.Log("MoveScript is waiting during search period at player's last known.");
                }
                else
                { 
                    Debug.Log("MoveScript randomly got overwritten it seems. L");        
                }
            }
                break; 
            case(2):    //ActivePatrol
            { 
                patrolInterrupted = false;
                targetPos = PatPos();
            }
                break; 
            case(3):    //Investigating
            {
                patrolInterrupted = true;
                targetPos = PlayPos();
                reactionTime = audioReactTime;
            }
                break; 
            case(4):    //Pursuing
            {
                patrolInterrupted = true;
                targetPos = PlayPos();
                reactionTime = visualReactTime;
            }
                break;
        }
        activeGuardState = changingState;
        TargetUpdater();
    }

    private Vector3 PatPos()
    { 
        Debug.Log(patrolPoints[currentPatrolIndex].transform.position);
        return patrolPoints[currentPatrolIndex].transform.position;    
    }

    private Vector3 PlayPos()
    { 
        Debug.Log(lastKnownPlayerPos);
        return lastKnownPlayerPos;    
    }

    private void TargetUpdater()
    {
        if(activeGuardState == waitingState)
        { 
            return;
        }
        else
        {
            guardNavAgent.SetDestination(targetPos);  
            guardNavAgent.speed = moveSpeed;
        }
        
        if(!overwriteNormalPatrol)
        { 
            patrolCoRo = StartCoroutine(CurrentPatrolTracker());
        }
        else if(overwriteNormalPatrol)
        { 
            playerCoRo = StartCoroutine(InterruptPatrol());
        }
    }

    IEnumerator CurrentPatrolTracker()
    { 
        headingToPatrolPoint = true;

        while(headingToPatrolPoint) //while uninterrupted and otw to patrol point
        {
            if(overwriteNormalPatrol || withinRange)    //patrol path can be interrupted by arriving at location or by the global overwrite
            {
                headingToPatrolPoint = false;
                guardNavAgent.speed = 0;
                CallForStateChange(waitingState);
                yield break;
            }
            yield return null;
        }
        yield break;
    }

    IEnumerator InterruptPatrol()
    {
        while(!arrivedToLastKnown)
        {
            //Set target vector and reaction time to whichever was noticed.
            if(playerSensed)
            {
                //Update destination to player location until losing them.
                lastKnownPlayerPos = currentPlayerPos;
                guardNavAgent.SetDestination(targetPos);
            }
            yield return new WaitForSeconds(reactionTime);
        }
        if(arrivedToLastKnown)
        { 
            //Request to change state to Waiting.
            yield return new WaitForSeconds(searchingDuration);
            CallForStateChange(resumeState);
        }
        yield break;    
    }

    public void OnPlayerHearingUpdate(bool brainInput)
    { 
        playerHeard = brainInput;
        CallForInterrupt();
    }

    public void OnPlayerVisionUpdate(bool brainInput)
    { 
        playerSeen = brainInput;
        CallForInterrupt();
    }

    private void CallForInterrupt()
    { 
        if((playerHeard || playerSeen) || (playerHeard && playerSeen))
        {
            playerSensed = true;
            overwriteNormalPatrol = true;    
        }
        else
        {
            playerSensed = false;
            overwriteNormalPatrol = false;    
        }
    }

    private void CallForStateChange(GuardState requestedState)
    { 
        attachedBrain.OnRequestStateUpdate(requestedState);    
    }
}
