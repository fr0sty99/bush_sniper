using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]     // This script requires the Gameobject to have a PlayerMotor component
public class PlayerController : MonoBehaviour {
    
    [SerializeField]    // When you mark a variable with "SerializeField", it will show up in the inspector, even tho it's a private variable
    private float speed = 5f;


    private PlayerMotor motor;

	void Start()
	{
        // no checking for errors needed because we use RequireComponent
        motor = GetComponent<PlayerMotor>();
	}

	void Update()
	{
		// Calculate movement velocity as a 2D Vector

        // get position of mouse inside of World, we have to convert it with screenToWorldPoint f.e.
        // get position of player

        // move player according to input (A,W,S,D)
        // rotate gun according to mouse position (angle and position)

	}
}
