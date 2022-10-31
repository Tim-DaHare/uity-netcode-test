using Unity.Netcode;

namespace Behaviors
{
    public class AbilityMessageHandler : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            NetworkManager.CustomMessagingManager.OnUnnamedMessage += ReceiveMessage;
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.CustomMessagingManager.OnUnnamedMessage -= ReceiveMessage;
        }

        private void ReceiveMessage(ulong senderClientId, FastBufferReader reader)
        {
            if (!IsServer) return;
            
            var playerObject = NetworkManager.ConnectedClients[senderClientId].PlayerObject;
            var sendingPlayer = playerObject.GetComponent<Player>();
            
            sendingPlayer.Role.UseAbilityMessage(senderClientId, reader);
        }
    }
}
