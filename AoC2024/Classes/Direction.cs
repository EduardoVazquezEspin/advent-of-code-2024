namespace AoC2024.Classes;

public enum DirectionType
{
    Up,
    Right,
    Down,
    Left
}

public class Direction
{
    public DirectionType Type { get; private set; }

    public Direction(char c)
    {
        switch (c)
        {
            case '^':
                Type = DirectionType.Up;
                break;
            case '>':
                Type = DirectionType.Right;
                break;
            case 'v':
                Type = DirectionType.Down;
                break;
            case '<':
                Type = DirectionType.Left;
                break;
            default:
                throw new Exception("Invalid Character!");
        }
    }

    public Direction(DirectionType directionType)
    {
        this.Type = directionType;
    }

    public static DirectionType[] Directions =
        {DirectionType.Up, DirectionType.Right, DirectionType.Down, DirectionType.Left}; 

    private static Tuple<int, int>[] _directions = 
    {
        new(-1, 0),
        new(0, 1),
        new(1, 0),
        new(0, -1)
    };

    public Tuple<int, int> GetVector() => GetVector(Type);
 
    public static Tuple<int, int> GetVector(DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Up:
                return _directions[0];
            case DirectionType.Right:
                return _directions[1];
            case DirectionType.Down:
                return _directions[2];
            default:
                return _directions[3];
        }
    }

    public Direction Copy()
    {
        return new Direction(Type);
    }

    public Direction Turn90DegreesRight()
    {
        switch (Type)
        {
            case DirectionType.Up:
                Type = DirectionType.Right;
                break;
            case DirectionType.Right:
                Type = DirectionType.Down;
                break;
            case DirectionType.Down:
                Type = DirectionType.Left;
                break;
            default:
                Type = DirectionType.Up;
                break;
        }

        return this;
    }
    
    public Direction Turn90DegreesLeft()
    {
        switch (Type)
        {
            case DirectionType.Up:
                Type = DirectionType.Left;
                break;
            case DirectionType.Right:
                Type = DirectionType.Up;
                break;
            case DirectionType.Down:
                Type = DirectionType.Right;
                break;
            default:
                Type = DirectionType.Down;
                break;
        }

        return this;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is Direction objDirection))
            return false;
        return objDirection.Type == Type;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        switch (Type)
        {
            case DirectionType.Up:
                return 0;
            case DirectionType.Right:
                return 1;
            case DirectionType.Down:
                return 2;
            default:
                return 3;
        }
    }

    public override string ToString()
    {
        return Type.ToString();
    }
}