namespace AoC2024.Classes;

public class AbstractObject<TSelf> where TSelf: AbstractObject<TSelf>
{
    // ReSharper disable once StaticMemberInGenericType
    private static ulong _serial = 0;

    private readonly ulong _id;
    
    public AbstractObject()
    {
        _id = _serial++;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is AbstractObject<TSelf> abstractObject))
            return false;

        return abstractObject._id == _id;
    }

    public override string ToString()
    {
        return "AbstractObject." + _id;
    }

    public override int GetHashCode()
    {
        return (int) _id;
    }
}