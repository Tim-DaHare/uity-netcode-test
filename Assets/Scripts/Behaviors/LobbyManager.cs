using System;
using System.Linq;
using Classes;
using Enums;
using NetTypes;
using Unity.Netcode;
using Random = UnityEngine.Random;

namespace Behaviors
{
    public class LobbyManager : NetworkBehaviour
    {
        private NetworkList<NetPlayerLobbyData> _netLobbyPlayers;
        private readonly NetworkVariable<float> _matchStartedAt = new(-1);
        
        public NetworkList<NetPlayerLobbyData> LobbyPlayers => _netLobbyPlayers;
        public bool IsMatchStarted => _matchStartedAt.Value >= 0;
        public float MatchStartedAt => _matchStartedAt.Value;
        
        private void Awake()
        {
            _netLobbyPlayers = new NetworkList<NetPlayerLobbyData>();
        }

        public override void OnNetworkSpawn()
        {
            _netLobbyPlayers.OnListChanged += OnChangeConnectedPlayers;
            
            if (!IsServer) return; // Only the server can handle connects and disconnects

            NetworkManager.OnServerStarted += OnServerStarted;
            
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

            Player.OnPlayerDeath += OnPlayerDeath;
        }

        public override void OnNetworkDespawn()
        {
            _netLobbyPlayers.OnListChanged -= OnChangeConnectedPlayers;
            
            if (!IsServer) return;
            
            NetworkManager.OnServerStarted -= OnServerStarted;
            
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
            
            Player.OnPlayerDeath -= OnPlayerDeath;
        }
        
        private void OnServerStarted()
        {
            _netLobbyPlayers.Add(new NetPlayerLobbyData(NetworkManager.LocalClientId));
        }

        private void OnClientConnected(ulong connectedClientId)
        {
            _netLobbyPlayers.Add(new NetPlayerLobbyData(connectedClientId));
        }

        private void OnClientDisconnected(ulong disconnectedClientId)
        {
            _netLobbyPlayers.Remove(new NetPlayerLobbyData(disconnectedClientId));
        }

        private void OnChangeConnectedPlayers(NetworkListEvent<NetPlayerLobbyData> changeEvent)
        {
            if (!IsServer || changeEvent.Type != NetworkListEvent<NetPlayerLobbyData>.EventType.Add) return;
            
            // Handle renaming of player objects for editor hierarchy
            var connectedClientId = changeEvent.Value.ClientId;
            NetworkManager.SpawnManager.GetPlayerNetworkObject(connectedClientId).name = "Player " + connectedClientId;
        }
        
        private void OnPlayerDeath(ulong clientId)
        {
            var winningTeam = GetWinningTeam();
            if (winningTeam == null) return;

            print("Winning Team: " + winningTeam);
            
            // Reset lobby
            ResetLobby();
        }

        public void ResetLobby()
        {
            if (!IsServer) return; // Only server can reset the lobby data
            
            // Reset all the lobby fields back to the non-started state
            _matchStartedAt.Value = -1;
            
            // TODO: respawn players
        }

        private static PlayerTeams? GetWinningTeam()
        {
            var remainingTeams = NetworkManager.Singleton.GetPlayers()
                .Where(p => p.IsAlive)
                .Select(p => p.Role.Team)
                .Distinct()
                .ToArray();

            if (remainingTeams.Length > 1) return null;

            if (!remainingTeams.Any())
                throw new Exception("There are no teams left in the game");

            return remainingTeams.First();
        }

        private void AssignRolesToPlayers()
        {
            var randKillerIndex = Random.Range(0, NetworkManager.ConnectedClients.Keys.Count());
            var killerClientId = NetworkManager.ConnectedClients.Keys.ElementAt(randKillerIndex);
            
            foreach (var connectedClient in NetworkManager.ConnectedClients)
            {
                if (!connectedClient.Value.PlayerObject.TryGetComponent<Player>(out var iPlayer))
                    continue;

                if (connectedClient.Key == killerClientId)
                {
                    iPlayer.SetPlayerRole(PlayerRoles.Killer);
                    continue;
                }
                
                iPlayer.SetPlayerRole(PlayerRoles.Civilian);
            }
        }

        public void StartMatch()
        {
            if (!IsServer || NetworkManager.ConnectedClients.Count == 0) return;
            
            _matchStartedAt.Value = NetworkManager.ServerTime.TimeAsFloat;
            AssignRolesToPlayers();
        }
    }
}
