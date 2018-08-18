using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    Camera followCamera;
    Camera sceneCamera;

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
            sceneCamera = Camera.main;
            if(sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }


            followCamera.GetComponent<FollowCamera>().setTarget(transform);
        }
    }

    void onDisable() // gets also called when the object is destroyed
    {
        if(sceneCamera != null) {
            sceneCamera.gameObject.SetActive(true);
        }
    }

}
