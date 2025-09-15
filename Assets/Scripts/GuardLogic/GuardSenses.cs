using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardSenses : MonoBehaviour
{
    [SerializeField] Rigidbody playerRB;
    [SerializeField] Rigidbody guardRB;
    [SerializeField] LayerMask obscuringLayers;
    [SerializeField] Collider guardHearing;

    enum GuardStates
    { 
        Idle = 0,
        Patrolling = 1,
        Investigating = 2,
        Pursuit = 3
    }

    private GuardStates currentGuardState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentGuardState = GuardStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StateUpdater()
    { 
        
    }

    public void OnHeard()
    { 
        
    }
}
