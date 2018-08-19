using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{

    public PlayerWeapon weapon;
    [SerializeField]
    private LayerMask mask;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // transform.right means forward in our case. the red axis is the axis which our player is facing

        Vector2 _startPos = transform.position;
        Vector2 _destPos = transform.right * weapon.range;

        RaycastHit2D _hit = Physics2D.Raycast(_startPos, _destPos, weapon.range, mask);
        Debug.DrawRay(_startPos, _destPos, Color.red);

        if (_hit)
        {
            // we hit something
            Debug.Log("Yout hit: " + _hit.collider.name);
        }
    }

}
