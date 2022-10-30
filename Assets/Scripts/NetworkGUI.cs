using Classes;
using Unity.Netcode;
using UnityEngine;

public class NetworkGUI : MonoBehaviour
{
    private static LobbyManager LobbyManager => GameManger.Singleton.LobbyManager;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
            LobbyControls();
        }
        
        GUILayout.EndArea();
    }

    private static void LobbyControls()
    {
        if (LobbyManager.IsMatchStarted) return;

        if (NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Start Match")) GameManger.Singleton.LobbyManager.StartMatch();
        }
        
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Stop Server" : "Leave lobby")) NetworkManager.Singleton.Shutdown();
    }

    private static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    private static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
    
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}