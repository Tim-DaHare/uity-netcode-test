using System.Collections.Generic;
using System.Linq;
using Enums;

namespace Classes
{
    public static class PlayerRoleMapping
    {
        public static readonly Dictionary<PlayerRoles, PlayerRole> Mapping = new()
        {
            {PlayerRoles.Unassigned, null},
            {PlayerRoles.Civilian, new CivilianRole()},
            {PlayerRoles.Detective, new DetectiveRole()},
            {PlayerRoles.Killer, new KillerRole()},
        };

        public static PlayerRole[] GetRolesForTeam(PlayerTeams team)
        {
            return Mapping.Where(kv => kv.Value.Team == team)
                .Select(skv => skv.Value)
                .ToArray();
        } 
    }
}
