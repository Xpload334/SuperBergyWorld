using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public void GoTo(Vector3 position)
    {
        transform.position = position;
        Debug.Log("CameraTarget moved to "+position);
    }

    public void GoTo(GameObject obj)
    {
        GoTo(obj.transform.position);
    }

    
    public void LockTo(Transform other)
    {
        var transform1 = transform;
        transform1.parent = other;
        transform1.localPosition = Vector3.zero; //Set local position to (0,0,0)
        Debug.Log("CameraTarget locked to "+other);

    }
    
    public void LockTo(GameObject obj)
    {
        LockTo(obj.transform);
    }
    
    public void Unlock()
    {
        transform.parent = null;
    }
}
