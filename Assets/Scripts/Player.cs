using UnityEngine;
using Unity.Netcode;
using Classes;
using Enums;
using NetTypes;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Renderer capsuleRenderer;
    
    private readonly NetworkVariable<NetPlayerTransform> _netTransform = new();
    private readonly NetworkVariable<NetPlayerInput> _netPlayerInput = new(readPerm: NetworkVariableReadPermission.Owner, writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<PlayerRoles> _netRole = new(readPerm: NetworkVariableReadPermission.Owner);
    private readonly NetworkVariable<int> _netHealth = new(100);
    
    private CharacterController _controller;
    private float _camXRotation;
    
    public Camera PlayerCamera => playerCamera;
    public bool IsAlive => _netHealth.Value > 0;
    public PlayerRole Role { get; private set; }
    
    private void Awake()
    {
        _camXRotation = playerCamera.transform.localEulerAngles.x;
        _controller = GetComponent<CharacterController>();
    }
    
    public override void OnNetworkSpawn()
    {
        _netRole.OnValueChanged += OnChangeRole;
        _netTransform.OnValueChanged += OnChangeTransform;
        
        if (IsOwner) playerCamera.enabled = true;
        if (IsOwner) audioListener.enabled = true;
    }
    
    public override void OnNetworkDespawn()
    {
        _netRole.OnValueChanged -= OnChangeRole;
        _netTransform.OnValueChanged -= OnChangeTransform;
    }
    
    private void OnChangeTransform(NetPlayerTransform prevValue, NetPlayerTransform newValue)
    {
        var currTransform = transform;

        if (IsOwner)
        {
            var dist = Vector3.Distance(transform.position, newValue.Position);
            if (dist > 0.5f) currTransform.position = newValue.Position;
        }
        
        if (!IsOwner) currTransform.position = newValue.Position;
        if (!IsOwner) currTransform.eulerAngles = new Vector3(0, newValue.YRotation, 0);
    }
    
    private void OnChangeRole(PlayerRoles prevValue, PlayerRoles newValue)
    {
        Role = PlayerRoleMapping.Mapping[newValue];
        capsuleRenderer.material.color = Role.Color;
    }
    
    private void Update()
    {
        if (IsOwner)
        {
            // TODO: check with new unity input system;
            if (Role != null && Input.GetButtonDown("Fire1")) Role.UseAbility();
            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            
            transform.Rotate(Vector3.up, mouseX);
            
            _camXRotation = Mathf.Clamp(_camXRotation - mouseY, -90, 90);
            playerCamera.transform.localEulerAngles = new Vector3(_camXRotation, 0, 0);
            
            _netPlayerInput.Value = new NetPlayerInput
            {
                Input = input,
                YRotation = transform.eulerAngles.y
            };

            if (!IsServer)
            {
                // client owner prediction
                var xzMoveDir = Vector3.ClampMagnitude(transform.rotation * new Vector3(input.x, 0, input.y), 1);
                _controller.Move(xzMoveDir * (speed * Time.deltaTime));
            }
        }
        
        if (IsServer)
        {
            // Server movement logic
            var xzMoveDir = Vector3.ClampMagnitude(transform.rotation * new Vector3(_netPlayerInput.Value.Input.x, 0, _netPlayerInput.Value.Input.y), 1);
            _controller.Move(xzMoveDir * (speed * Time.deltaTime));
            
            _netTransform.Value = new NetPlayerTransform
            {
                Position = transform.position,
                YRotation = _netPlayerInput.Value.YRotation
            };
        }
    }
    
    public void SetPlayerRole(PlayerRoles role)
    {
        if (!IsServer) return;
        _netRole.Value = role;
    }

    public void Die()
    {
        if (!IsServer) return;

        _netTransform.Value = new NetPlayerTransform
        {
            Position = Vector3.zero,
            YRotation = 0
        };
    }
}