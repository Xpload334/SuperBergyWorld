using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovingPlatform : MonoBehaviour
{
    public bool shouldMove = true;
    [SerializeField] 
    private Vector3[] positions;
    [SerializeField] 
    private float dockDuration = 2f;

    [SerializeField] private float moveSpeed = 1f;
    private List<NavMeshAgent> _agentsOnPlatform = new List<NavMeshAgent>();
    private List<CharacterController> _characterControllersOnPlatform = new List<CharacterController>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MovePlatform());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent agent))
        {
            _agentsOnPlatform.Add(agent);
        }

        if (other.TryGetComponent(out CharacterController controller))
        {
            _characterControllersOnPlatform.Add(controller);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out NavMeshAgent agent))
        {
            _agentsOnPlatform.Remove(agent);
        }
        
        if (other.TryGetComponent(out CharacterController controller))
        {
            _characterControllersOnPlatform.Remove(controller);
        }
    }

    private IEnumerator MovePlatform()
    {
        transform.position = positions[0];
        int positionIndex = 0;
        int lastPositionIndex = 0;
        WaitForSeconds wait = new WaitForSeconds(dockDuration);

        //While should move
        while (shouldMove)
        {
            //Increment position index
            lastPositionIndex = positionIndex;
            positionIndex++;
            if (positionIndex >= positions.Length)
            {
                positionIndex = 0;
            }

            //Calculate move direction and distance
            Vector3 platformMoveDirection = (positions[positionIndex] - positions[lastPositionIndex]).normalized;
            float distance = Vector3.Distance(transform.position, positions[positionIndex]);
            float distanceTravelled = 0;

            //While not at destination
            while (distanceTravelled < distance)
            {
                Vector3 moveIncrement = platformMoveDirection * (moveSpeed * Time.deltaTime);

                transform.position += moveIncrement; //move platform
                distanceTravelled += moveIncrement.magnitude; //increment distance

                //Move all agents on platform by same amount
                for (int i = 0; i < _agentsOnPlatform.Count; i++)
                {
                    _agentsOnPlatform[i].Warp(_agentsOnPlatform[i].transform.position + moveIncrement);
                }
                
                //Move all characters on platform by same amount
                for (int i = 0; i < _characterControllersOnPlatform.Count; i++)
                {
                    if (_characterControllersOnPlatform[i].enabled)
                    {
                        _characterControllersOnPlatform[i].Move(moveIncrement);
                    }
                }
                
                yield return null;
            }
            transform.position = positions[positionIndex]; //Snap to correct position
            yield return wait; //Wait for dock duration
            
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
