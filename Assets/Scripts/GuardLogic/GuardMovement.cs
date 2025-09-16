using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] Transform playerTransform;

    Transform guardTransform;
    Rigidbody guardRB;
    Vector3 playerLastHeardPosition;
    Coroutine coroutineRef;

    GuardSenses.GuardStates activeGuardState;

    private int patrolPointIndex;
    public int patrolCurrentIndex;
    public int patrolArrayLength;
    public float distanceToTarget;
    public float outputSeconds = 0.1f;

    [Header("Bools")]
    public bool withinRange;
    public bool waitingToUpdate;
    public bool isAlerted;
    public bool coroutineIsRunning;
    public bool isCurrentlyHearingPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerLastHeardPosition = playerTransform.position;
        patrolArrayLength = patrolPoints.Length;
        patrolCurrentIndex = 0;
        waitingToUpdate = false;
        activeGuardState = GuardSenses.GuardStates.Patrolling;

        PatrolUpdate(activeGuardState);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position, patrolPoints[patrolCurrentIndex].position);

        if (distanceToTarget <= 1)
        { 
            withinRange = true;            
        }
        else if (distanceToTarget > 1)
        { 
            withinRange = false;
        }
        if(isCurrentlyHearingPlayer)
        { 
            playerLastHeardPosition = playerTransform.position;
            isAlerted = true;
        }
    }

    private void MovementUpdate(int inputIndex)
    {
        Vector3 PositionTranslator(int indexValue)
        {
            Vector3 outputVector = new Vector3 (0,0,0);                         //Makes sure there is some Vector as a default.

            if(indexValue <= patrolPoints.Length)                               //Checks if the input index is within the range of the patrol array.
            {
                patrolCurrentIndex = indexValue % patrolPoints.Length;
                outputVector = patrolPoints[patrolCurrentIndex].position;
            }
            else if(indexValue == patrolPoints.Length + 1)                      //Checks if input is one more than the array length, designating it as a player location.
            {
                Debug.Log($"Going to player's last known location: {playerLastHeardPosition.ToString()}");
                outputVector = playerLastHeardPosition;
            }
            else 
            { 
                Debug.Log("Outside of array bounds, and not player position. Player position is patrol array length + 1.");
                outputVector = transform.position;
            }

            return outputVector;
        }

        Vector3 translatedPosition = PositionTranslator(inputIndex);
        navAgent.SetDestination(translatedPosition);
        //coroutineRef = StartCoroutine(ReactionTime(outputSeconds));
    }

    public void PatrolUpdate(GuardSenses.GuardStates currentGuardState)
    {
        Debug.Log("PatrolUpdate called.");
        string guardStateString = currentGuardState.ToString();

        //guardStateString = new string ("Patrolling");               //OVERWRITES TO ALWAYS BE PATROLLING

        
        if(withinRange && !isAlerted)
        { 
            patrolCurrentIndex++;
        }

        if(!isAlerted)
        {      
            switch (guardStateString) 
            {
                case "Idle":
                { 
                    //MovementUpdate(patrolCurrentIndex);
                    //outputSeconds = 0.25f;
                    break;
                }
                case "Patrolling":
                {
                    MovementUpdate(patrolCurrentIndex);
                    //patrolCurrentIndex++;
                    //outputSeconds = 20f;
                    break;
                }
            }
        }
        else if (isAlerted)
        {
            StopAllCoroutines();
            Debug.Log("Stopped All Coroutines");
            coroutineIsRunning = false;
            switch (guardStateString) 
            {   
                case "Investigating":
                { 
                    MovementUpdate(patrolPoints.Length + 1);
                    //outputSeconds = 4f;
                    break;
                }
                case "Pursuit":
                { 
                    //MovementUpdate(patrolPoints.Length + 1);
                    //outputSeconds = 0.1f;
                    break;
                } 
            }
        }
        
        coroutineRef = StartCoroutine(ReactionTime(outputSeconds));
        //float elapsedTime = 0f;
        //float duration = 10f;
        //while (elapsedTime < duration)
        //{ 
        //    elapsedTime += Time.deltaTime;
        //}
        //if(elapsedTime == duration)
        //{ 
        //    PatrolUpdate(activeGuardState);
        //    elapsedTime = 0f;
        //}
        //else if (elapsedTime > duration)
        //{ 
        //    //elapsedTime = duration;
        //    Debug.Log("elapsedTime past duration");
        //}
    }

    IEnumerator ReactionTime(float secondsToWait)
    {
        Debug.Log("Coroutine ReactionTime started.");
        coroutineIsRunning = true;
        float elapsedTime = 0f;
        float duration = secondsToWait;

        while (!withinRange)
        {
            yield return null;
        }

        if(withinRange && elapsedTime == 0f)
        { 
            Debug.Log($"Made it to destination, started waiting for {secondsToWait} seconds.");
        }
        else if(withinRange) 
        {
            elapsedTime += Time.deltaTime; 
        }

        yield return new WaitForSecondsRealtime(secondsToWait);
        isAlerted = false;
        Debug.Log($"Coroutine done waiting. Waited {secondsToWait} seconds.");
        coroutineIsRunning = false;
        PatrolUpdate(activeGuardState);
    }
}
