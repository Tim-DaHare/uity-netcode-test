using UnityEngine;
using Unity.Netcode;
using Classes;
using Enums;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Renderer capsuleRenderer;
    
    public Camera PlayerCamera => playerCamera;
    
    private readonly NetworkVariable<PlayerRoles> _netRole = new(readPerm: NetworkVariableReadPermission.Owner);
    // private readonly NetworkVariable<bool> _netIsAlive = new();
    
    private CharacterController _controller;
    
    public PlayerRole Role { get; private set; }
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    
    public override void OnNetworkSpawn()
    {
        _netRole.OnValueChanged += OnChangeRole;
        
        if (IsOwner) playerCamera.enabled = true;
        if (IsOwner) audioListener.enabled = true;
        
        // if (IsServer)
        // {
        //     NetworkManager.Singleton.OnClientConnectedCallback += clientId =>
        //     {
        //         NetworkManager.ConnectedClients[clientId].PlayerObject.transform.name = "Player " + clientId;
        //     };
        // }
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

    public void SetPlayerRole(PlayerRoles role)
    {
        if (!IsServer) return;
        _netRole.Value = role;
    }
    
    [ServerRpc]
    public void ChangeRoleServerRpc()
    {
        if (PlayerRoleMapping.Mapping.Count == 0) return;
        
        var i = Random.Range(1, PlayerRoleMapping.Mapping.Count);
        _netRole.Value = (PlayerRoles)i;
    }
    
    private void Update()
    {
        if (!IsOwner) return;
        
        // TODO: check with new unity input system;
        if (Role != null && Input.GetButtonDown("Fire1")) Role.UseAbility();
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(Vector3.up, mouseX);
        playerCamera.transform.Rotate(Vector3.right, -mouseY, Space.Self);
        
        var currRotation = transform.rotation;
        _controller.Move( currRotation * new Vector3(input.x, 0, input.y) * (speed * Time.deltaTime));
    }
}