using UnityEngine.Networking;

public class NetworkServerRelay : NetworkMessageHandler {
     private void Start()
    {
        if(isServer)
        {
            RegisterNetworkMessages();
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler(movement_msg, OnReceivePlayerMovementMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();
        NetworkServer.SendToAll(movement_msg, _msg);
    }
}
