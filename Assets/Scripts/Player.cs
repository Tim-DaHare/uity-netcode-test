using Classes;
using Enums;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private float speed = 5;
    
    private CharacterController _controller;

    private readonly NetworkVariable<PlayerTransformNetState> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private PlayerRole role;
    
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
        // _netState.OnValueChanged += OnNetStateUpdate;
    }

    public override void OnDestroy()
    {
        // _netState.OnValueChanged -= OnNetStateUpdate;
    }

    // private void OnNetStateUpdate(PlayerNetState prevValue, PlayerNetState newValue)
    // {
    //     transform.position = _netState.Value.Position;
    //     transform.rotation = Quaternion.Euler(0, _netState.Value.YRotation, 0);
    // }

    private void Update()
    {
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
    
    private struct PlayerIdentityNetState : INetworkSerializable
    {
        // public Vector3 Position;
        // public float YRotation;

        public PlayerTeams Team;
        
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            // serializer.SerializeValue(ref Position);
            // serializer.SerializeValue(ref YRotation);
        }
    }
}