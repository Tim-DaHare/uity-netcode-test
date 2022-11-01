using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace Behaviors
{
    public class VotingSystem : NetworkBehaviour
    {
        private readonly NetworkVariable<bool> _netIsVotingAllowed = new();
        private readonly Dictionary<ulong, ulong> _votes = new();

        public bool IsVotingAllowed => _netIsVotingAllowed.Value;

        public override void OnNetworkSpawn()
        {
            GameManger.Singleton.GameTimeManager.OnNightTimeStart += OnNightTimeStart;
            GameManger.Singleton.GameTimeManager.OnNightTimeEnd += OnNightTimeEnd;
        }

        public override void OnNetworkDespawn()
        {
            GameManger.Singleton.GameTimeManager.OnNightTimeStart -= OnNightTimeStart;
            GameManger.Singleton.GameTimeManager.OnNightTimeEnd -= OnNightTimeEnd;
        }

        private void OnNightTimeStart()
        {
            if (!NetworkManager.IsServer) return;
            
            // kill voted player
            var saaf = GameManger.Singleton.VotingSystem.GetClientIdWithMostVotes();
            if (saaf != null)
                NetworkManager.Singleton.ConnectedClients[(ulong)saaf].PlayerObject.GetComponent<Player>().Die();

            _votes.Clear();

            _netIsVotingAllowed.Value = false;
        }
        
        private void OnNightTimeEnd()
        {
            if (!NetworkManager.IsServer) return;
            _netIsVotingAllowed.Value = true;
        }

        public Dictionary<ulong, int> GetVoteCount()
        {
            var voteCount = new Dictionary<ulong, int>();

            foreach (var vote in _votes)
                if (!voteCount.ContainsKey(vote.Value))
                    voteCount.Add(vote.Value, 1);
                else
                    voteCount[vote.Value]++;
            
            return voteCount;
        }

        public ulong? GetClientIdWithMostVotes()
        {
            var voteCount = GetVoteCount();
            if (!voteCount.Any()) return null;

            var highestVoteCount = voteCount.Max(kv => kv.Value);
            var highestVotesCandidates = voteCount.Where(kv => kv.Value == highestVoteCount).ToList();

            if (highestVotesCandidates.Count() > 1)
                return null; // If there are more multiple players with the same amount of votes against them, then there can not be a fair candidate
            
            return highestVotesCandidates.First().Key;
        }

        public void SubmitVote(ulong votedOnClientId)
        {
            if (NetworkManager.IsServer)
                HandleVote(NetworkManager.LocalClientId, votedOnClientId);
            else
                SubmitVoteServerRpc(votedOnClientId);
        }

        private void HandleVote(ulong voterClientId, ulong votedOnClientId)
        {
            if (!IsServer) return; // Only server can handle votes
            
            if (!_votes.ContainsKey(voterClientId))
                _votes.Add(voterClientId, votedOnClientId);
            else
                _votes[voterClientId] = votedOnClientId;
        }
        
        [ServerRpc]
        private void SubmitVoteServerRpc(ulong votedOnClientId, ServerRpcParams rpcParams = default)
        {
            var senderId = rpcParams.Receive.SenderClientId;
            HandleVote(senderId, votedOnClientId);
        }
    }
}
