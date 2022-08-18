using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    //public int steps;

    //public PlayerStateMachine targetPlayer;
    public PlayerStateMachine player;
    //private Queue<InputStore> inputRecord = new Queue<InputStore>();

    public bool logRecords;

    public NavMeshAgent nav;
    public float minVelocity = 0.1f;
    public float stoppingDistance = 3f;
    public float animationBufferDistance = 0.1f;

    public bool ignoreBuffer;
    public Vector3 moveVelocity;
    

    //private NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = GetComponent<PlayerStateMachine>();

        nav.stoppingDistance = stoppingDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player.shouldFollow)
        {
            
            nav.SetDestination(target.position);
            //Vector3 moveVelocity = Vector3.Normalize(nav.velocity);
            moveVelocity = Vector3.Normalize(nav.desiredVelocity);

            //If remaining distance further than stopping distance + buffer, animation input is 0
            /*
            if (nav.remainingDistance < nav.stoppingDistance + animationBufferDistance)
            {
                player.CurrentMovementInput = Vector2.zero;
            }
            else
            {
                int moveX = Mathf.RoundToInt(moveVelocity.x);
                int moveY = Mathf.RoundToInt(moveVelocity.z);
                player.CurrentMovementInput = new Vector2(moveX, moveY); //For walk animation
            }
            */
            int moveX = Mathf.RoundToInt(moveVelocity.x);
            int moveY = Mathf.RoundToInt(moveVelocity.z);
            player.CurrentMovementInput = new Vector2(moveX, moveY); //For walk animation


        }
            
        
    }
}
