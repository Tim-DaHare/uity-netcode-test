using System.Linq;
using Enums;
using Unity.Netcode;
using Random = UnityEngine.Random;

namespace Behaviors
{
    public class LobbyManager : NetworkBehaviour
    {
        private readonly NetworkVariable<float> _matchStartedAt = new(-1);
        public bool IsMatchStarted => _matchStartedAt.Value >= 0;
        public float MatchStartedAt => _matchStartedAt.Value;
        
        public void StartMatch()
        {
            if (!IsServer || NetworkManager.ConnectedClients.Count == 0) return;
            
            _matchStartedAt.Value = NetworkManager.ServerTime.TimeAsFloat;
            AssignRolesToPlayers();
        }
        
        private void AssignRolesToPlayers()
        {
            var randKillerIndex = Random.Range(0, NetworkManager.ConnectedClients.Keys.Count());
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
