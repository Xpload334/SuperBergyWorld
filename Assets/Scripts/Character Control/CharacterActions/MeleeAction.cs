using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : CharacterAction
{
    public LayerMask worldLayer;
    public LayerMask objectLayer;
    public float attackRange = 1f;
    
    
    public override void TriggerActionEffect()
    {
        Vector3 rayDirection = new Vector3(player.LastMovementX, 0, player.LastMovementY).normalized;
        Vector3 rayOrigin = player.transform.position + new Vector3(0, 0.5f, 0.2f);
        
        //Debug.Log("Firing ray towards "+rayDirection);
        Ray ray = new Ray(rayOrigin, rayDirection);
        Debug.DrawRay(ray.origin, ray.direction * attackRange, Color.magenta, 1);

        RaycastHit hitData;
        //If hits world
        if (Physics.Raycast(ray, out hitData, attackRange, worldLayer))
        {
            Debug.Log("Hit wall at: "+hitData.point);
        }
        //If ray hits object
        else if (Physics.Raycast(ray, out hitData, attackRange, objectLayer))
        {
            Debug.Log("Hit object at: "+hitData.point);
            if (hitData.transform.TryGetComponent(out DestructableObject destructableObject))
            {
                destructableObject.DestroyObject();
            }
        }
        

    }

    
}
