using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]     // This script requires the GameObject to have a Rigidbody2D component
public class PlayerMotor : MonoBehaviour
{

    private Vector2 velocity = Vector2.zero;
    private float rotationAngle = 0.0f;
    private Rigidbody2D rb;
    private bool moving;

    void Start()
    {
        // we don't need to check for errors here, because we use RequireComponent
        rb = GetComponent<Rigidbody2D>();
    }

    // Gets a movement vector
    public void Move(Vector2 _velocity, bool _moving)
    {
        velocity = _velocity;
        moving = _moving;
    }

    // Gets a rotation Vector
    public void Rotate(float _rotationAngle)
    {
        rotationAngle = _rotationAngle;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on velocity variable
    void PerformMovement()
    {
        if (velocity != (Vector2.zero))
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    void PerformRotation()
    {
        rb.rotation = rotationAngle;
        Debug.Log("PerformRotation -- MoveRotation: " + rotationAngle);
    }
}
