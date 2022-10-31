using System.Linq;
using Unity.Netcode;
using Enums;
using URandom = UnityEngine.Random;

namespace Classes
{
    public class LobbyManager : NetworkBehaviour
    {
        private readonly NetworkVariable<double> _matchStartedAt = new(-1);

        public bool IsMatchStarted => _matchStartedAt.Value >= 0;
        public double MatchStartedAt => _matchStartedAt.Value;
        
        public void StartMatch()
        {
            if (!IsServer || NetworkManager.ConnectedClients.Count == 0) return;
            
            _matchStartedAt.Value = NetworkManager.ServerTime.Time;
            AssignRolesToPlayers();
        }
        
        private void AssignRolesToPlayers()
        {
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
