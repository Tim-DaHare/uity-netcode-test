using Classes;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Singleton;
    
    [SerializeField] private LobbyManager _lobbyManager;
    public LobbyManager LobbyManager => _lobbyManager;

    private void Awake()
    {
        if (Singleton != null) Destroy(gameObject);

        Singleton = this;
    }
}
