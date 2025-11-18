using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform target; 

    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0f, -10f); 

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Kamera-Ziel (Goat) wurde nicht zugewiesen!");
            return;
        }

       
        transform.position = target.position + offset;
        
       
    }
}