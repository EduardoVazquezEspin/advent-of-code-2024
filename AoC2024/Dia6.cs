namespace AoC2024;

public struct Dia6Input
{
    public char[][] Map { get; init; }
    public Tuple<int, int> InitialPosition { get; init; }
}

public class Dia6 : ProblemSolution<Dia6Input>
{
    public override int CurrentDay()
    {
        return 6;
    }

    public override Dia6Input ReadInput(string[] rawInput)
    {
        Tuple<int, int> initialPosition = new Tuple<int, int>(0, 0);
        char[][] map = new char[rawInput.Length][];

        for (int i = 0; i < rawInput.Length; i++)
        {
            var row = new char[rawInput[i].Length];
            for (int j = 0; j < rawInput[i].Length; j++)
            {
                switch (rawInput[i][j])
                {
                    case '#':
                        row[j] = '#';
                        break;
                    case '^':
                        initialPosition = new Tuple<int, int>(i, j);
                        row[j] = '.';
                        break;
                    default:
                        row[j] = '.';
                        break;
                }
                row[j] = rawInput[i][j];
            }
            map[i] = row;
        }

        return new Dia6Input
        {
            Map = map,
            InitialPosition = initialPosition
        };
    }

    private bool IsInBounds<T>(T[][] map, int posX, int posY)
    {
        if (posX < 0)
            return false;
        if (posY < 0)
            return false;
        if (posX >= map.Length)
            return false;
        if (posY >= map[posX].Length)
            return false;
        return true;
    }

    enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private readonly Tuple<int, int>[] _vectors = 
    {
        new (-1, 0),
        new (0, 1),
        new (1, 0),
        new (0, -1)
    };

    private Tuple<int, int> GetVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return _vectors[0];
            case Direction.Right:
                return _vectors[1];
            case Direction.Down:
                return _vectors[2];
            case Direction.Left:
                return _vectors[3];
        }

        throw new Exception("Invalid Direction Input");
    }

    private Direction TurnRight(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Direction.Right;
            case Direction.Right:
                return Direction.Down;
            case Direction.Down:
                return Direction.Left;
            case Direction.Left:
                return Direction.Up;
        }

        throw new Exception("Invalid Direction Input");
    }

    private int CountX(char[][] map)
    {
        var total = 0;
        for(int i =0; i<map.Length; i++)
            for(int j =0; j<map[i].Length; j++)
                if (map[i][j] == 'X')
                    total++;
        return total;
    }

    private void PrintMap(char[][] map)
    {
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                Console.Write(map[i][j]);
            }
            Console.WriteLine();
        }
    }
    
    public override object Part1(Dia6Input input)
    {
        var map = input.Map;
        var position = input.InitialPosition;
        var direction = Direction.Up;
        while (IsInBounds(map, position.Item1, position.Item2))
        {
            map[position.Item1][position.Item2] = 'X';
            var vector = GetVector(direction);
            var movesTo = new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2);
            if (IsInBounds(map, movesTo.Item1, movesTo.Item2) && map[movesTo.Item1][movesTo.Item2] == '#')
            {
                direction = TurnRight(direction);
                movesTo = position;
            }
            position = movesTo;
        }
        
        return CountX(map);
    }

    private T[][] CopyMap<T>(char[][] map, Func<char, T> onAssign)
    {
        T[][] copy = new T[map.Length][];
        for (int i = 0; i < map.Length; i++)
        {
            T[] row = new T[map[i].Length];
            copy[i] = row;
            for (int j = 0; j < map[i].Length; j++)
                row[j] = onAssign(map[i][j]);
        }

        return copy;
    }

    struct CellHistory
    {
        public char Value { get; }
        public Dictionary<Direction, bool> HaveWeBeenHere;

        public CellHistory(char value)
        {
            Value = value;
            HaveWeBeenHere = new Dictionary<Direction, bool>
            {
                [Direction.Up] = false,
                [Direction.Right] = false,
                [Direction.Down] = false,
                [Direction.Left] = false
            };
        }
    }

    private bool IsOnLoop(char[][] map, Tuple<int, int> initialPosition, Tuple<int, int> newObstacle)
    {
        var copy = CopyMap(map, c => new CellHistory(c));
        copy[newObstacle.Item1][newObstacle.Item2] = new CellHistory('#');
        var position = new Tuple<int, int>(initialPosition.Item1, initialPosition.Item2);
        var direction = Direction.Up;
        while (IsInBounds(copy, position.Item1, position.Item2))
        {
            if (copy[position.Item1][position.Item2].HaveWeBeenHere[direction])
                return true;
            copy[position.Item1][position.Item2].HaveWeBeenHere[direction] = true;
            var vector = GetVector(direction);
            var movesTo = new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2);
            if (!IsInBounds(copy, movesTo.Item1, movesTo.Item2))
                return false;
            if(copy[movesTo.Item1][movesTo.Item2].Value == '#')
            {
                direction = TurnRight(direction);
                movesTo = position;
            }
            position = movesTo;
        }

        return false;
    }
    
    public override object Part2(Dia6Input input)
    {
        var originalTravel = CopyMap( input.Map, (char c) => c);
        var position = new Tuple<int, int>(input.InitialPosition.Item1, input.InitialPosition.Item2);
        var direction = Direction.Up;
        while (IsInBounds(originalTravel, position.Item1, position.Item2))
        {
            originalTravel[position.Item1][position.Item2] = 'X';
            var vector = GetVector(direction);
            var movesTo = new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2);
            if (IsInBounds(originalTravel, movesTo.Item1, movesTo.Item2) && originalTravel[movesTo.Item1][movesTo.Item2] == '#')
            {
                direction = TurnRight(direction);
                movesTo = position;
            }
            position = movesTo;
        }

        var total = 0;
        for (int x = 0; x < originalTravel.Length; x++)
        {
            for (int y = 0; y < originalTravel[x].Length; y++)
            {
                if (originalTravel[x][y] == 'X')
                {
                    if (IsOnLoop(input.Map, input.InitialPosition, new Tuple<int, int>(x, y)))
                        total++;
                }
            }
        }

        return total;
    }
}