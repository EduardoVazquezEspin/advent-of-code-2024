using AoC2024.Helpers;

namespace AoC2024;

public struct Dia8Input
{
    public string[] Map { get; init; }
    public Dictionary<char,List<Tuple<int, int>>> AntennaSets { get; init; } 
}

public class Dia8 : ProblemSolution<Dia8Input>
{
    public override int CurrentDay()
    {
        return 8;
    }

    public override Dia8Input ReadInput(string[] rawInput)
    {
        var sets = new Dictionary<char, List<Tuple<int, int>>>();

        for(int i =0; i<rawInput.Length; i++)
        for (int j = 0; j < rawInput[i].Length; j++)
        {
            var symbol = rawInput[i][j];
            if (symbol != '.')
            {
                if(!sets.ContainsKey(symbol))
                    sets.Add(symbol, new List<Tuple<int, int>>());
                var dict = sets[symbol];
                dict.Add(new Tuple<int, int>(i, j));
            }
        }
        
        return new Dia8Input
        {
            Map = rawInput,
            AntennaSets = sets
        };
    }
    
    private bool IsInBounds(string[] map, Tuple<int, int> position)
    {
        if (position.Item1 < 0)
            return false;
        if (position.Item2 < 0)
            return false;
        if (position.Item1 >= map.Length)
            return false;
        if (position.Item2 >= map[position.Item1].Length)
            return false;
        return true;
    }

    public override object Part1(Dia8Input input)
    {
        var nodeMap = new char[input.Map.Length][];
        for (int i = 0; i < nodeMap.Length; i++)
        {
            var row = new char[input.Map[i].Length];
            for (int j = 0; j < input.Map[i].Length; j++)
                row[j] = '.';
            nodeMap[i] = row;
        }

        foreach (var list in input.AntennaSets.Values)
        {
            for(int i =0; i<list.Count-1; i++)
            for (int j = i + 1; j < list.Count; j++)
            {
                var node1 = list[i];
                var node2 = list[j];
                var antiNode1 = new Tuple<int, int>(2 * node1.Item1 - node2.Item1, 2 * node1.Item2 - node2.Item2);
                var antiNode2 = new Tuple<int, int>(2 * node2.Item1 - node1.Item1, 2 * node2.Item2 - node1.Item2);
                if (IsInBounds(input.Map, antiNode1))
                    nodeMap[antiNode1.Item1][antiNode1.Item2] = '#';
                if (IsInBounds(input.Map, antiNode2))
                    nodeMap[antiNode2.Item1][antiNode2.Item2] = '#';
            }
        }

        var total = 0;
        for(int i =0; i<nodeMap.Length; i++)
        for (int j = 0; j < nodeMap[i].Length; j++)
            total += nodeMap[i][j] == '#' ? 1 : 0;
        return total;
    }

    public override object Part2(Dia8Input input)
    {
        var nodeMap = new char[input.Map.Length][];
        for (int i = 0; i < nodeMap.Length; i++)
        {
            var row = new char[input.Map[i].Length];
            for (int j = 0; j < input.Map[i].Length; j++)
                row[j] = '.';
            nodeMap[i] = row;
        }

        foreach (var list in input.AntennaSets.Values)
        {
            for(int i =0; i<list.Count-1; i++)
            for (int j = i + 1; j < list.Count; j++)
            {
                var node1 = list[i];
                var node2 = list[j];
                var diff = new Tuple<int, int>(node1.Item1 - node2.Item1, node1.Item2 - node2.Item2);
                var gcd = ArithmeticAlgorithms.GreatestCommonDivisor(diff.Item1, diff.Item2);
                var normalizedDiff = new Tuple<int, int>(diff.Item1 / gcd, diff.Item2 / gcd);
                var position = new Tuple<int, int>(node1.Item1, node1.Item2);
                while (IsInBounds(input.Map, position))
                {
                    nodeMap[position.Item1][position.Item2] = '#';
                    position = new Tuple<int, int>(position.Item1 + normalizedDiff.Item1,
                        position.Item2 + normalizedDiff.Item2);
                }
                position = new Tuple<int, int>(node1.Item1 - normalizedDiff.Item1, node1.Item2 - normalizedDiff.Item2);
                while (IsInBounds(input.Map, position))
                {
                    nodeMap[position.Item1][position.Item2] = '#';
                    position = new Tuple<int, int>(position.Item1 - normalizedDiff.Item1,
                        position.Item2 - normalizedDiff.Item2);
                }
            }
        }

        var total = 0;
        for(int i =0; i<nodeMap.Length; i++)
        for (int j = 0; j < nodeMap[i].Length; j++)
            total += nodeMap[i][j] == '#' ? 1 : 0;

        return total;
    }
}