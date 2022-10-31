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
        
        // TODO: check with new unity input system;
        if (Role != null && Input.GetButtonDown("Fire1")) Role.UseAbility();
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");
        
        transform.Rotate(Vector3.up, mouseX);
        
        _camXRotation = Mathf.Clamp(_camXRotation - mouseY, -90, 90);
        playerCamera.transform.localEulerAngles = new Vector3(_camXRotation, 0, 0);

        var currRotation = transform.rotation;

        var xzMoveDir = Vector3.ClampMagnitude(currRotation * new Vector3(input.x, 0, input.y), 1);
        _controller.Move( xzMoveDir * (speed * Time.deltaTime));
    }
    
    public void SetPlayerRole(PlayerRoles role)
    {
        if (!IsServer) return;
        _netRole.Value = role;
    }
}