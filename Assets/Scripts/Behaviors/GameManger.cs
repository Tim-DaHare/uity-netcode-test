using UnityEngine;

namespace Behaviors
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Singleton;
    
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private GameTimeManager gameTimeManager;
        [SerializeField] private VotingSystem votingSystem;
        public LobbyManager LobbyManager => lobbyManager;
        public GameTimeManager GameTimeManager => gameTimeManager;
        public VotingSystem VotingSystem => votingSystem;

        private void Awake()
        {
            if (Singleton != null) Destroy(gameObject);
            
            Singleton = this;
        }
    }
}
