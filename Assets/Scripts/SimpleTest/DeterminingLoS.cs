using System.Collections;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DeterminingLoS : MonoBehaviour
{
    [SerializeField] GameObject playerObj;

    Vector3 playerPosition;
    Vector3 guardPosition;
    Vector3 inversePlayerToGuardTransform;
    Vector3 inverseGuardToPlayerTransform;
    Vector3 directionToPlayer;
    Vector3 dotForwardVector;
    Vector3 oppositeDotVector;

    Vector3 guardForwardVector;
    Vector3 leftSideOfViewArc;
    Vector3 rightSideOfViewArc;
    Vector3 flatGuardVector;

    public float dotProduct;
    public float oppositeDotProduct;
    public float dotForwardAddition;
    public float angleBetweenForwardPlayer;
    public float relativeXMagnitude;

    public float distanceToPlayer;
    public float inverseDistanceToPlayer;

    public float raycastDistance;
    public float viewAngle;
    public float viewUpdateTimer;

    public bool testTrigger;
    public bool raycastResult;
    public bool drawLineBool;
    public bool inverseBool;
    public bool drawDotGuardForward;

    public LayerMask layerMaskTest;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guardForwardVector = transform.forward;
        flatGuardVector = new Vector3(guardForwardVector.x, 0, guardForwardVector.z);
        relativeXMagnitude = Mathf.Cos(60);
        rightSideOfViewArc = new Vector3(relativeXMagnitude, 0, guardForwardVector.z);
        leftSideOfViewArc = new Vector3(-relativeXMagnitude, 0, guardForwardVector.z);

        layerMaskTest = LayerMask.GetMask("Default", "RaycastTest");
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = playerObj.GetComponentInChildren<Rigidbody>().transform.position;
        guardPosition = transform.position;
        directionToPlayer = ((playerPosition - guardPosition).normalized);
        inverseDistanceToPlayer = directionToPlayer.x;

        dotProduct = Vector3.Dot(transform.forward, directionToPlayer);
        Vector3 splinkVector = new Vector3(dotProduct, 0, dotProduct).normalized;
        oppositeDotProduct = Vector3.Dot(directionToPlayer, transform.forward);
        oppositeDotVector = new Vector3(-oppositeDotProduct, 0, transform.forward.z);
        dotForwardVector = new Vector3(dotProduct, 0, transform.forward.z);

        angleBetweenForwardPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        distanceToPlayer = Vector3.Distance(guardPosition, playerPosition);

        Vector3 dotDoubleVec = new Vector3(dotProduct, 0, 1).normalized;
        Vector3 dotDoubleVecNeg = new Vector3(1, 0, dotProduct).normalized;

        Vector3 addForward = transform.forward * distanceToPlayer;
        addForward = transform.position + addForward;

        //Debug.DrawLine(transform.position, playerPosition, Color.red);
    
        //Debug.DrawLine(addForward, playerPosition, Color.cyan);

        //Debug.DrawRay(guardPosition, rightSideOfViewArc, Color.red);
        //Debug.DrawRay(guardPosition, transform.forward * distanceToPlayer, Color.green); 
        //Debug.DrawRay(guardPosition, leftSideOfViewArc, Color.red);
        Vector3 halfPosVector = new Vector3(0.66f, 0, 1);
        Vector3 halfNegVector = new Vector3(-0.66f, 0, 1);

        Debug.DrawRay(guardPosition, halfPosVector, Color.black);
        Debug.DrawRay(guardPosition, halfNegVector, Color.black);

        Debug.DrawRay(guardPosition, oppositeDotVector, Color.red);
        Debug.DrawRay(guardPosition, dotForwardVector, Color.red);

        Debug.DrawRay(guardPosition, dotDoubleVec, Color.yellow);
        //Debug.DrawRay(guardPosition, dotDoubleVecNeg, Color.cyan);
        Debug.DrawRay(guardPosition, -dotDoubleVec, Color.yellow);
        //Debug.DrawRay(guardPosition, -dotDoubleVecNeg, Color.cyan);

        Debug.DrawRay(guardPosition, directionToPlayer.normalized, Color.cyan);

        Debug.DrawRay(guardPosition, splinkVector, Color.magenta);

        //So when dotProduct <= viewAngle.x is what we are tracking.
    }

    private void FixedUpdate()
    {
        if(testTrigger)
        { 
            //Vector3 normalizedPlayerDirVector = directionToPlayer.normalized;
            //float angleBetween = Vector3.Angle(normalizedPlayerDirVector, transform.forward);
            if(dotProduct >= 0.66f)
            { 
                raycastResult = Physics.Raycast(guardPosition, directionToPlayer, Mathf.Infinity, layerMaskTest);
                Debug.DrawLine(guardPosition, playerPosition, Color.green);
            }
        }
        if(drawLineBool)
        { 
            //Debug.DrawLine(guardPosition, playerPosition, Color.red);
            if(inverseBool)
            { 
                //Debug.DrawRay(guardPosition, inversePlayerToGuardTransform, Color.cyan);
                //Debug.DrawRay(playerPosition, inverseGuardToPlayerTransform, Color.red);
            }
            if(drawDotGuardForward)
            {
            }

            //L = theta * radius
            //60 degrees

        }
    }

    IEnumerator VisionChecker()
    { 
        yield return null;    
    }
}
