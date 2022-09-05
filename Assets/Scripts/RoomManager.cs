using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public PartyCharacterManager partyCharacterManager;
    
    public GameObject cameraObject;
    public Collider thisCollider;
    public Transform cameraTarget;

    private void Awake()
    {
        partyCharacterManager = FindObjectOfType<PartyCharacterManager>();
        
        //Can I apply the camera to this object?
        //If set to null, camera is the first child of this object
        if (cameraObject == null)
        {
            cameraObject = transform.GetChild(0).gameObject;
        }
        cameraTarget = cameraObject.GetComponent<CinemachineVirtualCamera>().m_Follow;

        if (thisCollider == null)
        {
            thisCollider = GetComponent<Collider>();
        }

        ChangeCameraTarget(FindObjectOfType<CameraTarget>().transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //Later, add a check to see if this collider is attached to the active player
            //Class the manages the party will allocate control to the given player (or rather, player states)
            cameraObject.SetActive(true);
            Debug.Log(cameraObject.name+" is active.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            cameraObject.SetActive(false);
            Debug.Log(cameraObject.name+" is not active.");
        }
    }

    //Change camera target to another Transform
    public void ChangeCameraTarget(Transform targetTransform)
    {
        if (cameraTarget == targetTransform)
        {
            Debug.Log("Target of "+cameraObject.name+" is already "+targetTransform);
            return;
        }

        cameraObject.GetComponent<CinemachineVirtualCamera>().m_Follow = targetTransform;
        Debug.Log("Changed target of "+cameraObject.name+" to "+targetTransform);
    }
}
