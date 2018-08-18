using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    float cameraDistance;
    Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        // Detach the transform from player
        transform.parent = null;
    }

    public void setTarget(Transform target) {
        this.playerTransform = target;
    }

    void FixedUpdate()
    {
        if(playerTransform != null) 
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z - cameraDistance);
        }
    }
}
