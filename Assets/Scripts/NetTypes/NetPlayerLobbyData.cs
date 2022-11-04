using System;
using Unity.Netcode;

namespace NetTypes
{
    public struct NetPlayerLobbyData : INetworkSerializable, IEquatable<NetPlayerLobbyData>
    {
        private ulong _clientId;
        public ulong ClientId => _clientId;
        
        public NetPlayerLobbyData(ulong clientId)
        {
            _clientId = clientId;
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _clientId);
        }

        public bool Equals(NetPlayerLobbyData other)
        {
            return _clientId == other._clientId;
        }

        public override bool Equals(object obj)
        {
            return obj is NetPlayerLobbyData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _clientId.GetHashCode();
        }
    }
}