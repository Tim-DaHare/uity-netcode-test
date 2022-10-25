using Enums;
using Unity.Netcode;
using UnityEngine;

namespace Classes
{
    public struct KillerAbilityNetData : INetworkSerializable
    {
        public const string MessageName = "killer_ability";
        
        public Vector3 Direction;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            
        }
    }

    public class KillerRole : PlayerRole
    {
        public override PlayerRoles Role => PlayerRoles.Killer;
        public override PlayerTeams Team => PlayerTeams.Evil;
        public override Color Color => Color.red;

        public override void UseAbility()
        {
            
        }
    }
}
