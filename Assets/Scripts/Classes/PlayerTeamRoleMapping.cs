using System.Collections.Generic;
using Enums;

namespace Classes
{
    public enum PlayerRoles : byte
    {
        Civilian,
        Killer,
    }
    
    public static class PlayerTeamRoleMapping
    {
        public static readonly Dictionary<PlayerRoles, PlayerRole> Mapping = new()
        {
            {PlayerRoles.Civilian, new CivilianRole(PlayerTeams.Innocent)},
            {PlayerRoles.Killer, new KillerRole(PlayerTeams.Evil)}
        };
    }
}
