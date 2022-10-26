using Classes;
using Unity.Netcode;
using UnityEngine;

public class NetworkGUI : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();

            RolePicker();
        }
    
        GUILayout.EndArea();
    }

    private void RolePicker()
    {
        foreach (var pair in PlayerRoleMapping.Mapping)
        {
            if (pair.Value != null && GUILayout.Button(pair.Value.ToString()))
            {
                var localPlayerObj = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                if (!localPlayerObj || !localPlayerObj.TryGetComponent<Player>(out var player)) return;
                
                player.ChangeRoleServerRpc(pair.Key);
            }
        }
    }

    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }
    
    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
    
        GUILayout.Label("Transport: " +
                        NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }
}