using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GuardPresetValues", menuName = "Scriptable Objects/GuardPresetValues")]
public class GuardPresetValues : ScriptableObject
{
    [Header("Physical Aspects")]
    public float visionRange;               //For how far the raycast in GuardVision goes.
    public float movementSpeed;             //For movement speed of NavAgent.
    public float searchingDuration;         //For how long guard spends at a player's last known location before returning to patrol.
    public float patrolPauseDuration;       //For how long a guard waits at each point before continuing to the next.
    public float visualReactionTime;        //For how often a guard updates their position and attempts to send a raycast when the player has been spotted.
    public float audioReactionTime;         //For how often a guard updates their position and attempts to send a raycast when the player has NOT been spotted, but has been heard.

    public GuardState guardStateDefault;
}

[System.Serializable] public enum GuardState 
{
    ResumePatrol = 0,       //For when guard is going back to patrol point where they were last headed after being interrupted.
    ActivePatrol = 1,       //For when guard is going from point to point without interruption.
    Waiting = 2,            //For when guard has reached their destination and is waiting to go to either their next patrol point or the previously interrupted one.
    Investigating = 3,      //For when player enters guard radius but isn't actively spotted by raycast, and guard is heading to the last known player position.
    Pursuing = 4,           //For when guard has spotted player and is actively chasing.
}
