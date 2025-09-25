using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GuardSenses4 : MonoBehaviour
{
    Collider hearingRadius;
    LayerMask raycastLayers;
    GuardBrain_3 attachedBrain;
    GameObject playerObj, guardObj, guardOverseer;
    
    public bool playerHeard, playerBlocked, losRunning;
    private bool doneInitializing, playerSneaking;
    private Vector3 playerPosition, guardPosition, directionToPlayer;
    private Coroutine losCoRo;

    private GuardState activeGuardState;
    private int activeStateInt;

    [Header("Vision Values")]
    public bool isVisionCheckRunning, playerSpotted;
    public float visionRange, visualReactionTime, viewAngle;

    void Start()
    {
        attachedBrain = gameObject.GetComponentInParent<GuardBrain_3>();
        hearingRadius = gameObject.GetComponent<Collider>();
        raycastLayers = LayerMask.GetMask("RaycastTest");
        playerHeard = false;
    }
    
    public void InitializeSenseValues(float vr, float vrt)
    {
        visionRange = vr;
        visualReactionTime = vrt;
        doneInitializing = true;
    }

    public void AssignSenseRefs(GameObject guardObjRef, GameObject playerObjRef, Collider hearingRadiusRef, GuardBrain_3 attachedBrainRef, LayerMask visionColliderRefs, GameObject guardOverseerRef)
    { 
        AssignHearingRefs(guardObjRef, playerObjRef, hearingRadiusRef, attachedBrainRef);
        AssignVisionRefs(guardObjRef, guardOverseerRef, playerObjRef, visionColliderRefs, attachedBrainRef);
    }

    public void AssignHearingRefs(GameObject guardObjRef, GameObject playerObjRef, Collider hearingRadiusRef, GuardBrain_3 attachedBrainRef)
    { 
        guardObj = guardObjRef;
        playerObj = playerObjRef;
        hearingRadius = hearingRadiusRef;
        attachedBrain = attachedBrainRef;
    }
    
    public void AssignVisionRefs(GameObject attachedGuardObjRef, GameObject guardOverseerRef, GameObject playerObjRef, LayerMask visionColliderRefs, GuardBrain_3 attachedBrainRef)
    {
        guardObj = attachedGuardObjRef;
        guardOverseer = guardOverseerRef;
        raycastLayers = visionColliderRefs; 
        playerObj = playerObjRef;
        attachedBrain = attachedBrainRef;
    }

    private void FixedUpdate()
    {
        if(doneInitializing)
        { 
            playerPosition = playerObj.GetComponentInChildren<Rigidbody>().transform.position;
            guardPosition = guardObj.transform.position;
            directionToPlayer = ((playerPosition - guardPosition).normalized);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerHeard = true;
            RunCoroutine();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        { 
            playerHeard = false;
            attachedBrain.PlayerAudioProximityUpdate(false);
        }
    }

    private void RunCoroutine()
    { 
         Coroutine losCoRororo;
         losCoRororo = StartCoroutine(losChecker());
    }
    
    public void SensesChangeState(GuardState changingState)
    { 
        activeGuardState = changingState;
    }

    public void PlayerStealthUpdate(bool isPlayerSneaking)
    { 
        playerSneaking = isPlayerSneaking;
    }

    IEnumerator losChecker()
    { 
        losRunning = true;
        while(losRunning)
        {   
            RaycastHit hit;
            RaycastHit[] hits = new RaycastHit[5];
            Ray rayDirection = new Ray(guardPosition, directionToPlayer);

            Debug.DrawRay(guardPosition, directionToPlayer, Color.cyan);

            bool raycastBool = Physics.Raycast(guardPosition, directionToPlayer, out hit, Mathf.Infinity, raycastLayers);
            bool playerBool = hit.collider.gameObject.CompareTag("Player");

            if(playerBool && raycastBool)    //Raycast hit something and what it hit *is* a player.
            {
                Debug.Log("Raycast hit a player. Calling Brain.PlayerSpotted().");
                playerSpotted = true;
                attachedBrain.PlayerSpotted(hit.transform.gameObject);
            }
            else                                                    //Either raycast failed or it wasnt a player.
            {
                playerSpotted = false;
                if(raycastBool)
                { 
                    Debug.Log($"Raycast collided with: {hit.collider.tag}.");
                    playerBlocked = true;
                }
                else if(playerHeard && !playerSneaking)
                {
                    attachedBrain.PlayerAudioProximityUpdate(true);
                }
                else
                { 
                    yield break;    
                }
            }
            yield return null;
        }
        yield break;
    }
}
