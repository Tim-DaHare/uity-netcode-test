using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace Classes
{
    public static class ExtensionMethods
    {
        public static Player GetPlayer(this NetworkManager netManager, ulong clientId)
        {
            return NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<Player>();
        }
        
        /// <summary>
        /// (Server Only) Gets the player components from all connected clients
        /// </summary>
        /// <param name="netManager"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static IEnumerable<Player> GetPlayers(this NetworkManager netManager)
        {
            if (!NetworkManager.Singleton.IsServer) return null;
            
            return NetworkManager.Singleton.ConnectedClients
                .Select(c => 
                    c.Value.PlayerObject.GetComponent<Player>())
                .ToArray();
        }
        
    }
}
