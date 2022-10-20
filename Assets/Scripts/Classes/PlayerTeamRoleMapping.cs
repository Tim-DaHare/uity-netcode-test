using System.Collections.Generic;
using Enums;

namespace Classes
{
    public static class PlayerTeamRoleMapping
    {
        public static readonly Dictionary<PlayerTeams, PlayerRole> Mapping = new()
        {
            {PlayerTeams.Innocent, new CivilianRole(PlayerTeams.Innocent)},
            {PlayerTeams.Evil, new KillerRole(PlayerTeams.Evil)}
        };
    }
}
