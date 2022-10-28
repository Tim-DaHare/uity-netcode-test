using Enums;
using Unity.Netcode;
using UnityEngine;

namespace Classes
{
    public abstract class PlayerRole
    {
        public abstract PlayerRoles Role { get; }
        public abstract PlayerTeams Team { get; }
        public abstract Color Color { get; }
        public abstract void UseAbility();
        public abstract void UseAbilityServer(ulong senderClientId = default, FastBufferReader reader = default);
    }
}
