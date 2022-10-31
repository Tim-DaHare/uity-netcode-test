using Behaviors;
using UnityEngine;
using Unity.Netcode;
using Classes;

public class NetworkGUI : MonoBehaviour
{
    private static GameManger GameManger => GameManger.Singleton;
    private static Player LocalPlayer => NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
    
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

            if (GameManger.LobbyManager.IsMatchStarted)
            {
                GUILayout.Label("Role: " + LocalPlayer.Role.RoleName);
                Clock();
            }
        }
        
        GUILayout.EndArea();
    }

    private static void Clock()
    {
        GUILayout.Label("Time data: ");
        GUILayout.Label("Match time: " + GameManger.GameTimeManager.MatchTime);
        GUILayout.Label("Day time: " + GameManger.GameTimeManager.CurrentDaytime);
        GUILayout.Label("Day time ratio: " + GameManger.GameTimeManager.DayTimeRatio);
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

    private static void LobbyControls()
    {
        if (NetworkManager.Singleton.IsServer && !GameManger.LobbyManager.IsMatchStarted)
        {
            if (GUILayout.Button("Start Match")) GameManger.Singleton.LobbyManager.StartMatch();
        }
        
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Stop Server" : "Leave lobby")) NetworkManager.Singleton.Shutdown();
    }
}