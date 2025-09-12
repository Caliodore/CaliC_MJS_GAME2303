using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerRunSpeed;
    [SerializeField] float playerSneakSpeed;

    Vector2 inputMovementVector;
    Vector3 outputMovementVector;
    Vector3 outputMovementVectorScaled;

    public bool isSneaking;
    public bool isMoving;
    public bool playerTracking;
    public float currentPlayerSpeed;

    [SerializeField] Rigidbody playerRB;
    [SerializeField] GameObject pathMarkerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSneaking = false;
        currentPlayerSpeed = playerRunSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSneak(InputAction.CallbackContext ctx)
    { 
        Debug.Log("OnSneak Invoked");
        isSneaking = true;
        currentPlayerSpeed = playerSneakSpeed;
        
        if(ctx.canceled)
        { 
            isSneaking= false;
            currentPlayerSpeed = playerRunSpeed;
        }
    }

    public void OnMovement(InputAction.CallbackContext ctx)
    {
        inputMovementVector = ctx.ReadValue<Vector2>();

        if(ctx.performed)
        {
            isMoving = true;
            outputMovementVector = new Vector3(inputMovementVector.x, 0, inputMovementVector.y);
        }

        if(ctx.canceled)
        { 
            isMoving = false;    
        }
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {
            outputMovementVectorScaled = (outputMovementVector * currentPlayerSpeed * Time.deltaTime);
            transform.Translate(outputMovementVectorScaled);
        }        
    }

    public void PathMaker(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) 
        { 
            StartCoroutine(PathSpawner());
        }
        if ((ctx.interaction is HoldInteraction))
        {
            StopAllCoroutines();   
        }
    }

    IEnumerator PathSpawner()
    {
        while(playerTracking)
        {
            Instantiate(pathMarkerPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }    
    }


}
