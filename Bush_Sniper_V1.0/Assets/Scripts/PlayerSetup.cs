using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    GameObject followCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            GameObject go = Instantiate(followCamera, new Vector3(transform.position.x, transform.position.y, transform.position.z - 10.0f), Quaternion.identity);
            go.GetComponent<FollowCamera>().setTarget(transform);
        }

    }

}
