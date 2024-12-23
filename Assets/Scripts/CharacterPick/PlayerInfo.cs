using Unity.Collections;
using Unity.Netcode;


[System.Serializable]
public struct PlayerInfo : INetworkSerializable, System.IEquatable<PlayerInfo>
{
    public ulong ClientId;
    public FixedString64Bytes PlayerName;
    public int Order;

    public PlayerInfo(ulong clientId, FixedString64Bytes playerName, int order)
    {
        ClientId = clientId;
        PlayerName = playerName;
        Order = order;  
    }

    public bool Equals(PlayerInfo other)
    {
        return ClientId == other.ClientId &&
               PlayerName.Equals(other.PlayerName) &&
               Order == other.Order;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Order);
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return ClientId.GetHashCode() ^ PlayerName.GetHashCode() ^ Order.GetHashCode();
    }
}
