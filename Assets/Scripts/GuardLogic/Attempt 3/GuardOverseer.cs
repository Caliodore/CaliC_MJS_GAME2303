using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class GuardOverseer : MonoBehaviour
{
    [SerializeField] GuardPresetValues guardPresetValues;
    [SerializeField] GameObject guardPrefab;
    [SerializeField] GameObject playerObj;
    [SerializeField] GameObject[] guardsInLevel;
    [SerializeField] GameObject trackingPlayerPrefab;

    Dictionary<GameObject, GuardPlayerTrackerCoRo> guardCoRos = new Dictionary<GameObject, GuardPlayerTrackerCoRo>();

    [Header("Player-Related Vars")]
    public Vector3 playerCurrentPos;
    public float guardVisionRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerObj = FindAnyObjectByType<PlayerMovement>().gameObject;
        InitializePresetValues();
        InitializeGuardArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void InitializePresetValues()
    { 
        guardVisionRange = guardPresetValues.visionRange;
    }

    public GuardPresetValues AssignGuardSO()
    { 
        return guardPresetValues;    
    }

    public void InitializeGuardArray()
    { 
        int indexCount = 0;
        var guardComponents = FindObjectsByType<GuardBrain_3>(FindObjectsSortMode.InstanceID);
        guardsInLevel = new GameObject[guardComponents.Length];
        foreach (GuardBrain_3 guardObj in guardComponents) 
        {
            GameObject guardObject = guardComponents[indexCount].gameObject;
            guardsInLevel[indexCount] = guardObject;
            int guardID = guardsInLevel[indexCount].GetInstanceID();
            GameObject coroScript = Instantiate(trackingPlayerPrefab, guardObject.gameObject.transform.position, Quaternion.identity, guardObject.transform);
            GuardPlayerTrackerCoRo attachedTracker = coroScript.GetComponent<GuardPlayerTrackerCoRo>();
            guardCoRos.Add(guardObject, attachedTracker);
            attachedTracker.CreateCoroScript(guardID);
            indexCount++;
        }
        Debug.Log($"There are {guardsInLevel.Length} guards currently loaded.");
        Debug.Log($"There are {guardCoRos.Count} key-value pairs stored within the dictionary.");
    }

}
