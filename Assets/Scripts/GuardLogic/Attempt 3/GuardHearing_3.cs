using UnityEngine;
using UnityEngine.Events;

public class GuardHearing_3 : MonoBehaviour
{   
    [Header("Component References")]
    [SerializeField] GameObject guardObj;
    [SerializeField] GuardBrain_3 attachedBrain;
    [SerializeField] GameObject playerObj;
    [SerializeField] Collider guardHearingRadius;

    private bool doneInitializing, playerSneaking;
    private float audioReactionTime;
    private GuardState activeGuardState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attachedBrain = gameObject.GetComponentInParent<GuardBrain_3>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        { 
            attachedBrain.PlayerAudioProximityUpdate(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        { 
            attachedBrain.PlayerAudioProximityUpdate(false);
        }
    }

    public void PlayerStealthUpdate(bool isPlayerSneaking)
    { 
        playerSneaking = isPlayerSneaking;
    }

    public void InitializeHearingValues(float art)
    { 
        audioReactionTime = art;
        doneInitializing = true;
    }

    public void AssignHearingRefs(GameObject guardObjRef, GameObject playerObjRef, Collider hearingRadiusRef, GuardBrain_3 attachedBrainRef)
    { 
        guardObj = guardObjRef;
        playerObj = playerObjRef;
        guardHearingRadius = hearingRadiusRef;
        attachedBrain = attachedBrainRef;
    }

    public void HearingChangeState(GuardState changingState)
    { 
        activeGuardState = changingState;
    }
}
