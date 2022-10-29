using Enums;
using Unity.Netcode;
using UnityEngine;

namespace Classes
{
    public class CivilianRole : PlayerRole
    {
        public override PlayerRoles Role => PlayerRoles.Civilian;
        public override PlayerTeams Team => PlayerTeams.Innocent;
        public override Color Color => Color.green;
        public override string RoleName => "Civilian";

        public override void UseAbility()
        {
        }

        public override void UseAbilityMessage(ulong senderClientId = default, FastBufferReader reader = default)
        {
        }
    }
}
