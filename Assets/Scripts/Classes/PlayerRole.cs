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
        public abstract string RoleName { get; }
        protected static Player LocalPlayer => NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        public abstract void UseAbility();
        public abstract void UseAbilityMessage(ulong senderClientId = default, FastBufferReader reader = default);

        public override string ToString() => RoleName;
    }
}
