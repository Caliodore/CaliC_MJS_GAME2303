using UnityEngine;

public class GuardHearing_3 : MonoBehaviour
{   
    [Header("Component References")]
    [SerializeField] Rigidbody guardRB;
    [SerializeField] GuardBrain_3 attachedBrain;
    [SerializeField] GameObject playerRB;
    [SerializeField] Collider guardHearingRadius;
    [SerializeField] LayerMask visionInteractionLayers;

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
            attachedBrain.OnPlayerEntersAudioRange();
        }
    }

    public void InitializeHearingValues(float art)
    { 
        audioReactionTime = art;
    }

    public void HearingChangeState(GuardState changingState)
    { 
        activeGuardState = changingState;
    }
}
