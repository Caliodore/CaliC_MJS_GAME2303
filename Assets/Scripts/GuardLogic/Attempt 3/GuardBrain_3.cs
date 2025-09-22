using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardBrain_3 : MonoBehaviour
{
    [SerializeField] GameObject attachedGuardObject;
    [SerializeField] GameObject guardOverseer;
    [SerializeField] GameObject playerParentObj;
    [SerializeField] NavMeshAgent guardMoveAgent;

    [Header("Non-Player Trackers")]
    public bool isCurrentlyPatrolling, isCurrentlyWaiting;
    public int patrolPointIndex;
    public float distanceToTarget, elapsedWaitingTime, patrolPauseDuration, moveSpeed;
    public Transform[] patrolPointArray;

    [Header("Non-Pursuit Player Trackers")]
    public bool playerCurrentlyInHearingRadius, travellingToPlayerLastKnown, searchingAtLastKnown, isReturningToPatrol;
    public int playerIndexFlag;
    public float timeToSearchForPlayer, audioReactionTime;
    public LayerMask visionRaycastCollisionMask;

    [Header("Pursuing Player Trackers")]
    public bool isSeeingPlayer, isPursuingPlayer, visionBlocked, playerInVisualRange;
    public float visualRange, visualReactionTime, timeSinceLastSeen, distanceToPlayer, playerPosition;

    [Header("Vars for Changes")]
    public bool stateChanged, playerInRangeGeneral, doneInitializing;
    public GuardState currentGuardState, desiredState;

    //Vars for internal references
    private Collider hearingRadiusRef;
    private GuardBrain_3 attachedBrainRef;
    private GuardPresetValues guardPresets;
    private GuardMovement3 AS_Move;
    private GuardVision_3 AS_Vision;
    private GuardHearing_3 AS_Hearing;
    private GuardStateChange_3 AS_StateChange;
    private GuardPlayerTrackerCoRo attachedCoRo;
        //Coroutines
        private Coroutine reactionCoRo;



    void Start()
    {
        StartCoroutine(EstablishInternalReferences());
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(stateChanged)
        {
            stateChanged = false;
        }
    }

    /// <summary>
    /// Called at the start to make sure every component is properly referred to and all values are assigned the same for every guard while still being individualized.
    /// </summary>
    private void InitializePresetsAndComponents()
    {
        attachedGuardObject = GetComponent<GuardBrain_3>().gameObject;
        attachedCoRo = this.gameObject.GetComponentInChildren<GuardPlayerTrackerCoRo>();
        attachedBrainRef = attachedGuardObject.GetComponent<GuardBrain_3>();
        hearingRadiusRef = attachedGuardObject.GetComponentInChildren<GuardHearing_3>().gameObject.GetComponent<Collider>();
        visionRaycastCollisionMask = LayerMask.GetMask("Player","Wall","LoS Blocker");
        guardMoveAgent = attachedGuardObject.GetComponent<NavMeshAgent>();
        guardOverseer = FindAnyObjectByType<GuardOverseer>().gameObject;
        playerParentObj = FindAnyObjectByType<PlayerMovement>().gameObject;

        guardPresets = guardOverseer.GetComponent<GuardOverseer>().AssignGuardSO();

        //Making sure it's referencing the individual parent object of all aspects of the guard prefab for getting various components.
        AS_Move = attachedGuardObject.GetComponentInChildren<GuardMovement3>();
        AS_Hearing = attachedGuardObject.GetComponentInChildren<GuardHearing_3>();
        AS_Vision = attachedGuardObject.GetComponentInChildren<GuardVision_3>();
        AS_StateChange = attachedGuardObject.GetComponentInChildren<GuardStateChange_3>();

        //Debug.Log("Finished establishing internal references.");
    }

    IEnumerator EstablishInternalReferences()
    {
        //Debug.Log("Establishing Internal References");
        InitializePresetsAndComponents();
        yield return StartCoroutine(InitializingCoroutine());
        doneInitializing = true;
        yield break;
    }

    IEnumerator InitializingCoroutine()
    {
        //Debug.Log("Establishing secondary script preset values.");
        yield return StartCoroutine(EstablishPresetValues());
        //Debug.Log("Establishing secondary script references.");
        yield return StartCoroutine(EstablishSecondReferences());
        AssignScriptPresets();
        Debug.Log("All coroutines finished.");
        yield return null;    
    }

    IEnumerator EstablishSecondReferences()
    {
        AssignReferences();
        //Debug.Log("Last coroutine finished.");
        yield return null;
    }

    IEnumerator EstablishPresetValues()
    { 
        AssignPresetValues();
        //Debug.Log("Second coroutine finished.");
        yield return null;
    }

    /// <summary>
    /// Assigns the most necessary references for the other scripts to guarantee uniformity across all instances of this prefab.
    /// </summary>
    private void AssignReferences()
    { 
        AS_Move.AssignMoveRefs(attachedGuardObject, playerParentObj, guardMoveAgent, attachedBrainRef, attachedCoRo);
        AS_Hearing.AssignHearingRefs(attachedGuardObject, playerParentObj, hearingRadiusRef, attachedBrainRef);
        AS_Vision.AssignVisionRefs(attachedGuardObject, guardOverseer, playerParentObj, visionRaycastCollisionMask, attachedBrainRef);
        AS_StateChange.AssignStateRefs(attachedGuardObject, attachedBrainRef);
        //Debug.Log("Finished assigning component references.");
    }

    /// <summary>
    /// Assigns values to all variables according to the scriptable object of preset values.
    /// If not inside of scrip obj then manually assigned here for uniformity and ease of changing.
    /// </summary>
    private void AssignPresetValues()
    {
        //Variables with defined presets that are to be uniform across guards.
        moveSpeed = guardPresets.movementSpeed;
        patrolPauseDuration = guardPresets.patrolPauseDuration;
        timeToSearchForPlayer = guardPresets.searchingDuration;
        audioReactionTime = guardPresets.audioReactionTime;
        visualReactionTime = guardPresets.visualReactionTime;
        visualRange = guardPresets.visionRange;
        currentGuardState = GuardState.ResumePatrol;

        //Individualized variables that aren't necessarily just zeroed out from the beginning.
        patrolPointIndex = 0;
        playerIndexFlag = (patrolPointArray.Length + 1);
        isCurrentlyPatrolling = false;
        isCurrentlyWaiting = false;

        //Making sure individualized values/as needed values are zeroed out.
        distanceToTarget = 0f;
        elapsedWaitingTime = 0f;
        timeSinceLastSeen = 0f;
        distanceToPlayer = 0f;

        //Bools that are only used when interacting with the player, which should initialize as false;
        playerCurrentlyInHearingRadius = false;
        travellingToPlayerLastKnown = false;
        searchingAtLastKnown = false;
        isReturningToPatrol = false;
        isSeeingPlayer = false;
        isPursuingPlayer = false;
        visionBlocked = false;
        playerInVisualRange = false;
        //Debug.Log("Finished internalizing preset values.");
    }

    /// <summary>
    /// Calls the individualized methods per script to set their values according to the scrip obj preset.
    /// Placed AFTER the initialization confirming they are referencing the correct parent obj and children and their components.
    /// </summary>
    /// <param name="scriptValue"></param>
    private void AssignScriptPresets()
    {
        AS_Move.InitializeMoveValues(moveSpeed, timeToSearchForPlayer, patrolPauseDuration);
        AS_Vision.InitializeVisionValues(visualRange, visualReactionTime);
        AS_Hearing.InitializeHearingValues(audioReactionTime);
        AS_StateChange.InitializeStateValues(currentGuardState, patrolPauseDuration);
        //Debug.Log("Established secondary script presets.");
    }

    /// <summary>
    /// Mainly a public method to be called by the StateChange script to signal when its time to change states and in turn change all other component states.
    /// </summary>
    /// <param name="stateToChangeTo"></param>
    public void OnStateChange(GuardState stateToChangeTo)
    {
        currentGuardState = stateToChangeTo;
        ChangeScriptStates(currentGuardState);
        stateChanged = true;
    }

    /// <summary>
    /// Method called by non-StateChange scripts to communicate to Brain to change state through StateChange script.
    /// </summary>
    /// <param name="requestedStateChange"></param>
    public void OnRequestStateUpdate(GuardState requestedStateChange)
    { 
        Debug.Log($"Requesting StateChange to swap to: {requestedStateChange}.");
        AS_StateChange.ChangeState(requestedStateChange);
    }

    public void PlayerInRange()
    { 
        playerInRangeGeneral = true;
    }

    public void OnPlayerEntersAudioRange()  //Should be activated when player collides/is colliding with the hearing radius trigger, but not when actively being seen by the guard.
    {
        if(playerInRangeGeneral)
            reactionCoRo = StartCoroutine(GuardReactsToPlayer(audioReactionTime, GuardState.Investigating));
    }

    public void OnPlayerEntersVisualRange() //Should overwrite the audio range alert functions and investigating state if player can be seen.
    {
        if(playerInRangeGeneral)
            reactionCoRo = StartCoroutine(GuardReactsToPlayer(visualReactionTime, GuardState.Pursuing));
    }

    private void ChangeScriptStates(GuardState changingState)
    {
        Debug.Log($"Changing attached script states to {currentGuardState}.");
        AS_Hearing.HearingChangeState(changingState);
        AS_Move.MovementChangeState(changingState);
        AS_Vision.VisionChangeState(changingState);
    }

    IEnumerator GuardReactsToPlayer(float reactionType, GuardState stateToChangeTo)
    {
        Debug.Log("GuardBrain begins reaction.");
        yield return new WaitForSeconds(reactionType);
        //Need to add a call to vision raycast function here before submitting StateChange.
        Debug.Log($"GuardBrain tells StateChange to change to: {stateToChangeTo}");
        AS_StateChange.ChangeState(stateToChangeTo);
        yield break;
    }
}
