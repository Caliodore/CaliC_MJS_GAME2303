using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PlayerStealth : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject pathMarkerPrefab;

    public bool isSneaking;
    public bool inHearingRange;

    GameObject alertedGuard;

    public UnityEngine.Object[] guardArray;
    public GameObject[] guardObjects;
    int arrayLength;

    Coroutine coroutineRef;
    UnityEvent guardHeard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        guardArray = FindObjectsByType<GuardSenses>(FindObjectsSortMode.None);
        arrayLength = guardArray.Length;
        guardObjects = GuardObjectArrayCreate();
        //gameManager = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Guard Hearing Radius"))
        {
            Debug.LogWarning("Attempting to contact Game Manager.");
            alertedGuard = other.gameObject.GetComponentInParent<GuardSenses>().gameObject;
            gameManager.GuardHeardPlayer(alertedGuard);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Guard Hearing Radius"))
        {
            Debug.LogWarning("Attempting to contact Game Manager.");
            alertedGuard = other.gameObject.GetComponentInParent<GuardSenses>().gameObject;
            gameManager.GuardCantHearPlayer(alertedGuard);
        }

        //Instantiate(pathMarkerPrefab);
    }

    IEnumerator AlertCheck(GameObject guardObj)
    {
        guardObj = alertedGuard;
        if(!isSneaking && inHearingRange)
        { 
            
        }
        yield return null;
    }

    private GameObject[] GuardObjectArrayCreate()
    {
        int currentIndex = 0;
        var guardObjectsHolderArray = new GameObject[arrayLength];
        foreach(UnityEngine.Object currentObj in guardArray)
        {
            Debug.Log($"Current index is: {currentIndex}.");
            guardObjectsHolderArray[currentIndex] = currentObj.GameObject().gameObject;
            currentIndex++;
        }
        return guardObjectsHolderArray;
    }
}
