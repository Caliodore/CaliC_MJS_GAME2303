using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuardVision_3 : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] GameObject guardsOverseer;
    [SerializeField] GuardBrain_3 attachedBrain;
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject guardObj;
    [SerializeField] LayerMask visionInteractionLayers;

    [Header("Vision Values")]
    public bool isVisionCheckRunning;
    public float visionRange, visualReactionTime, viewAngle;

    private GuardState activeGuardState;
    private int activeStateInt;
    private bool playerHeard, playerSpotted, doneInitializing, playerSneaking;

    private Vector3 playerPosition, guardPosition, directionToPlayer;
    private Coroutine visionCheckerCoRo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObj = FindAnyObjectByType<PlayerMovement>().gameObject;
        playerHeard = false;
    }



    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if(doneInitializing)
        { 
            playerPosition = playerObj.GetComponentInChildren<Rigidbody>().transform.position;
            guardPosition = transform.position;
            directionToPlayer = ((playerPosition - guardPosition).normalized);
        }
    }

    public void VisionChangeState(GuardState changingState)
    { 
        activeGuardState = changingState;
        activeStateInt = (int)activeGuardState;
        if(activeStateInt > 2)
        {
            if((visionCheckerCoRo != null) && true)
                StartCoroutine(VisionChecker());
        }
    }


    public void InitializeVisionValues(float vr, float vrt)
    {
        visionRange = vr;
        visualReactionTime = vrt;
        doneInitializing = true;
    }

    public void AssignVisionRefs(GameObject attachedGuardObjRef, GameObject guardOverseerRef, GameObject playerObjRef, LayerMask visionColliderRefs, GuardBrain_3 attachedBrainRef)
    {
        guardObj = attachedGuardObjRef;
        guardsOverseer = guardOverseerRef;
        visionInteractionLayers = visionColliderRefs; 
        playerObj = playerObjRef;
        attachedBrain = attachedBrainRef;
    }

    IEnumerator VisionChecker()
    {
        Debug.Log("VisionScript Coroutine running.");
        isVisionCheckRunning = true;
        while(isVisionCheckRunning)        //Firing a raycast every x seconds to determine if LoS to player.
        {
            RaycastHit hit;
            bool raycastBool = Physics.Raycast(guardPosition, directionToPlayer, out hit, Mathf.Infinity, visionInteractionLayers);

            if(hit.collider.CompareTag("Player") && raycastBool)    //Raycast hit something and what it hit *is* a player.
            {
                Debug.Log("Raycast hit a player. Calling Brain.PlayerSpotted().");
                playerSpotted = true;
                attachedBrain.PlayerSpotted(hit.transform.gameObject);
            }
            else                                                    //Either raycast failed or it wasnt a player.
            {
                if(raycastBool)
                    Debug.Log($"Raycast collided with: {hit.collider.tag}.");
                playerSpotted = false;
            }

            yield return new WaitForSeconds(visualReactionTime);

            if(!playerSpotted)
            {
                isVisionCheckRunning = false; 
                yield break;    
            }
            yield return null;
        }
        yield break;
    }
}
