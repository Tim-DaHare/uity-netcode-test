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
        public override string RoleName => "Detective";

        public override void UseAbility()
        {
        }

        public override void UseAbilityMessage(ulong senderClientId = default, FastBufferReader reader = default)
        {
        }
    }
}
