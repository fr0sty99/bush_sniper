using UnityEngine;

public class CamerScript : MonoBehaviour
{
    public Transform target;
    Camera mycam;

    void Start()
    {
        mycam = GetComponent<Camera>();

    }
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, 0.1f) + new Vector3(0, 0, -10);
        }
    }
}
