using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Player))]
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
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            // give the followCamera its target
            followCamera.GetComponent<FollowCamera>().setTarget(transform);

            GetComponent<Player>().SetupPlayer();

        }
    }
    // this method belongs to the NetworkBehaviour class
	public override void OnStartClient()
	{
        base.OnStartClient();

        // moved to PlayerManager
        //string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        //Player _player = GetComponent<Player>(); // Player is required in this class

        //PlayerManager.RegisterPlayer(_netId, _player);
	}

	void onDisable() // gets also called when the object is destroyed
    {
        // switch from followCamera to SceneCamera when playerObject is destroyed
        if(isLocalPlayer) {
            if (sceneCamera != null && followCamera != null)
            {
                sceneCamera.gameObject.SetActive(true);
                followCamera.gameObject.SetActive(false);
            } 
        }
    
        // unregister player

        // moved to PlayerManager
        //PlayerManager.UnregisterPlayer(transform.name);
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
