using AoC2024.Helpers;

namespace AoC2024.Dia21;

public abstract class RobotController
{
    protected abstract char[] GetNeighbours(char c);
    
    protected abstract char GetDirection(char from, char to);

    private int EvaluateTransition(char from, char to) => from != to ? 1024 : 1;

    private readonly char[] _charOrder = new[] { '<', 'v', '^', '>' };
    
    private string Sort(string input, bool flipped)
    {
        var array = input.ToArray();
        Array.Sort(array, (c, c1) => (flipped ? -1 : 1) *(Array.IndexOf(_charOrder, c)-Array.IndexOf(_charOrder, c1)) );
        return array.Aggregate("", (acc, curr) => acc + curr );
    }

    private bool IsPathValid(char start, string path)
    {
        var current = start;
        foreach (var step in path)
        {
            var neighbours = GetNeighbours(current);
            var valid = neighbours
                .Select(it => new Tuple<char, char>(it,GetDirection(current, it)))
                .Where(it => step == it.Item2)
                .ToArray();

            if (valid.Length == 0)
                return false;
            current = valid[0].Item1;
        }

        return true;
    }

    private string[] GetAllFlips(char start, string result)
    {
        var first = Sort(result, true);
        var second = Sort(result, false);
        if (first == second)
            return new[] {result};

        var solution = new List<string>();
        if(IsPathValid(start, first))
            solution.Add(first);

        if(IsPathValid(start, second))
            solution.Add(second);

        return solution.ToArray();
    }

    private string GetSortedSequence(char start, string result)
    {
        var sorted = Sort(result, false);
        if (IsPathValid(start, sorted))
            return sorted;
        return result;
    }
    
    private struct DirectionNode
    {
        public char Position { get; init; }
        public char Direction { get; init; }
    }
    
    public string[] GetInput(string input)
    {
        var position = 'A';
        var result = new List<string> { "" };
        DirectionNode finalNode = default;
        foreach (var node in input)
        {
            GraphAlgorithms.Dijkstra(new []
                {
                    new DirectionNode{ Position = position, Direction = '^'},
                    new DirectionNode{ Position = position, Direction = '>'},
                    new DirectionNode{ Position = position, Direction = '<'},
                    new DirectionNode{ Position = position, Direction = 'v'},
                }, (directionNode => GetNeighbours(directionNode.Position)
                        .Select(neighbour =>
                            new DirectionNode
                            {
                                Position = neighbour,
                                Direction = GetDirection(directionNode.Position, neighbour)
                            })
                        .Select(neighbourNode => new Tuple<DirectionNode, int>(
                            neighbourNode,
                            EvaluateTransition(directionNode.Direction, neighbourNode.Direction))
                        )
                        .ToArray()
                ), 
                out var comesFrom, 
                target=>
                {
                    if (target.Position != node)
                        return false;
                    finalNode = target;
                    return true;
                });

            string sequence = "";
            var hasPreviousValue = true;
            
            DirectionNode current = finalNode;
            
            while (hasPreviousValue)
            {
                hasPreviousValue = comesFrom.TryGetValue(current, out var previous);
                if (hasPreviousValue)
                {
                    sequence = current.Direction + sequence;
                    current = previous;
                }
            }

            position = node;
            var newResult = new List<string>();
            var sequences = GetAllFlips(current.Position, sequence);
            foreach (var solution in result)
            {
                foreach (var seq in sequences)
                {
                    newResult.Add(solution + seq + 'A');
                }
            }

            result = newResult;
        }

        return result.ToArray();
    }
    
        public string GetGreedyInput(string input)
    {
        var position = 'A';
        var result = "";
        DirectionNode finalNode = default;
        foreach (var node in input)
        {
            GraphAlgorithms.Dijkstra(new []
                {
                    new DirectionNode{ Position = position, Direction = '^'},
                    new DirectionNode{ Position = position, Direction = '>'},
                    new DirectionNode{ Position = position, Direction = '<'},
                    new DirectionNode{ Position = position, Direction = 'v'},
                }, (directionNode => GetNeighbours(directionNode.Position)
                        .Select(neighbour =>
                            new DirectionNode
                            {
                                Position = neighbour,
                                Direction = GetDirection(directionNode.Position, neighbour)
                            })
                        .Select(neighbourNode => new Tuple<DirectionNode, int>(
                            neighbourNode,
                            EvaluateTransition(directionNode.Direction, neighbourNode.Direction))
                        )
                        .ToArray()
                ), 
                out var comesFrom, 
                target=>
                {
                    if (target.Position != node)
                        return false;
                    finalNode = target;
                    return true;
                });

            string sequence = "";
            var hasPreviousValue = true;
            
            DirectionNode current = finalNode;
            
            while (hasPreviousValue)
            {
                hasPreviousValue = comesFrom.TryGetValue(current, out var previous);
                if (hasPreviousValue)
                {
                    sequence = current.Direction + sequence;
                    current = previous;
                }
            }

            position = node;
            result += GetSortedSequence(current.Position, sequence) + 'A';
        }

        return result;
    }
}