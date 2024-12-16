// See https://aka.ms/new-console-template for more information

using AoC2024;
using AoC2024.Helpers;

var solution = new Dia15();
var rawInput = FileSystemHelper.GetInput(solution.CurrentDay(), "part2");
var input = solution.ReadInput(rawInput);
Console.WriteLine(solution.Part2(input));
Console.ReadLine();

