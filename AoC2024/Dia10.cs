using AoC2024.Classes;

namespace AoC2024;

public struct Dia10Input
{
    public CharMap<int> Map { get; init; }
    public List<Tuple<int, int>> Trailheads { get; init; }
}

public class Dia10 : ProblemSolution<Dia10Input>
{
    public override int CurrentDay()
    {
        return 10;
    }

    public override Dia10Input ReadInput(string[] rawInput)
    {
        var trailheads = new List<Tuple<int, int>>();
        var Mapper = (char c, int i, int j) =>
        {
            if(c == '0')
                trailheads.Add(new Tuple<int, int>(i, j));
            return Int32.Parse(c.ToString());
        };
        var map = new CharMap<int>(rawInput, Mapper);
        return new Dia10Input
        {
            Map = map,
            Trailheads = trailheads
        };
    }

    private List<Tuple<int, int>> _directions = new List<Tuple<int, int>>
    {
        new (1, 0),
        new (0, 1),
        new (-1, 0),
        new (0, -1)
    }; 

    private int CountTrails(CharMap<int> map, Tuple<int, int> origin)
    {
        var hasBeenVisited = map.Copy(() => false);
        var stack = new List<Tuple<int, int>>{ origin };
        while (stack.Any())
        {
            var top = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            if (!hasBeenVisited[top.Item1][top.Item2])
            {
                hasBeenVisited[top.Item1][top.Item2] = true;
                var value = map.Get(top);
                foreach (var direction in _directions)
                {
                    var newPosition = new Tuple<int, int>(top.Item1 + direction.Item1, top.Item2 + direction.Item2);
                    if(map.IsInBounds(newPosition, out int neighbour) && neighbour == value + 1)
                        stack.Add(newPosition);
                }
            }
        }
        
        return map.MapAllCells((value, i, j) => value == 9 && hasBeenVisited[i][j] ? 1 : 0).Sum();
    }

    private int CalculateRating(CharMap<int> map, Tuple<int, int> origin)
    {
        var numberOfVisits = map.Copy(() => 0);
        var hasBeenVisited = map.Copy(() => false);
        var queue = new List<Tuple<int, int>>{ origin };
        numberOfVisits[origin.Item1][origin.Item2] = 1;
        hasBeenVisited[origin.Item1][origin.Item2] = true;
        while (queue.Any())
        {
            var top = queue[0];
            queue.RemoveAt(0);
            var value = map.Get(top);
            var visits = numberOfVisits[top.Item1][top.Item2];
            foreach (var direction in _directions)
            {
                var newPosition = new Tuple<int, int>(top.Item1 + direction.Item1, top.Item2 + direction.Item2);
                if (map.IsInBounds(newPosition, out int neighbour) && neighbour == value + 1)
                {
                    numberOfVisits[newPosition.Item1][newPosition.Item2] += visits;
                    if (!hasBeenVisited[newPosition.Item1][newPosition.Item2])
                    {
                        queue.Add(newPosition);
                        hasBeenVisited[newPosition.Item1][newPosition.Item2] = true;   
                    }
                }
            }
        }

        return map.MapAllCells((value, i, j) => value == 9 && hasBeenVisited[i][j] ? numberOfVisits[i][j] : 0).Sum();
    }

    public override object Part1(Dia10Input input)
    {
        return input.Trailheads.Aggregate(0, (acc, curr) => acc + CountTrails(input.Map, curr));
    }

    public override object Part2(Dia10Input input)
    {
        return input.Trailheads.Aggregate(0, (acc, curr) => acc + CalculateRating(input.Map, curr));
    }
}