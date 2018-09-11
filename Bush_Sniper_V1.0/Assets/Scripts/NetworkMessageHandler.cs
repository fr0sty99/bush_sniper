using UnityEngine;
using UnityEngine.Networking;

public class NetworkMessageHandler : NetworkBehaviour {

    public const short movement_msg = 1337;

    public class PlayerMovementMessage : MessageBase
    {
        public string objectTransformName;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public float time;
    }
}
