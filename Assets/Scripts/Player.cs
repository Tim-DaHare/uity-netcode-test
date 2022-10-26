using Classes;
using Enums;
using NetTypes;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Renderer capsuleRenderer;
    
    private readonly NetworkVariable<PlayerNetTransform> _netTransform = new(writePerm: NetworkVariableWritePermission.Owner);
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
        _netTransform.OnValueChanged += OnNetStateChanged;
        
        if (IsOwner) playerCamera.enabled = true;
        if (IsOwner) audioListener.enabled = true;
    }

    public override void OnNetworkDespawn()
    {
        _netRole.OnValueChanged -= OnChangeRole;
        _netTransform.OnValueChanged -= OnNetStateChanged;
    }

    private void OnChangeRole(PlayerRoles prevValue, PlayerRoles newValue)
    {
        Role = PlayerRoleMapping.Mapping[newValue];
        capsuleRenderer.material.color = Role.Color;
    }
    
    private void OnNetStateChanged(PlayerNetTransform prevVal, PlayerNetTransform newValue)
    {
        if (IsOwner) return;
        
        transform.position = _netTransform.Value.Position;
        transform.rotation = Quaternion.Euler(0, _netTransform.Value.YRotation, 0);
    }

    [ServerRpc]
    public void ChangeRoleServerRpc()
    {
        if (PlayerRoleMapping.Mapping.Count == 0) return;
        
        var i = Random.Range(1, PlayerRoleMapping.Mapping.Count);
        _netRole.Value = (PlayerRoles)i;
    }
    
    [ServerRpc]
    private void UseAbilityServerRpc()
    {
        Role?.UseAbility();
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
        
        var currRotation = transform.rotation;
        _controller.Move( currRotation * new Vector3(input.x, 0, input.y) * (speed * Time.deltaTime));
        _netTransform.Value = new PlayerNetTransform()
        {
            Position = transform.position,
            YRotation = currRotation.eulerAngles.y
        };
    }
}