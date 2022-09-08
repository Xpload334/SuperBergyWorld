using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum OffMeshLinkMoveMethod
{
    Teleport,
    NormalSpeed,
    Parabola
}

[RequireComponent(typeof(NavMeshAgent))]
public class FollowPlayer : MonoBehaviour
{
    
    public Transform target;
    private PlayerStateMachine player;
    private CharacterController characterController;

    public NavMeshAgent nav;
    public float minVelocity = 0.01f;
    public float stoppingDistance = 3f;
    public Vector3 moveVelocity;

    public float updateSpeed = 0.1f; //how frequently to recalculate path
    public bool isRunning;

    [Header("OffLink Movement")] 
    public OffMeshLinkMoveMethod Method = OffMeshLinkMoveMethod.Parabola;
    public float jumpHeight = 2f;
    public float maxJumpHeight = 2f;
    public float jumpDuration = 0.5f;

    public delegate void LinkEvent();
    public LinkEvent OnLinkStart;
    public LinkEvent OnLinkEnd;

    //private NavMeshAgent nav;
    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        player = GetComponent<PlayerStateMachine>();
        characterController = GetComponent<CharacterController>();

        nav.stoppingDistance = stoppingDistance;
        nav.autoTraverseOffMeshLink = false;
    }


    private IEnumerator FollowTarget(Transform targetTransform)
    {
        isRunning = true;
        WaitForSeconds wait = new WaitForSeconds(updateSpeed);
        
        while (player.shouldFollow)
        {
            nav.SetDestination(target.position);

            //If off mesh link
            if (nav.isOnOffMeshLink)
            {
                //On link start
                OnLinkStart?.Invoke();
                
                if (Method == OffMeshLinkMoveMethod.NormalSpeed)
                    yield return StartCoroutine(NormalSpeed(nav));
                else if (Method == OffMeshLinkMoveMethod.Parabola)
                    yield return StartCoroutine(Parabola(nav, jumpHeight, jumpDuration));
                
                //On link end
                nav.CompleteOffMeshLink();
                OnLinkEnd?.Invoke();
            }
            
            //Distance check
            if (nav.velocity.magnitude > minVelocity)
            {
                moveVelocity = Vector3.Normalize(nav.velocity);
                Vector2 animationVector = new Vector2(moveVelocity.x, moveVelocity.z);
                player.UpdateMoveAnims(animationVector); //For walk animation
            }
            else
            {
                player.UpdateMoveAnims(Vector2.zero); //For walk animation
            }
    
            yield return wait;
        }

        isRunning = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO: handle pathfinding changes in PlayerStateMachine/PartyCharacterManager
        /*
        if (player.shouldFollow && !nav.enabled)
        {
            EnablePathfinding();
        }
        else if (!player.shouldFollow)
        {
            DisablePathfinding();
        }
        */
    }

    public void EnablePathfinding(Transform targetTransform)
    {
        nav.enabled = true; //Enable nav agent
        characterController.enabled = false; //Disable character controller

        if (isRunning)
        {
            StopCoroutine(FollowTarget(target));
        }
        
        target = targetTransform;
        
        StartCoroutine(FollowTarget(targetTransform));
    }

    public void EnableNavAgent()
    {
        nav.enabled = true;
    }

    public void DisableNavAgent()
    {
        nav.enabled = false;
    }

    public void DisablePathfinding()
    {
        nav.enabled = false; //Disable nav agent
        characterController.enabled = true; //Enable character controller
        
        StopCoroutine(FollowTarget(target));

        target = null;
    }
    
    IEnumerator NormalSpeed(NavMeshAgent agent)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        while (agent.transform.position != endPos)
        {
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Parabola(NavMeshAgent agent, float height, float duration)
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }

}
