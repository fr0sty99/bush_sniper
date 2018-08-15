using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]     // This script requires the GameObject to have a Rigidbody2D component
public class PlayerMotor : MonoBehaviour {

    private Vector2 velocity = Vector3.zero;

    private Rigidbody2D rb;

	void Start()
	{
        // we don't need to check for errors here, because we use RequireComponent
        rb = GetComponent<Rigidbody2D>();
	}

    public void Move (Vector2 _velocity) {
        
    }

}
