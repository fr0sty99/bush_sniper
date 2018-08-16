using UnityEngine;
using System;

[RequireComponent(typeof(PlayerMotor))]     // This script requires the Gameobject to have a PlayerMotor component
public class PlayerController : MonoBehaviour
{

    [SerializeField]    // When you mark a variable with "SerializeField", it will show up in the inspector, even tho it's a private variable
    private float speed = 5f;
    private bool moving = false;


    private PlayerMotor motor;

    void Start()
    {
        // no checking for errors needed because we use RequireComponent
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // Calculate movement velocity as a 2D Vector
        // get user input

        Vector2 _moveVertical = Vector2.zero;
        Vector2 _moveHorizontal = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            _moveHorizontal = -transform.right;
            moving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _moveVertical = -transform.up;
            moving = true;
        }

        if (Input.GetKey(KeyCode.W))
        {
            _moveVertical = transform.up;
            moving = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            _moveHorizontal = transform.right;
            moving = true;
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            _moveHorizontal = Vector2.zero;
            moving = false;
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            _moveVertical = Vector2.zero;
            moving = false;

        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            _moveVertical = Vector2.zero;
            moving = false;

        }
        if (Input.GetKey(KeyCode.D))
        {
            _moveHorizontal = Vector2.zero;
            moving = false;
        }

        Vector2 _velocity = (_moveVertical + _moveHorizontal) * speed;

      //  float _xMov = Input.GetAxisRaw("Horizontal");
     //   float _yMov = Input.GetAxisRaw("Vertical");

        // calculate seperated x- and y- axis velocity
        // Vector2 _moveVertical = transform.up * _yMov;
        // Vector2 _moveHorizontal = transform.right * _xMov;

        // calculate final movement vector and make it a normalized magnitude, which means the length of the Vector will always be 1, before multiplying it with the players speed
      //  Vector2 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

        // apply movement
        motor.Move(_velocity, moving);


        // Calculatre rotation as a 2D Vector
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = _mousePos - (Vector2) transform.position;
        diff.Normalize();
        float _rotZ = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        // apply rotation
        motor.Rotate(_rotZ);


        // TODO: move gun and change it's angle relative to playerPosition and mousePosition     
        // get position of mouse inside of World, we have to convert it with screenToWorldPoint f.e.
        // get position of player
        // rotate gun according to mouse position (angle and position)

    }
}
