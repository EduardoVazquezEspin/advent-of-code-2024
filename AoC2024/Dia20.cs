using AoC2024.Classes;
using AoC2024.Helpers;

namespace AoC2024;

public struct Dia20Input
{
    public CharMap Map { get; init; }
    public Tuple<int, int> Start { get; init; }
    public Tuple<int, int> Exit { get; init; }
    public int LowerLimit { get; init; }
}

public class Dia20 : ProblemSolution<Dia20Input>
{
    public override int CurrentDay()
    {
        return 20;
    }

    public override Dia20Input ReadInput(string[] rawInput)
    {
        Tuple<int, int>? start = null;
        Tuple<int, int>? exit = null;
        var map = new CharMap(rawInput.Length -2, rawInput[0].Length, (i, j) =>
        {
            var c = rawInput[i][j];
            if (c == 'S')
                start = new Tuple<int, int>(i, j);
            if (c == 'E')
                exit = new Tuple<int, int>(i, j);
            return c;
        });

        int lowerLimit = int.Parse(rawInput[^1]);
        return new Dia20Input
        {
            Start = start!,
            Exit = exit!,
            Map = map,
            LowerLimit = lowerLimit
        };
    }

    private Func<Tuple<int, int>, Tuple<int, int>[]> MakeGetNeighbours(CharMap map) => (position) =>
        Direction.Directions
            .Select(Direction.GetVector)
            .Select(vector => new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2))
            .Select(newPosition =>
            {
                var isInBounds = map.IsInBounds(newPosition, out char value);
                if (!isInBounds)
                    return null;
                if (value == '#')
                    return null;
                return newPosition;
            })
            .Where(it => it != null)
            .ToArray()!;

    private readonly Tuple<int, int>[] _twoStepDirections = new[]
    {
        new Tuple<int, int>(2, 0),
        new Tuple<int, int>(1, 1),
        new Tuple<int, int>(0, 2),
        new Tuple<int, int>(-1, 1),
        new Tuple<int, int>(-2, 0),
        new Tuple<int, int>(-1, -1),
        new Tuple<int, int>(0, -2),
        new Tuple<int, int>(1, -1)
    };

    private Tuple<int, int>[] NthStepsDirections(int n)
    {
        var solution = new List<Tuple<int, int>>();
        for(int i =0; i<n; i++)
            solution.Add(new Tuple<int, int>(n -i, i));
        for(int i =0; i<n; i++)
            solution.Add(new Tuple<int, int>(-i, n-i));
        for(int i =0; i<n; i++)
            solution.Add(new Tuple<int, int>(-n+i, -i));
        for(int i =0; i<n; i++)
            solution.Add(new Tuple<int, int>(i, -n +i));

        return solution.ToArray();
    }

    private Tuple<int, int>[] UpToNthStepsDirections(int n)
    {
        var result = new List<Tuple<int, int>>();
        for (int i = 2; i <= n; i++)
        {
            var vectors = NthStepsDirections(i);
            foreach (var vector in vectors)
            {
                result.Add(vector);
            }
        }

        return result.ToArray();
    }

    private int VectorDistance(Tuple<int, int> first, Tuple<int, int> second)
    {
        return Math.Abs(first.Item1 - second.Item1) + Math.Abs(first.Item2 - second.Item2);
    }

    public override object Part1(Dia20Input input)
    {
        var result = GraphAlgorithms.BreadthFirstSearch(
            input.Start,
            MakeGetNeighbours(input.Map)
        );

        return input.Map.MapAllCells((c, position) =>
            {
                if (c == '#')
                    return null;
                var hasBeenMapped = result.TryGetValue(position, out var distance);
                if (!hasBeenMapped)
                    return null;
                return new Tuple<Tuple<int, int>, int>(position, distance);

            }).Where(it => it != null)
            .SelectMany(tuple =>
            {
                var position = tuple!.Item1;
                var distance = tuple.Item2;
                return _twoStepDirections
                    .Select(vector => new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2))
                    .Select(newPosition =>
                    {
                        var hasBeenPassed = result.TryGetValue(newPosition, out var value);
                        if (!hasBeenPassed)
                            return false;
                        if (distance - value - 2 < input.LowerLimit)
                            return false;
                        return true;
                    })
                    .Where(it => it)
                    .ToArray();
            }).Count();
    }

    public override object Part2(Dia20Input input)
    {
        var result = GraphAlgorithms.BreadthFirstSearch(
            input.Start,
            MakeGetNeighbours(input.Map)
        );

        List<Tuple<Tuple<int, int>, int>> allMappedCells = input.Map.MapAllCells((c, position) =>
            {
                if (c == '#')
                    return null;
                var hasBeenMapped = result.TryGetValue(position, out var distance);
                if (!hasBeenMapped)
                    return null;
                return new Tuple<Tuple<int, int>, int>(position, distance);

            }).Where(it => it != null)
            .ToList()!;

        long total = 0L;
        var vectors = UpToNthStepsDirections(20);
        foreach (var (position, distance) in allMappedCells)
        {
            var res = vectors
                .Select(vector => new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2))
                .Select(newPosition =>
                {
                    var hasBeenPassed = result.TryGetValue(newPosition, out var value);
                    if (!hasBeenPassed)
                        return -1;
                    if (distance - value - VectorDistance(position, newPosition) < input.LowerLimit)
                        return -1;
                    return distance - value - VectorDistance(position, newPosition);
                })
                .Where(it => it != -1)
                .ToArray();

            total += res.Length;
        }

        return total;
    }
}