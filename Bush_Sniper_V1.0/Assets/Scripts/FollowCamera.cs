using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // This class makes the camera following a target in a given distance.

    [SerializeField]
    float cameraDistance;
    Transform playerTransform;

    // Use this for initialization
    void Start()
    {
        // detach from player, we want our own movement here
        transform.parent = null;
    }

    public void setTarget(Transform target)
    {
        // we dont move our object with the player but we still need it's transform to know the players position
        this.playerTransform = target;
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z - cameraDistance);
        }
    }
}
