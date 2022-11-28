using Unity.Netcode;
using UnityEngine;

namespace NetTypes
{
    public struct NetPlayerTransform : INetworkSerializable
    {
        public Vector3 Position;
        public float YRotation;
        public bool DidTeleport;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref YRotation);
            serializer.SerializeValue(ref DidTeleport);
        }
    }
}
