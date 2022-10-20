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
    
            // SubmitNewPosition();

            RolePicker();
        }
    
        GUILayout.EndArea();
    }

    private void RolePicker()
    {
        foreach (var pair in PlayerTeamRoleMapping.Mapping)
        {
            if (GUILayout.Button(pair.Value.ToString()))
            {
                ChangeRoleServerRpc(pair.Value);
            }
        }
    }

    [ServerRpc]
    private void ChangeRoleServerRpc(PlayerRole role)
    {
        ChangeRoleClientRpc(role);
    }

    [ClientRpc]
    private void ChangeRoleClientRpc(PlayerRole role)
    {
        if (!NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().TryGetComponent<Player>(out var player)) return;

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
    
    
    
    // static void SubmitNewPosition()
    // {
    //     if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
    //     {
    //         // if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
    //         // {
    //         //     foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
    //         //         NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
    //         // }
    //         // else
    //         // {
    //             var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
    //             var player = playerObject.GetComponent<Player>();
    //             player.Move();
    //         // }
    //     }
    // }
}