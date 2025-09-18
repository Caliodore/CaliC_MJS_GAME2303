using System;
using System.Collections;
using UnityEngine;

public class GuardBrain_3 : MonoBehaviour
{
    [SerializeField] GameObject attachedGuardObject;
    [SerializeField] GameObject guardOverseer;
    [SerializeField] GameObject playerParentObj;

    [Header("Non-Player Trackers")]
    public bool isCurrentlyPatrolling, isCurrentlyWaiting;
    public int patrolPointIndex;
    public float distanceToTarget, elapsedWaitingTime, patrolPauseDuration, moveSpeed;
    public Transform[] patrolPointArray;

    [Header("Non-Pursuit Player Trackers")]
    public bool playerCurrentlyInHearingRadius, travellingToPlayerLastKnown, searchingAtLastKnown, isReturningToPatrol;
    public int playerIndexFlag;
    public float timeToSearchForPlayer, audioReactionTime;

    [Header("Pursuing Player Trackers")]
    public bool isSeeingPlayer, isPursuingPlayer, visionBlocked, playerInVisualRange;
    public float visualRange, visualReactionTime, timeSinceLastSeen, distanceToPlayer;

    [Header("Vars for Changes")]
    public bool stateChanged;
    public GuardState currentGuardState, desiredState;

    //Vars for internal references
    private GuardPresetValues guardPresets;
    private GuardMovement3 AS_Move;
    private GuardVision_3 AS_Vision;
    private GuardHearing_3 AS_Hearing;
    private GuardStateChange_3 AS_StateChange;
        //Coroutines
        private Coroutine reactionCoRo;



    void Start()
    {
        InitializePresetsAndComponents();
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
        guardPresets = guardOverseer.GetComponent<GuardPresetValues>();
        AssignPresetValues();
        //Making sure it's referencing the individual parent object of all aspects of the guard prefab for getting various components.
        AS_Move = attachedGuardObject.GetComponent<GuardMovement3>();
        AS_Hearing = attachedGuardObject.GetComponent<GuardHearing_3>();
        AS_Vision = attachedGuardObject.GetComponent<GuardVision_3>();
        AS_StateChange = attachedGuardObject.GetComponent<GuardStateChange_3>();
        AssignScriptPresets();
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
        AS_StateChange.InitializeStateValues(currentGuardState);
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
    /// Method called by non-StateChange scripts to check with Brain if it is time to swap states.
    /// </summary>
    public void OnRequestStateUpdate()
    { 
        
    }

    public void OnPlayerEntersAudioRange()  //Should be activated when player collides/is colliding with the hearing radius trigger, but not when actively being seen by the guard.
    {
        reactionCoRo = StartCoroutine(GuardReactsToPlayer(audioReactionTime, GuardState.Investigating));
    }

    public void OnPlayerEntersVisualRange() //Should overwrite the audio range alert functions and investigating state if player can be seen.
    { 
        reactionCoRo = StartCoroutine(GuardReactsToPlayer(visualReactionTime, GuardState.Pursuing));
    }

    IEnumerator GuardReactsToPlayer(float reactionType, GuardState stateToChangeTo)
    {
        Debug.Log("GuardBrain begins reaction.");
        yield return new WaitForSeconds(reactionType);
        Debug.Log($"GuardBrain tells StateChange to change to: {stateToChangeTo}");
        AS_StateChange.ChangeState(stateToChangeTo);
        StopCoroutine(reactionCoRo);
    }

    private void ChangeScriptStates(GuardState changingState)
    { 
        AS_Hearing.HearingChangeState(changingState);
        AS_Move.MovementChangeState(changingState);
        AS_Vision.VisionChangeState(changingState);
    }
}
