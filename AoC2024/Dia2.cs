using System.Text.RegularExpressions;

namespace AoC2024;

using Dia2Input = List<List<int>>;
public class Dia2 : ProblemSolution<List<List<int>>>
{
    public override int CurrentDay() => 2;

    public override List<List<int>> ReadInput(string[] rawInput)
    {
        List<List<int>> result = new List<List<int>>();
        var regex = new Regex(@"\d+");
        foreach (var line in rawInput)
        {
            List<int> report = new List<int>();
            foreach (var match in regex.Matches(line))
            {
                var value = (match as Match)!.Value;
                report.Add(Int32.Parse(value));
            }
            result.Add(report);
        }
        return result;
    }

    private bool IsReportSafe(List<int> report)
    {
        if (report.Count < 2)
            return true;
        var startingDiff = report[1] - report[0];
        if (startingDiff == 0)
            return false;
        var sign = Math.Sign(startingDiff);
        for (int i = 0; i < report.Count - 1; i++)
        {
            var diff = report[i + 1] - report[i];
            var abs = Math.Abs(diff);
            if (abs < 1 || abs > 3)
                return false;
            if (Math.Sign(diff) != sign)
                return false;
        }

        return true;
    }

    private bool IsReportSafeWithoutElement(List<int> report, int index)
    {
        var copy = new List<int>();
        for (int i = 0; i < report.Count; i++)
        {
            if(i != index)
                copy.Add(report[i]);
        }
        return IsReportSafe(copy);
    }

    public override object Part1(List<List<int>> input)
    {
        var total = 0;
        for (int i = 0; i < input.Count; i++)
            total += IsReportSafe(input[i]) ? 1 : 0;
        return total;
    }

    public override object Part2(List<List<int>> input)
    {
        var total = 0;
        for (int i = 0; i < input.Count; i++)
        {
            var report = input[i];
            var isSafe = IsReportSafe(report);
            for (int index = 0; index < report.Count && !isSafe; index++)
                isSafe = IsReportSafeWithoutElement(report, index);
            total += isSafe ? 1 : 0;
        }
        return total;
    }
}