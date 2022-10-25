using Enums;

namespace Classes
{
    public abstract class PlayerRole
    {
        protected PlayerRole(PlayerTeams team)
        {
            Team = team;
        }

        public PlayerTeams Team { get; }
    }
}
