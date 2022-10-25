using Unity.Netcode;
using UnityEngine;

namespace NetTypes
{
    public struct PlayerTransformNetState : INetworkSerializable
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
