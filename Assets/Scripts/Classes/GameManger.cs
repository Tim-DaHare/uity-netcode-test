using UnityEngine;

namespace Classes
{
    public class GameManger : MonoBehaviour
    {
        public static GameManger Singleton;
    
        [SerializeField] private LobbyManager lobbyManager;
        public LobbyManager LobbyManager => lobbyManager;

        private void Awake()
        {
            if (Singleton != null) Destroy(gameObject);

            Singleton = this;
        }
    }
}
