using Enums;
using Unity.Netcode;
using UnityEngine;

namespace Classes
{
    public class DetectiveRole : PlayerRole
    {
        public override PlayerRoles Role => PlayerRoles.Detective;
        public override PlayerTeams Team => PlayerTeams.Innocent;
        public override Color Color => Color.blue;
        public override void UseAbility()
        {
            throw new System.NotImplementedException();
        }

        public override void UseAbilityServer(ulong senderClientId = default, FastBufferReader reader = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
