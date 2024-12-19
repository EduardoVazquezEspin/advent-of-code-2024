using System.Text.RegularExpressions;
using AoC2024.Classes;
using AoC2024.Helpers;

namespace AoC2024;

public struct Dia18Input
{
    public List<Tuple<int, int>> FallingBytes { get; init; }
    public int Height { get; init; }
    public int Width { get; init; }
    public int NumberOfBytes { get; init; }
}

public class Dia18 : ProblemSolution<Dia18Input>
{
    public override int CurrentDay()
    {
        return 18;
    }

    private readonly Regex _tupleRegex = new Regex(@"(\d+),(\d+)");
    private readonly Regex _dimensionsRegex = new Regex(@"(\d+) (\d+) (\d+)");
    
    public override Dia18Input ReadInput(string[] rawInput)
    {
        List<Tuple<int, int>> fallingBytes = new List<Tuple<int, int>>();

        for (int i = 0; i < rawInput.Length - 2; i++)
        {
            var match = _tupleRegex.Match(rawInput[i]);
            var position = new Tuple<int, int>(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value)
            );
            fallingBytes.Add(position);
        }

        var dimensionsMatch = _dimensionsRegex.Match(rawInput[^1]);
        int height = int.Parse(dimensionsMatch.Groups[1].Value);
        int width = int.Parse(dimensionsMatch.Groups[2].Value);
        int numberOfBytes = int.Parse(dimensionsMatch.Groups[3].Value);

        return new Dia18Input
        {
            FallingBytes = fallingBytes,
            Height = height,
            Width = width,
            NumberOfBytes = numberOfBytes
        };
    }

    private Func<Tuple<int, int>, Tuple<int, int>[]> MakeGetNeighbours(CharMap map) =>  position =>
        Direction.Directions
            .Select(Direction.GetVector)
            .Select(vector => new Tuple<int, int>(position.Item1 + vector.Item1, position.Item2 + vector.Item2))
            .Where(it =>
            {
                var isInBounds = map.IsInBounds(it, out char value);
                if (!isInBounds)
                    return false;
                if (value == '#')
                    return false;
                return true;
            }).ToArray();
    

    public override object Part1(Dia18Input input)
    {
        var map = new CharMap(input.Height, input.Width, '.');
        for(int i =0; i<input.NumberOfBytes; i++)
            map.Set(input.FallingBytes[i], '#');
        
        var result = GraphAlgorithms.BreadthFirstSearch(
            new Tuple<int, int>(0, 0),
            MakeGetNeighbours(map));
        return result[new Tuple<int, int>(input.Height- 1, input.Width - 1)];
    }

    public override object Part2(Dia18Input input)
    {
        var map = new CharMap(input.Height, input.Width, '.');
        Tuple<int, int>? solution = null;
        int index = 0;
        while(solution == null)
        {
            var position = input.FallingBytes[index++];
            map.Set(position, '#');
            var result = GraphAlgorithms.BreadthFirstSearch(
                new Tuple<int, int>(0, 0),
                MakeGetNeighbours(map));

            bool canExit =
                result.TryGetValue(new Tuple<int, int>(input.Height - 1, input.Width - 1), out int _);

            if (!canExit)
                solution = position;
        }

        return solution.Item1 + "," + solution.Item2;
    }
}