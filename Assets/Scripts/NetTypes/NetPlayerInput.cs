using UnityEngine;
using Unity.Netcode;

namespace NetTypes
{
    public struct NetPlayerInput : INetworkSerializable
    {
        public Vector2 Input;
        public float YRotation;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Input);
            serializer.SerializeValue(ref YRotation);
        }
    }
}
