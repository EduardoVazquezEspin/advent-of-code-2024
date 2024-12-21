using System.Text.RegularExpressions;

namespace AoC2024.Dia21;

public class Dia21 : ProblemSolution<string[]>
{
    public override int CurrentDay()
    {
        return 21;
    }

    public override string[] ReadInput(string[] rawInput) => rawInput;

    private readonly Regex _numberRegex = new Regex(@"\d+");
    public override object Part1(string[] input)
    {
        var numpadController = new NumpadController();
        var keypadController = new KeypadController();
        long total = 0L;
        
        foreach (var line in input)
        {
            var numValue = long.Parse(_numberRegex.Match(line).Value);
            var robot1Input = numpadController.GetInput(line);
            var robot2Input = robot1Input.SelectMany(it => keypadController.GetInput(it)).ToArray();
            var robot3Input = robot2Input.SelectMany(it => keypadController.GetInput(it)).ToArray();
            var minSolution = robot3Input.Select(it => it.Length).Min();
            
            total += numValue * minSolution;
        }

        return total;
    }

    public override object Part2(string[] input)
    {
        var numpadController = new NumpadController();
        var keypadController = new KeypadController();
        long total = 0L;
        
        foreach (var line in input)
        {
            var numValue = long.Parse(_numberRegex.Match(line).Value);
            var robotInput = numpadController.GetInput(line);
            var minLength = robotInput.Select(it => it.Length).Min();
            robotInput = robotInput.Where(it => it.Length == minLength).ToArray();
            for (int i = 0; i < 25; i++)
            {
                robotInput = robotInput.SelectMany(it => keypadController.GetInput(it)).ToArray();
                minLength = robotInput.Select(it => it.Length).Min();
                robotInput = robotInput.Where(it => it.Length == minLength).ToArray();
            }
            
            total += numValue * minLength;
        }

        return total;
    }
}