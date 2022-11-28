using System;
using Behaviors;
using UnityEngine;
using Unity.Netcode;
using Classes;
using Enums;

public class Player : NetworkBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Renderer capsuleRenderer;
    [SerializeField] private NetworkObject networkObject;
    
    private PlayerMovement _playerMovement;
    
    private readonly NetworkVariable<PlayerRoles> _netRole = new(PlayerRoles.Unassigned, readPerm: NetworkVariableReadPermission.Owner);
    private readonly NetworkVariable<int> _netHealth = new(100);
    
    public Camera PlayerCamera => playerCamera;
    public bool IsAlive => _netHealth.Value > 0;
    public PlayerRole Role { get; private set; }
    public static event Action<ulong> OnPlayerDeath;
    
    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }
    
    public override void OnNetworkSpawn()
    {
        networkObject = GetComponent<NetworkObject>();
        
        _netRole.OnValueChanged += OnChangeRole;
        
        if (IsOwner) playerCamera.enabled = true;
        if (IsOwner) audioListener.enabled = true;
    }
    
    public override void OnNetworkDespawn()
    {
        _netRole.OnValueChanged -= OnChangeRole;
    }
    
    private void OnChangeRole(PlayerRoles prevValue, PlayerRoles newValue)
    {
        Role = PlayerRoleMapping.Mapping[newValue];
        capsuleRenderer.material.color = Role.Color;
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        // TODO: check with new unity input system
        if (Role != null && Input.GetButtonDown("Fire1")) Role.UseAbility();
    }
    
    public void SetPlayerRole(PlayerRoles role)
    {
        if (!IsServer) return; // only server can set player roles
        _netRole.Value = role;
    }

    public void Kill()
    {
        if (!IsServer) return; // only server can kill players

        _netHealth.Value = 0;

        OnPlayerDeath?.Invoke(networkObject.OwnerClientId);
        PlayerDeathClientRPC(); // Notify all clients this player (object) has died
    }
    
    [ClientRpc]
    public void PlayerDeathClientRPC(ClientRpcParams clientRpcParams = default)
    {
        gameObject.SetActive(false);
    }
}