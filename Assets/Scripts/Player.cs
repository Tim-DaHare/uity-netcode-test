using Classes;
using Enums;
using NetTypes;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Camera playerCamera;
    
    private readonly NetworkVariable<PlayerTransformNetState> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<PlayerRoles> _netRole = new();
    
    private CharacterController _controller;
    private Renderer _renderer;
    
    private PlayerRole _role;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _renderer = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        _netRole.OnValueChanged += OnChangeRole;
        _netState.OnValueChanged += OnNetStateChanged;
        playerCamera.enabled = true;
    }

    public override void OnNetworkDespawn()
    {
        _netRole.OnValueChanged -= OnChangeRole;
        _netState.OnValueChanged -= OnNetStateChanged;
    }

    private void OnChangeRole(PlayerRoles prevValue, PlayerRoles newValue)
    {
        _role = PlayerRoleMapping.Mapping[newValue];
        _renderer.material.color = _role.Color;
    }
    
    private void OnNetStateChanged(PlayerTransformNetState prevVal, PlayerTransformNetState newValue)
    {
        if (IsOwner) return;
        
        transform.position = _netState.Value.Position;
        transform.rotation = Quaternion.Euler(0, _netState.Value.YRotation, 0);
    }

    [ServerRpc]
    public void ChangeRoleServerRpc(PlayerRoles roleName)
    {
        _netRole.Value = roleName;
    }
    
    [ServerRpc]
    private void UseAbilityServerRpc()
    {
        _role?.UseAbility();
    }

    private void Update()
    {
        if (!IsOwner) return;
        
        // TODO: check with new unity input system;
        if (Input.GetButtonDown("Fire1")) UseAbilityServerRpc();
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(Vector3.up, mouseX);
        playerCamera.transform.Rotate(Vector3.right, -mouseY, Space.Self);
        
        _controller.Move( transform.rotation * new Vector3(input.x, 0, input.y) * (speed * Time.deltaTime));
        _netState.Value = new PlayerTransformNetState()
        {
            Position = transform.position,
            YRotation = 0
        };
    }
}