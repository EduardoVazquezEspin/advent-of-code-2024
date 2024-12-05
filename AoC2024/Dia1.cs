using System.Text.RegularExpressions;

namespace AoC2024;

using Dia1Input = Tuple<List<int>, List<int>>;
public class Dia1 : ProblemSolution<Tuple<List<int>, List<int>>>
{
    public override int CurrentDay() => 1;
    public override Tuple<List<int>, List<int>> ReadInput(string[] rawInput)
    {
        var regex = new Regex(@"(\d+)\s+(\d+)");
        var firstList = new List<int>();
        var secondList = new List<int>();
        foreach (var line in rawInput)
        {
            var match = regex.Match(line);
            firstList.Add(Int32.Parse(match.Groups[1].Value));
            secondList.Add(Int32.Parse(match.Groups[2].Value));
        }
        return new Tuple<List<int>, List<int>>(firstList, secondList);
    }

    public override IFormattable Part1(Tuple<List<int>, List<int>> input)
    {
        var firstList = input.Item1;
        var secondList = input.Item2;
        firstList.Sort();
        secondList.Sort();
        var total = 0;
        for (int i = 0; i < firstList.Count; i++)
            total += Math.Abs(firstList[i] - secondList[i]);
        return total;
    }

    public override IFormattable Part2(Tuple<List<int>, List<int>> input)
    {
        var firstList = input.Item1;
        var secondList = input.Item2;
        firstList.Sort();
        secondList.Sort();
        var total = 0;
        var secondListIndex = 0;
        for (int i = 0; i < firstList.Count; i++)
        {
            var number = firstList[i];
            var appearsOnLeft = 1;
            var appearsOnRight = 0;
            i++;
            while (i < firstList.Count && firstList[i] == number)
            {
                appearsOnLeft++;
                i++;
            }
            i--;
            while (secondListIndex < secondList.Count && secondList[secondListIndex] < number) secondListIndex++;
            while (secondListIndex < secondList.Count && secondList[secondListIndex] == number)
            {
                secondListIndex++;
                appearsOnRight++;
            }

            total += number * appearsOnLeft * appearsOnRight;
        }
            
        return total;
    }
}