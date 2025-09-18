using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class GuardNavigation : MonoBehaviour
{
    [SerializeField] NavMeshAgent guardNavAgent;
    [SerializeField] GameObject[] patrolPoints;
    [SerializeField] GameObject playerObject;

    public int patrolCurrentIndex;
    private int playerIndexFlag;

    public float distanceToTarget;
    public float searchingDuration;
    public float elapsedTime;

    [Header("Vectors")]
    public Vector3 playerLastHeardPosition;
    public Vector3 navDestination;
    public Vector3 destinationTransform;

    [Header("Booleans")]
    public bool withinRange;
    public bool travellingToPoint;
    public bool isAlerted;
    public bool trackingPlayer;
    public bool playerActiveInRadius;
    public bool waitingBoolTrigger;

    Coroutine waitingCoroutine;
    Coroutine trackingCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        searchingDuration = 6f;
        patrolCurrentIndex = 0;
        playerIndexFlag = patrolPoints.Length + 1;
        MovementUpdate(patrolCurrentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        navDestination = new Vector3(guardNavAgent.destination.x, transform.position.y, guardNavAgent.destination.z);
        distanceToTarget = Vector3.Distance(transform.position, navDestination);

        if (distanceToTarget <= 1)
        { 
            withinRange = true;
            travellingToPoint = false;
        }
        else if (distanceToTarget > 1)
        { 
            withinRange = false;
        }
    }

    public void MovementUpdate(int inputIndex)
    {
        travellingToPoint = true;
        Vector3 PositionTranslator(int indexValue)
        {
            Vector3 outputVector = new Vector3 (0,0,0);                         //Makes sure there is some Vector as a default.

            if(indexValue <= patrolPoints.Length)                               //Checks if the input index is within the range of the patrol array.
            {
                patrolCurrentIndex = indexValue % patrolPoints.Length;
                outputVector = patrolPoints[patrolCurrentIndex].transform.position;
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
                
            destinationTransform = outputVector;
            return outputVector;
        }

        Vector3 translatedPosition = PositionTranslator(inputIndex);
        guardNavAgent.SetDestination(translatedPosition);
        
        if(waitingCoroutine != null)
            StopCoroutine(waitingCoroutine);

        CoroutineStateUpdater();
   
        Debug.Log("Finished MovementUpdate() iteration.");
    }

    public void CoroutineStateUpdater()
    {
        if(!isAlerted)
        { 
            waitingCoroutine = StartCoroutine(WaitingOnArrival());
        }
        if((trackingCoroutine == null) && isAlerted)
        { 
            trackingCoroutine = StartCoroutine(TrackingPlayer());
        }
    }

    public void PlayerHeard()
    {
        isAlerted = true;
        playerActiveInRadius = true;
        CoroutineStateUpdater();
    }

    public void PlayerLost()
    { 
        playerActiveInRadius = false;
        Debug.Log("Player left hearing radius.");
    }


    IEnumerator TrackingPlayer()
    {
        Debug.LogWarning("PLAYER IS BEING TRACKED");
        trackingPlayer = false;
        //isAlerted = true;
        StopCoroutine(waitingCoroutine);
        elapsedTime = 0f;
        yield return new WaitForSeconds(1f);
        while(isAlerted)
        {
            while(playerActiveInRadius)
            {
                //isAlerted = true;
                playerLastHeardPosition = playerObject.GetComponentInChildren<Rigidbody>().transform.position;
                MovementUpdate(playerIndexFlag);
                elapsedTime = 0f;
                yield return null;
            }
            while(withinRange && !playerActiveInRadius)
            {
                Debug.Log("At location, waiting.");
                travellingToPoint = false;
                elapsedTime += Time.deltaTime;
                if(elapsedTime >= searchingDuration)
                {
                    Debug.Log("Elapsed searching duration.");
                    trackingPlayer = false;
                    isAlerted = false;
                    MovementUpdate(patrolCurrentIndex);
                    StopCoroutine(trackingCoroutine);
                }
                yield return null;
            }
            yield return null;
        }
    }

    IEnumerator WaitingOnArrival()
    {
        elapsedTime = 0f;
        Debug.Log("Coroutine started");
        waitingBoolTrigger = true;
        while(waitingBoolTrigger)
        {
            if(withinRange && !isAlerted)
            {
                travellingToPoint = false;
                Debug.Log("Coroutine realized object was in range");
                while(withinRange && (elapsedTime < searchingDuration))
                {
                    elapsedTime += Time.deltaTime;
                    if(elapsedTime >= searchingDuration)
                    {
                        Debug.Log("Finished waiting.");
                        waitingBoolTrigger = false;
                        patrolCurrentIndex++;
                        MovementUpdate(patrolCurrentIndex);
                    }
                    yield return null;
                }
            }
            yield return null;
        }
        yield return null;
    }
}
