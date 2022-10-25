using Classes;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    
    private CharacterController _controller;
    private Renderer _renderer;
    
    private readonly NetworkVariable<PlayerTransformNetState> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<PlayerRoles> _netRole = new();

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _renderer = GetComponent<Renderer>();
    }
    
    public override void OnNetworkSpawn()
    {
        // _netRole += OnChangeRole;
    }

    private void OnChangeRole(FixedString32Bytes old, FixedString32Bytes newV)
    {
        // _renderer.material.color = 
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void ChangeRoleServerRpc(PlayerRoles roleName, ServerRpcParams serverRpcParams = default)
    {
        var sendClientId = serverRpcParams.Receive.SenderClientId;
        var senderObject = NetworkManager.ConnectedClients[sendClientId].PlayerObject;
        
        if (!senderObject.TryGetComponent<Player>(out var player)) return;
        
        _netRole.Value = roleName;
    }

    private void Update()
    {
        _renderer.material.color = _netRole.Value == PlayerRoles.Civilian ? Color.green : Color.red;
        
        if (!IsOwner)
        {
            transform.position = _netState.Value.Position;
            transform.rotation = Quaternion.Euler(0, _netState.Value.YRotation, 0);
            
            return;
        }
        
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        _controller.Move(new Vector3(input.x, 0, input.y) * (speed * Time.deltaTime));
        _netState.Value = new PlayerTransformNetState()
        {
            Position = transform.position,
            YRotation = 0
        };
    }

    private struct PlayerTransformNetState : INetworkSerializable
    {
        public Vector3 Position;
        public float YRotation;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref YRotation);
        }
    }
}