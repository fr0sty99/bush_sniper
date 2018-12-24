using UnityEngine;
using UnityEngine.Networking;
using System;

[RequireComponent(typeof(Player))]
public class PlayerSetup : MonoBehaviour
{

    [SerializeField]
    Camera followCamera;
    Camera sceneCamera;

    void Start()
    {
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
