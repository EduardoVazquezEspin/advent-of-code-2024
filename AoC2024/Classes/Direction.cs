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
    public DirectionType Type { get; }

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

    private static Tuple<int, int>[] _directions = new Tuple<int, int>[]
    {
        new(-1, 0),
        new(0, 1),
        new(1, 0),
        new(0, -1)
    };

    public Tuple<int, int> GetVector()
    {
        switch (Type)
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

}