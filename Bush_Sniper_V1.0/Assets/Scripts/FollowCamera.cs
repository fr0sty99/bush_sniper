using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // This class makes the camera following a target in a given distance.

    [SerializeField]
    float cameraDistance;
    Transform target;

    // Use this for initialization
    void Start()
    {
        // detach from player, we want our own movement here
        transform.parent = null;
    }

    public void setTarget(Transform _target)
    {
        // we dont move our object with the player but we still need it's transform to know the players position
        this.target = _target;
    }

    void FixedUpdate()
    {
        // if the target is set, follow it in a given cameraDistance
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, target.position.z - cameraDistance);
        }
    }
}
