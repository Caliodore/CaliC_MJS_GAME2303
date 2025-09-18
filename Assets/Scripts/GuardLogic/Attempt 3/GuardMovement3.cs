using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement3 : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] Rigidbody guardRigidbody;
    [SerializeField] Transform patrolMarkerPrefab;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] NavMeshAgent guardNavAgent;

    [Header("Movement Values")]
    public float moveSpeed, searchingDuration, patrolPauseDuration;

    [Header("Bools")]
    public bool withinRange, travellingToDestination, updatingPlayerLastKnown;

    public GuardState activeGuardState;
    public GuardState previousGuardState;
    private int activeStateInt;
    private int previousStateInt;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeMoveValues(float ms, float sd, float ppd)
    {
        moveSpeed = ms;
        searchingDuration = sd;
        patrolPauseDuration = ppd;
    }

    public void MovementChangeState(GuardState changingState)
    {
        if(activeGuardState != changingState)
        { 
            previousGuardState = activeGuardState;
            //StateChangedLogicHandler(changingState);
        }
    }
    
    IEnumerator WaitingOnArrival()
    { 
        yield return null;    
    }
    IEnumerator CheckIfArrived()
    {
        while(withinRange)
        { 
                
        }
        yield return null;    
    }
}
