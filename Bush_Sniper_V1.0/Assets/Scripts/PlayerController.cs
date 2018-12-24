using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]     // This script requires the Gameobject to have a PlayerMotor component
public class PlayerController : MonoBehaviour
{
    // This class is responsible for the player input and informing the PlayerMotor what movements to perform next

    [SerializeField]    
    private float movementSpeed;

    [SerializeField]
    private Camera followCam;
    private PlayerMotor motor;

    void Start()
    {
        // no checking for errors needed because we use RequireComponent
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        Vector2 _moveVertical = Vector2.zero;
        Vector2 _moveHorizontal = Vector2.zero;

        // add velocity if user is pressing buttons
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            _moveHorizontal = Vector2.left;
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            _moveHorizontal = Vector2.right;
        }
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            _moveVertical = Vector2.down;
        }
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            _moveVertical = Vector2.up;
        }

        // remove velocity if the user stops pressing buttons
        if (Input.GetKeyUp(KeyCode.A))
        {
            _moveHorizontal = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _moveVertical = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            _moveVertical = Vector2.zero;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _moveHorizontal = Vector2.zero;
        }

        // add up velocities and multiply with speed
        Vector2 _velocity = (_moveVertical + _moveHorizontal) * movementSpeed;

        // apply movement
        motor.Move(_velocity);

        // create a vector "diff" from the mouse to the player
        Vector2 _mousePos = followCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = _mousePos - (Vector2)transform.position;

        // shorten the vector to 1
        diff.Normalize();

        // get the angle in float from the vector diff
        float _rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // apply rotation
        motor.Rotate(_rotation);
    }

}
