using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{
    // This class is responsible for dis- and enabling components from other players in multiplayer.

    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    Camera followCamera;
    Camera sceneCamera;

    void Start()
    {
        // if this object is not the clients player, disable the choosen components
        if (!isLocalPlayer) 
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            // change from MainCamera to followCamera
            sceneCamera = Camera.main;
            if(sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }

            // give the followCamera it's target
            followCamera.GetComponent<FollowCamera>().setTarget(transform);
        }
    }
    s
    void onDisable() // gets also called when the object is destroyed
    {
        // switch from followCamera to SceneCamera when playerObject is destroyed
        if(sceneCamera != null && followCamera != null) {
            sceneCamera.gameObject.SetActive(true);
            followCamera.gameObject.SetActive(false);
        }
    }

}
