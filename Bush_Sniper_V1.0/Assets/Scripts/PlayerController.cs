using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]     // This script requires the Gameobject to have a PlayerMotor component
public class PlayerController : MonoBehaviour
{
    [SerializeField]    // When you mark a variable with "SerializeField", it will show up in the inspector, even tho it's a private variable
    private float speed = 10f;

    private PlayerMotor motor;

    void Start()
    {
        // no checking for errors needed because we use RequireComponent
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // Calculate movement velocity as a 2D Vector

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
        Vector2 _velocity = (_moveVertical + _moveHorizontal) * speed;

        // apply movement
        motor.Move(_velocity);


        // Calculatre rotation as a 2D Vector
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = _mousePos - (Vector2)transform.position;
        diff.Normalize();
        float _rotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // apply rotation
        motor.Rotate(_rotation);




    }
}
