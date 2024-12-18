using AoC2024.Classes;
using AoC2024.Helpers;

namespace AoC2024;

public class Player
{
    public Tuple<int, int> Position { get;  }
    public Direction Orientation { get; }

    public Player()
    {
        Position = new Tuple<int, int>(0, 0);
        Orientation = new Direction('>');
    }

    public Player(Tuple<int, int> position, Direction orientation)
    {
        Position = position;
        Orientation = orientation;
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is Player objPlayer))
            return false;

        if (objPlayer.Position.Item1 != Position.Item1)
            return false;
        
        if (objPlayer.Position.Item2 != Position.Item2)
            return false;
        
        return objPlayer.Orientation.Equals(Orientation);
    }

    public override int GetHashCode()
    {
        var hash = Position.Item1;
        hash += Position.Item2 << 5;
        hash += Orientation.GetHashCode();
        return hash;
    }
}

public struct Dia16Input
{
    public CharMap Map { get; init; }
    public Player Player { get; init; }
}

public class Dia16 : ProblemSolution<Dia16Input>
{
    public override int CurrentDay()
    {
        return 16;
    }

    public override Dia16Input ReadInput(string[] rawInput)
    {
        Player player = new Player();
        var map = new CharMap(rawInput, (c, i, j) =>
        {
            if (c == 'S')
                player = new Player(new Tuple<int, int>(i, j), new Direction('>'));
            return c;
        });

        return new Dia16Input
        {
            Map = map,
            Player = player
        };
    }

    private Func<Player, Tuple<Player, int>[]> MakeGetNeighbours(CharMap map, bool isInverted = false) => player =>
    {
        var options = new List<Tuple<Player, int>>
        {
            new(new Player(player.Position, player.Orientation.Copy().Turn90DegreesLeft()), 1000),
            new(new Player(player.Position, player.Orientation.Copy().Turn90DegreesRight()), 1000)
        };

        var vector = player.Orientation.GetVector();
        var newPosition =
            new Tuple<int, int>(player.Position.Item1 + vector.Item1, player.Position.Item2 + vector.Item2);
        if(isInverted)
            newPosition = new Tuple<int, int>(player.Position.Item1 - vector.Item1, player.Position.Item2 - vector.Item2);
        var value = map.Get(newPosition);
        if (value != '#')
            options.Add(new(new Player(newPosition, player.Orientation), 1));

        return options.ToArray();
    };
    
    public override object Part1(Dia16Input input)
    {
        Player? finalPosition = null;
        var results = GraphAlgorithms.Dijkstra(
            input.Player, 
            MakeGetNeighbours(input.Map),
            player => input.Map.Get(player.Position) == 'E',
            player =>
            {
                if (finalPosition == null && input.Map.Get(player.Position) == 'E')
                    finalPosition = player;
            });

        return results[finalPosition!];
    }

    public override object Part2(Dia16Input input)
    {
        Player? finalPosition = null;
        var results = GraphAlgorithms.Dijkstra(
            input.Player, 
            MakeGetNeighbours(input.Map),
            _ => false,
            player =>
            {
                if (finalPosition == null && input.Map.Get(player.Position) == 'E')
                    finalPosition = player;
            });

        var queue = new List<Player> {finalPosition!};
        var quotient = new Quotient<Player>();
        var getNeighbours = MakeGetNeighbours(input.Map, true);

        while (queue.Any())
        {
            var top = queue[0];
            var distance = results[top];
            queue.RemoveAt(0);
            quotient.SetClass(top);
            var neighbours = getNeighbours(top);
            foreach (var (neighbour, neighbourDistance) in neighbours)
            {
                if(quotient.HasClass(neighbour))
                    continue;
                if(!results.ContainsKey(neighbour))
                    continue;
                if(distance < neighbourDistance + results[neighbour])
                    continue;
                if(neighbourDistance == 1000)
                    quotient.SetEqual(top, neighbour);
                queue.Add(neighbour);
            }
        }

        return quotient.ClassCount;
    }
}