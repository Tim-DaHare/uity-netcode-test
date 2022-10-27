using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using Enums;

namespace Classes
{
    public class KillerRole : PlayerRole
    {
        public override PlayerRoles Role => PlayerRoles.Killer;
        public override PlayerTeams Team => PlayerTeams.Evil;
        public override Color Color => Color.red;
        
        /// <summary>
        ///     This method allows the player to use his role ability
        /// </summary>
        /// <param name="senderClientId">The Client Id of the client that send the message to the server (only available on server)</param>
        /// <param name="reader">Buffer containing the data (only available on server)</param>
        public override void UseAbility(ulong senderClientId = default, FastBufferReader reader = default)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                var abilityBuffer = GetAbilityDataBuffer();
                
                using (abilityBuffer)
                    NetworkManager.Singleton.CustomMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, abilityBuffer);
            }
            // if is server
            else
            {
                reader.ReadValueSafe(out Vector3 lookDir);
                
                var playerObject = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject;
                var sendingPlayer = playerObject.GetComponent<Player>();
                
                var origin = sendingPlayer.PlayerCamera.transform.position;
                
                var ray = new Ray(origin, lookDir);
                if (!Physics.Raycast(ray, out var hitInfo, 1)) return;
                
                if (!hitInfo.transform.TryGetComponent<Player>(out var hitPlayer)) return;
                
                hitPlayer.transform.position = Vector3.zero;
            }
        }

        private FastBufferWriter GetAbilityDataBuffer()
        {
            var localPlayer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            
            var facingDir = localPlayer.PlayerCamera.transform.forward;            
            var writer = new FastBufferWriter(sizeof(float) * 3, Allocator.Temp);

            writer.WriteValueSafe(facingDir);
            
            return writer;
        }
    }
}
