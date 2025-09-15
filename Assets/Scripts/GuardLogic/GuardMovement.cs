using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform[] patrolPoints;

    Transform guardTransform;
    Rigidbody guardRB;

    private int patrolPointIndex;
    private int patrolCurrentIndex;
    public float distanceToTarget;
    public bool withinRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        patrolCurrentIndex = 0;

        MovementTargetUpdate(patrolCurrentIndex);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(transform.position, patrolPoints[patrolCurrentIndex].position);

        if ((distanceToTarget < 1) && !withinRange)
        {
            patrolCurrentIndex++;
            MovementTargetUpdate(patrolCurrentIndex);
            withinRange = true;            
        }
        else if (distanceToTarget >= 1)
        { 
            withinRange = false;
        }
    }

    private void MovementTargetUpdate(int inputIndex)
    {
        patrolCurrentIndex = inputIndex % patrolPoints.Length;
        Debug.Log($"Input Index: {patrolCurrentIndex}" );
        navAgent.SetDestination(patrolPoints[patrolCurrentIndex].position);
    }

    public void PatrolUpdate()
    { 
        
    }
}
