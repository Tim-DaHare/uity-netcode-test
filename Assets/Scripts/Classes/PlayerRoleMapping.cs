using System.Collections.Generic;
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
    }
}
