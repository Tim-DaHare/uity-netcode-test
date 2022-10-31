using System.Collections.Generic;
using Unity.Netcode;

namespace Behaviors
{
    public class VotingSystem : NetworkBehaviour
    {
        private Dictionary<ulong, int> _votes = new();
        
        public void GetVoteCount()
        {
            
        }

        public void AddVote()
        {
            
        }

        [ServerRpc]
        public void AddVoteServerRpc(ulong voteeClientId)
        {
            if (!_votes.ContainsKey(voteeClientId))
            {
                _votes.Add(voteeClientId, 1);
            }
            else
            {
                
            }
        }
    }
}
