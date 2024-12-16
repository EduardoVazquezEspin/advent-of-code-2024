using AoC2024.Classes;

namespace AoC2024;

public struct Dia15Input
{
    public CharMap Map { get; init; }
    public Tuple<int, int> PlayerPosition { get; init; }
    public List<Direction> Instructions { get; init; }
}

public class Dia15 : ProblemSolution<Dia15Input>
{
    public override int CurrentDay()
    {
        return 15;
    }

    public override Dia15Input ReadInput(string[] rawInput)
    {
        var index = rawInput
            .Select((s, i) => new Tuple<string, int>(s, i))
            .Where(it => it.Item1 == "")
            .Select(it => it.Item2).First();

        var map = new CharMap(rawInput, 0, index);

        var playerPosition = map
            .MapAllCells((cell, i, j) => new Tuple<char, Tuple<int, int>>(cell, new Tuple<int, int>(i, j)))
            .Where(it => it.Item1 == '@')
            .Select(it => it.Item2)
            .First();

        var instructions = new List<Direction>();
        for (int i = index + 1; i < rawInput.Length; i++)
        for(int j = 0; j< rawInput[i].Length; j++)
                instructions.Add(new Direction(rawInput[i][j]));

        return new Dia15Input
        {
            Map = map,
            PlayerPosition = playerPosition,
            Instructions = instructions
        };
    }

    private Tuple<int, int> DoOneMovement(CharMap map, Tuple<int, int> playerPosition, Direction direction)
    {
        var thingsToMove = new List<Tuple<int, int>> { playerPosition };
        var vector = direction.GetVector();
        char currentCellValue;
        var currentPosition = playerPosition;
        do
        {
            currentPosition =
                new Tuple<int, int>(currentPosition.Item1 + vector.Item1, currentPosition.Item2 + vector.Item2);
            thingsToMove.Add(currentPosition);
            currentCellValue = map.Get(currentPosition);
        } while (currentCellValue == 'O' || currentCellValue == '[' || currentCellValue == ']');

        if (currentCellValue == '#')
            return playerPosition;

        thingsToMove.RemoveAt(thingsToMove.Count - 1);
        
        while (thingsToMove.Any())
        {
            var thingToMove = map.Get(thingsToMove[^1]);
            thingsToMove.RemoveAt(thingsToMove.Count - 1);
            map.Set(currentPosition, thingToMove);
            currentPosition = new Tuple<int, int>(currentPosition.Item1 - vector.Item1, currentPosition.Item2 - vector.Item2); 
        }

        map.Set(currentPosition, '.');

        return new Tuple<int, int>(currentPosition.Item1 + vector.Item1, currentPosition.Item2 + vector.Item2);;
    }

    private long EvaluteMap(CharMap map) => 
        map.MapAllCells((cell, i, j) =>
        {
            if (cell != 'O' && cell != '[') return 0L;
            return 100 * i + j;
        }).Sum();
    

    public override IFormattable Part1(Dia15Input input)
    {
        var playerPosition = input.PlayerPosition;
        foreach (var instruction in input.Instructions)
            playerPosition = DoOneMovement(input.Map, playerPosition, instruction);

        return EvaluteMap(input.Map);
    }

    private struct DuplicationResult
    {
        public CharMap Map { get; init; }
        public Tuple<int, int> PlayerPosition { get; init; }
    }

    private DuplicationResult DuplicateMap(CharMap map)
    {
        Tuple<int, int> playerPosition = null;
        var copy = new CharMap(map.Height, 2 * map.Width, (i, j) =>
        {
            var c = map.Get(i, j / 2);
            if (c == '#')
                return c;
            if (c == 'O' && j % 2 == 0)
                return '[';
            if (c == 'O' && j % 2 == 1)
                return ']';
            if (c == '@' && j % 2 == 0)
            {
                playerPosition = new Tuple<int, int>(i, j);
                return '@';
            }
            return '.';
        });
        
        return new DuplicationResult
        {
            Map = copy,
            PlayerPosition = playerPosition!
        };
    }

    private int HashPosition(Tuple<int, int> position, CharMap map) => position.Item1 + map.Height * position.Item2;

    private Tuple<int, int> DoOneMovementPart2(CharMap map, Tuple<int, int> playerPosition, Direction direction)
    {
        if (direction.Type == DirectionType.Left || direction.Type == DirectionType.Right)
            return DoOneMovement(map, playerPosition, direction);

        var vector = direction.GetVector();
        var hasBeenChecked = new HashSet<int>();
        
        var cellsToCheckQueue = new List<Tuple<int, int>> { playerPosition };
        var cellsToPushStack = new List<Tuple<int, int>>();
        while (cellsToCheckQueue.Any())
        {
            var top = cellsToCheckQueue[0];
            cellsToCheckQueue.RemoveAt(0);
            var value = map.Get(top);
            if (value == '#')
                return playerPosition;
            
            var hash = HashPosition(top, map);
            if(hasBeenChecked.Contains(hash))
                continue;

            hasBeenChecked.Add(hash);
            if (value != '.')
            {
                cellsToCheckQueue.Add(new Tuple<int, int>(top.Item1 + vector.Item1, top.Item2 + vector.Item2));
                cellsToPushStack.Add(top);
            }
            if(value == '[')
                cellsToCheckQueue.Add(new Tuple<int, int>(top.Item1, top.Item2 + 1));
            else if(value == ']')
                cellsToCheckQueue.Add(new Tuple<int, int>(top.Item1, top.Item2 - 1));
        }

        while (cellsToPushStack.Any())
        {
            var top = cellsToPushStack[^1];
            cellsToPushStack.RemoveAt(cellsToPushStack.Count - 1);
            map.Set(top.Item1 + vector.Item1, top.Item2 + vector.Item2, map.Get(top));
            map.Set(top, '.');
        }

        return new Tuple<int, int>(playerPosition.Item1 + vector.Item1, playerPosition.Item2 + vector.Item2);
    }

    public override IFormattable Part2(Dia15Input input)
    {
        var result = DuplicateMap(input.Map);
        var playerPosition = result.PlayerPosition;
        foreach (var instruction in input.Instructions)
            playerPosition = DoOneMovementPart2(result.Map, playerPosition, instruction);
        

        return EvaluteMap(result.Map);
    }
}