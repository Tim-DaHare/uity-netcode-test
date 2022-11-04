using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using Enums;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

namespace Classes
{
    public class KillerRole : PlayerRole
    {
        public override PlayerRoles Role => PlayerRoles.Killer;
        public override PlayerTeams Team => PlayerTeams.Evil;
        public override Color Color => Color.red;
        public override string RoleName => "Killer";

        /// <summary>
        /// This method allows the player to use his role ability
        /// </summary>
        public override void UseAbility()
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                var abilityBuffer = GetAbilityDataBuffer();
                
                using (abilityBuffer)
                    NetworkManager.Singleton.CustomMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId, abilityBuffer);
            }
            // if is server
            else AbilityLogic(LocalPlayer.PlayerCamera.transform.forward, LocalPlayer);
        }

        public override void UseAbilityMessage(ulong senderClientId = default, FastBufferReader reader = default)
        {
            reader.ReadValueSafe(out Vector3 lookDir);
            
            var abilityPlayer = NetworkManager.Singleton.ConnectedClients[senderClientId].PlayerObject.GetComponent<Player>();
            
            AbilityLogic(lookDir, abilityPlayer);
        }
        
        private static void AbilityLogic(Vector3 aimDir, Player abilityPlayer)
        {
            var origin = abilityPlayer.PlayerCamera.transform.position;
            
            var ray = new Ray(origin, aimDir);
            if (!Physics.Raycast(ray, out var hitInfo, 1)) return;
            
            if (!hitInfo.transform.TryGetComponent<Player>(out var hitPlayer)) return;
            hitPlayer.Die();

            // hitPlayerTransform.SetState(Vector3.zero, shouldGhostsInterpolate: false);
        }
        
        private static FastBufferWriter GetAbilityDataBuffer()
        {
            var facingDir = LocalPlayer.PlayerCamera.transform.forward;            
            var writer = new FastBufferWriter(sizeof(float) * 3, Allocator.Temp);
            
            writer.WriteValueSafe(facingDir);
            
            return writer;
        }
    }
}
