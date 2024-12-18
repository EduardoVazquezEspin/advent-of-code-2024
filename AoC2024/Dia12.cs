using AoC2024.Classes;

namespace AoC2024;

public class Dia12 : ProblemSolution<CharMap>
{
    public override int CurrentDay()
    {
        return 12;
    }

    public override CharMap ReadInput(string[] rawInput)
    {
        return new CharMap(rawInput);
    }

    private List<Tuple<int, int>> _directions = new List<Tuple<int, int>>
    {
        new(1, 0),
        new(0, 1),
        new(-1, 0),
        new(0, -1)
    };

    private ulong CalculatePrice1(CharMap map)
    {
        var hasBeenAdded = map.Copy(_ => false);
        var stack = new List<Tuple<int, int>>();
        ulong price = 0;
        map.MapAllCells((_, row, column) =>
        {
            if(hasBeenAdded[row][column])
                return;
            stack.Add(new Tuple<int, int>(row, column));
            hasBeenAdded[row][column] = true;
            ulong area = 0;
            ulong perimeter = 0;
            while (stack.Any())
            {
                var top = stack[^1];
                var topValue = map.Get(top);
                stack.RemoveAt(stack.Count - 1);
                var neighbours = _directions
                    .Select(direction => new Tuple<int, int>(top.Item1 + direction.Item1, top.Item2 + direction.Item2))
                    .Where(position =>
                    {
                        var inBounds = map.IsInBounds(position, out char neighbourValue);
                        if (!inBounds)
                            return false;
                        if (neighbourValue != topValue)
                            return false;
                        return true;
                    }).ToList();
                area += 1;
                perimeter += 4L - (ulong) neighbours.Count;
                foreach (var neighbour in neighbours)
                    if (!hasBeenAdded[neighbour.Item1][neighbour.Item2])
                    {
                        hasBeenAdded[neighbour.Item1][neighbour.Item2] = true;
                        stack.Add(neighbour);
                    }
                        
            }
            
            price += area * perimeter;
        });
        return price;
    }

    public override object Part1(CharMap input)
    {
        return CalculatePrice1(input);
    }

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private readonly Direction[] _enumDirections = {Direction.Up, Direction.Right, Direction.Down, Direction.Left};

    private Tuple<int, int> GetVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return new Tuple<int, int>(-1, 0);
            case Direction.Right:
                return new Tuple<int, int>(0, 1);
            case Direction.Down:
                return new Tuple<int, int>(1, 0);
            case Direction.Left:
            default:
                return new Tuple<int, int>(0, -1);
        }
    }
    
    internal class Side : AbstractObject<Side> { }
    
    private ulong CalculatePrice2(CharMap map)
    {
        var hasBeenAdded = map.Copy(_ => false);
        var stack = new List<Tuple<int, int>>();
        ulong price = 0;
        map.MapAllCells((_, row, column) =>
        {
            if(hasBeenAdded[row][column])
                return;
            stack.Add(new Tuple<int, int>(row, column));
            hasBeenAdded[row][column] = true;
            var sideDictionary = map.Copy(_ => new Dictionary<Direction, Side>());
            var quotient = new Quotient<Side>();
            ulong area = 0;
            while (stack.Any())
            {
                var top = stack[0];
                var topValue = map.Get(top);
                stack.RemoveAt(0);
                var invalidDirections = new List<Direction>();
                var neighbours = new List<Tuple<int, int>>();
                
                foreach (var direction in _enumDirections)
                {
                    var vector = GetVector(direction);
                    var newPosition = new Tuple<int, int>(top.Item1 + vector.Item1, top.Item2 + vector.Item2);
                    var isInBounds = map.IsInBounds(newPosition, out char value);
                    if(!isInBounds || value != topValue)
                        invalidDirections.Add(direction);
                    else
                        neighbours.Add(newPosition);
                }
                
                area += 1;
                foreach (var direction in invalidDirections)
                {
                    var side = new Side();
                    sideDictionary[top.Item1][top.Item2].Add(direction, side);
                    quotient.SetClass(side);
                    foreach (var neighbour in neighbours)
                    {
                        var otherDictionary = sideDictionary[neighbour.Item1][neighbour.Item2];
                        if(otherDictionary.ContainsKey(direction))
                            quotient.SetEqual(side, otherDictionary[direction]);
                    }
                }

                foreach (var neighbour in neighbours)
                {
                    if (hasBeenAdded[neighbour.Item1][neighbour.Item2]) continue;
                    hasBeenAdded[neighbour.Item1][neighbour.Item2] = true;
                    stack.Add(neighbour);
                }
            }
            
            price += area * (ulong) quotient.ClassCount;
        });
        return price;
    }

    public override object Part2(CharMap input)
    {
        return CalculatePrice2(input);
    }
}