using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GuardVision_3 : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] GameObject guardsOverseer;
    [SerializeField] GameObject playerRB;
    [SerializeField] LayerMask visionInteractionLayers;
    [SerializeField] ScriptableObject guardPresetValues;

    [Header("Vision Values")]
    public float visionRange, visualReactionTime, viewAngle;

    private GuardState activeGuardState;
    private int activeStateInt;
    private bool playerHeard;
    private bool isVisionCheckRunning;
    private bool playerSpotted;

    private Coroutine visionCheckerCoRo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHeard = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    public void VisionChangeState(GuardState changingState)
    { 
        activeGuardState = changingState;
        activeStateInt = (int)activeGuardState;
        if(activeStateInt > 2)
        {
            if((visionCheckerCoRo != null) && true)
            StartCoroutine(VisionChecker());
            playerHeard = true;
        }
    }


    public void InitializeVisionValues(float vr, float vrt)
    {
        visionRange = vr;
        visualReactionTime = vrt;
    }

    IEnumerator VisionChecker()
    {
        isVisionCheckRunning = true;
        while(!playerSpotted)        //Firing a raycast every x seconds to determine if LoS to player.
        { 
            
            yield return new WaitForSeconds(visualReactionTime);  
        }
        if(playerSpotted)
        {
            while (playerSpotted)
            { 
                
                yield return new WaitForSeconds(visualReactionTime);
            }
        }
        isVisionCheckRunning = false;
        yield break;
    }
}
