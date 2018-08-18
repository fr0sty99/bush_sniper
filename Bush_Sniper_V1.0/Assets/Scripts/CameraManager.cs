using UnityEngine;

public class CameraManager : MonoBehaviour {
    private GameObject player;       //Public variable to store a reference to the player game object
    public float distanceToTarget;
    // Use this for initialization
    void Start()
    {
        
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        if(player != null) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - distanceToTarget);
        }
    }

    public void setTarget(GameObject _player) {
        this.player = _player;
    }

}
