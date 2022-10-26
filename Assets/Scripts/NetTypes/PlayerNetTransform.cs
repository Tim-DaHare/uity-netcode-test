using Unity.Netcode;
using UnityEngine;

namespace NetTypes
{
    public struct PlayerNetTransform : INetworkSerializable
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
