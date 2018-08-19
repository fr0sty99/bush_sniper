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

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    void Start()
    {
        // if this object is not the clients player, disable the choosen components
        if (!isLocalPlayer) 
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {   // this object is the local player
            
            // change from MainCamera to followCamera
            sceneCamera = Camera.main;
            if(sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }

            // give the followCamera its target
            followCamera.GetComponent<FollowCamera>().setTarget(transform);
        }
    }

    void onDisable() // gets also called when the object is destroyed
    {
        // switch from followCamera to SceneCamera when playerObject is destroyed
        if(sceneCamera != null && followCamera != null) {
            sceneCamera.gameObject.SetActive(true);
            followCamera.gameObject.SetActive(false);
        }
    }

    void AssignRemoteLayer() {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents() {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
}
