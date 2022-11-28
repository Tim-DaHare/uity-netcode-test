using NetTypes;
using Unity.Netcode;
using UnityEngine;

namespace Behaviors
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float speed = 5;
    
        private readonly NetworkVariable<NetPlayerTransform> _netTransform = new();
        private readonly NetworkVariable<NetPlayerInput> _netPlayerInput = new(readPerm: NetworkVariableReadPermission.Owner, writePerm: NetworkVariableWritePermission.Owner);
    
        private CharacterController _controller;
        private float _camXRotation;
    
        private Player _player;
    
        private void Awake()
        {
            _player = GetComponent<Player>();
            _controller = GetComponent<CharacterController>();
    
            _camXRotation = _player.PlayerCamera.transform.localEulerAngles.x;
        }
    
        public override void OnNetworkSpawn()
        {
            _netTransform.OnValueChanged += OnChangeTransform;
        }
    
        public override void OnNetworkDespawn()
        {
            _netTransform.OnValueChanged -= OnChangeTransform;
        }
    
        private void OnChangeTransform(NetPlayerTransform prevValue, NetPlayerTransform newValue)
        {
            var currTransform = transform;
        
            // TODO: Fix teleport
            // if (newValue.DidTeleport)
            // {
            //     print("teleported to: " + newValue.Position);
            //     currTransform.position = newValue.Position;
            //     currTransform.eulerAngles = new Vector3(0, newValue.YRotation, 0);
            // }
        
            if (IsClient && !IsServer)
            {
                var dist = Vector3.Distance(transform.position, newValue.Position);
                if (dist > 0.5f) currTransform.position = newValue.Position;
            }
        
            if (!IsOwner) currTransform.position = newValue.Position;
            if (!IsOwner) currTransform.eulerAngles = new Vector3(0, newValue.YRotation, 0);
        }
    
        private void Update()
        {
            if (IsOwner)
            {
                // TODO: check with new unity input system
                var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
                var mouseX = Input.GetAxis("Mouse X");
                var mouseY = Input.GetAxis("Mouse Y");
            
                transform.Rotate(Vector3.up, mouseX);

                _camXRotation = Mathf.Clamp(_camXRotation - mouseY, -90, 90);
                _player.PlayerCamera.transform.localEulerAngles = new Vector3(_camXRotation, 0, 0);
            
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

        public void Teleport(Vector3 position)
        {
            if (!IsServer) return; // Only server can teleport players

            _netTransform.Value = new NetPlayerTransform
            {
                Position = position,
                YRotation = 0,
                DidTeleport = true
            };
        }
    }
}