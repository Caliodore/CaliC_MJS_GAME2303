using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

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
    public bool isPlayerSneaking;
    public UnityEvent playerSneak;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSneak = new UnityEvent();
        playerObj = FindAnyObjectByType<PlayerMovement>().gameObject;
        InitializePresetValues();
        InitializeGuardArray();
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

    /// <summary>
    /// To be called by the player to tell whether or not they're sneaking at any moment. This is so the guard overseer can update the guards as needed on whether they can sense the player.
    /// </summary>
    /// <param name="playerSneaking"></param>
    public bool PlayerStealthStateUpdate(bool playerStealthState)
    { 
        isPlayerSneaking = playerStealthState;
        return isPlayerSneaking;
    }

    public void AddStealthListener(UnityAction listener)
    { 
        playerSneak.AddListener(listener);
    }

    public void RemoveStealthListener(UnityAction listener)
    { 
        playerSneak.RemoveListener(listener);
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
