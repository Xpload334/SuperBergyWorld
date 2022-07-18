using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject camera;
    public Collider collider;
    public Transform cameraTarget;

    private void Awake()
    {
        //Can I apply the camera to this object?
        //If set to null, camera is the first child of this object
        if (camera == null)
        {
            camera = transform.GetChild(0).gameObject;
        }
        cameraTarget = camera.GetComponent<CinemachineVirtualCamera>().m_Follow;

        if (collider == null)
        {
            collider = GetComponent<Collider>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            //Later, add a check to see if this collider is attached to the active player
            //Class the manages the party will allocate control to the given player (or rather, player states)
            camera.SetActive(true);
            Debug.Log(camera.name+" is active.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            camera.SetActive(false);
            Debug.Log(camera.name+" is not active.");
        }
    }

    public void changeCameraTarget(GameObject gameObject)
    {
        if (cameraTarget == gameObject.transform)
        {
            Debug.Log("Target of "+camera.name+" is already "+gameObject.name);
            return;
        }

        camera.GetComponent<CinemachineVirtualCamera>().m_Follow = gameObject.transform;
        Debug.Log("Changed target of "+camera.name+" to "+gameObject.name);
    }
}
