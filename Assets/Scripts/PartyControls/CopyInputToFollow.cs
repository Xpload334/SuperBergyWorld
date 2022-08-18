using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CopyInputToFollow : MonoBehaviour
{
    public int steps;

    public PlayerStateMachine targetPlayer;
    public PlayerStateMachine player;
    private Queue<InputStore> inputRecord = new Queue<InputStore>();

    public bool logRecords;
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        //Record leader position
        if (Vector3.Distance(target.position, transform.position) > distance)
        {
            record.Enqueue(target.transform.position);
        }
        
        //remove last position from record and use for our own
        if (record.Count > steps)
        {
            this.transform.position = record.Dequeue();
        }
        */

        if (player.shouldFollow)
        {
            //record leader input
            Vector2 movementInput = targetPlayer.CurrentMovementInput;
            bool jumpPressed = targetPlayer.IsJumpPressed;

            //If target player is doing nothing on the ground, do not record
            //If movement is non-zero, or jump pressed, or jumping
            /*
            if (movementInput != Vector2.zero || !jumpPressed || !player.IsJumping)
            {
                inputRecord.Enqueue(new InputStore(movementInput, jumpPressed));
            }
            */
            
            inputRecord.Enqueue(new InputStore(movementInput, jumpPressed));
            
            //remove last input from record and use for own
            //If current state involves jumping, step forward until is not
            if (inputRecord.Count > steps || player.IsJumping)
            {
                InputStore store = inputRecord.Dequeue();
                
                if (logRecords)
                {
                    Debug.Log("move="+store.MovementVector2 + " ||| jump="+store.IsJumpPressed);
                }
                player.CurrentMovementInput = store.MovementVector2;
                player.IsJumpPressed = store.IsJumpPressed;

            }
            
        }
        else
        {
            //Clear record if contains any actions
            if (inputRecord.Count != 0)
            {
                inputRecord.Clear();
            }
        }
    }
}
