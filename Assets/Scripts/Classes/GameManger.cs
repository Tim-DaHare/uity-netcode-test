using UnityEngine;

namespace Classes
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Singleton;
    
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private GameTimeManager gameTimeManager;
        public LobbyManager LobbyManager => lobbyManager;
        public GameTimeManager GameTimeManager => gameTimeManager;

        private void Awake()
        {
            if (Singleton != null) Destroy(gameObject);

            Singleton = this;
        }
    }
}
