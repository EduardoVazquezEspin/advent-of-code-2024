using AoC2024.Classes;
using AoC2024.Helpers;

namespace AoC2024;

public class Cell
{
    public char Value { get; init; }
    public bool HasAntenna { get; set; }
}

public struct Dia8Alt1Input
{
    public CharMap<Cell> Map { get; init; }
    public Dictionary<char,List<Tuple<int, int>>> AntennaSets { get; init; } 
}

public class Dia8Alt1 : ProblemSolution<Dia8Alt1Input>
{
    public override int CurrentDay()
    {
        return 8;
    }

    public override Dia8Alt1Input ReadInput(string[] rawInput)
    {
        var sets = new Dictionary<char, List<Tuple<int, int>>>();

        var map = new CharMap<Cell>(rawInput, (c, i, j) =>
        {
            if (c != '.')
            {
                if (!sets.ContainsKey(c))
                    sets.Add(c, new List<Tuple<int, int>>());
                var list = sets[c];
                list.Add(new Tuple<int, int>(i, j));
            }
            return new Cell {Value = c, HasAntenna = false};
        });
        
        return new Dia8Alt1Input
        {
            Map = map,
            AntennaSets = sets
        };
    }

    public override IFormattable Part1(Dia8Alt1Input input)
    {
        foreach (var list in input.AntennaSets.Values)
        {
            for(int i =0; i<list.Count-1; i++)
            for (int j = i + 1; j < list.Count; j++)
            {
                var node1 = list[i];
                var node2 = list[j];
                var antiNode1 = new Tuple<int, int>(2 * node1.Item1 - node2.Item1, 2 * node1.Item2 - node2.Item2);
                var antiNode2 = new Tuple<int, int>(2 * node2.Item1 - node1.Item1, 2 * node2.Item2 - node1.Item2);
                if (input.Map.IsInBounds(antiNode1, out Cell cell))
                    cell.HasAntenna = true;
                if (input.Map.IsInBounds(antiNode2, out cell))
                    cell.HasAntenna = true;
            }
        }
        
        return input.Map.MapAllCells(c => c.HasAntenna ? 1 : 0).Sum();
    }

    public override IFormattable Part2(Dia8Alt1Input input)
    {
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
                while (input.Map.IsInBounds(position, out Cell cell))
                {
                    cell.HasAntenna = true;
                    position = new Tuple<int, int>(position.Item1 + normalizedDiff.Item1,
                        position.Item2 + normalizedDiff.Item2);
                }
                position = new Tuple<int, int>(node1.Item1 - normalizedDiff.Item1, node1.Item2 - normalizedDiff.Item2);
                while (input.Map.IsInBounds(position, out Cell cell))
                {
                    cell.HasAntenna = true;
                    position = new Tuple<int, int>(position.Item1 - normalizedDiff.Item1,
                        position.Item2 - normalizedDiff.Item2);
                }
            }
        }
        
        return input.Map.MapAllCells(cell => cell.HasAntenna ? 1 : 0).Sum();
    }
}