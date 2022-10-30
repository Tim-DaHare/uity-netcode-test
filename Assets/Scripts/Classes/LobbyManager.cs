using System.Linq;
using Enums;
using Unity.Netcode;
using URandom = UnityEngine.Random;

namespace Classes
{
    public class LobbyManager : NetworkBehaviour
    {
        public bool IsMatchStarted { get; private set; }
        
        public void StartMatch()
        {
            if (NetworkManager.ConnectedClients.Count == 0) return;
            
            IsMatchStarted = true;
            AssignRolesToPlayers();
        }
        
        private void AssignRolesToPlayers()
        {
            if (!IsServer) return; // Only server can assign roles to players

            var randKillerIndex = URandom.Range(0, NetworkManager.ConnectedClients.Keys.Count());
            var killerClientId = NetworkManager.ConnectedClients.Keys.ElementAt(randKillerIndex);
            
            foreach (var connectedClient in NetworkManager.ConnectedClients)
            {
                var iPlayer = connectedClient.Value.PlayerObject.GetComponent<Player>();

                if (connectedClient.Key == killerClientId)
                {
                    iPlayer.SetPlayerRole(PlayerRoles.Killer);
                    continue;
                }
                
                iPlayer.SetPlayerRole(PlayerRoles.Civilian);
            }
        }
    }
}
