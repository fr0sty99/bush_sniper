using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Player))]
public class PlayerSetup : MonoBehaviour
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
       /* used for multiplayer  
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

        GetComponent<Player>().Setup();

        if (!isServer)
        {
            DisableMapGenerator();
            getAndSpawnMap();
        }
        */

        // change from MainCamera to followCamera
        sceneCamera = Camera.main;
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(false);
        }

        // give the followCamera its target
        followCamera.GetComponent<FollowCamera>().setTarget(transform);
    }

}
