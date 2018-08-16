using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]     // This script requires the Gameobject to have a PlayerMotor component
public class PlayerController : MonoBehaviour
{

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
        // get user input
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _yMov = Input.GetAxisRaw("Vertical");

        // calculate seperated x- and y- axis velocity
        Vector2 _moveVertical = transform.up * _yMov;
        Vector2 _moveHorizontal = transform.right * _xMov;

        // calculate final movement vector and make it a normalized magnitude, which means the length of the Vector will always be 1, before multiplying it with the players speed
        Vector2 _velocity = (_moveHorizontal + _moveVertical).normalized * speed;

        // apply movement
        motor.Move(_velocity);

        // Calculatre rotation as a 2D Vector
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("mousePos: " + _mousePos);

        Vector2 _playerPos = transform.position;
        Debug.Log("playerPos: " + _playerPos);

        // for calculating the angle between the two points we need this formula from https://www.mathworks.com/matlabcentral/answers/180131-how-can-i-find-the-angle-between-two-vectors-including-directional-information
        // angle = atan2d(x*_mousePos.y-_playerPos.y*_mousePos.x,_playerPos.x*_mousePos.x+_playerPos.y*_mousePos.y);
        float angle = Mathf.Atan2(_playerPos.x * _mousePos.y - _playerPos.y * _mousePos.x, _playerPos.x * _mousePos.x + _playerPos.y * _mousePos.y);

        Debug.Log("RotationAngle: " + angle);
        motor.Rotate(angle);


        // TODO: move gun and change it's angle relative to playerPosition and mousePosition     
        // get position of mouse inside of World, we have to convert it with screenToWorldPoint f.e.
        // get position of player
        // rotate gun according to mouse position (angle and position)

    }
}
