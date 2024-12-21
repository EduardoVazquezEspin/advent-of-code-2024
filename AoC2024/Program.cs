// See https://aka.ms/new-console-template for more information

// using AoC2024;
// using AoC2024.Helpers;

// var solution = new Dia15();
// var rawInput = FileSystemHelper.GetInput(solution.CurrentDay(), "part2");
// var input = solution.ReadInput(rawInput);
// Console.WriteLine(solution.Part2(input));
// Console.ReadLine();

var order = new char[] {'<', '>', '^', 'v', 'A'};

var array = "<>^vA<>^vA<>^vA<>^vA<>^vA".ToArray();
Array.Sort(array, (c, c1) => Array.IndexOf(order, c) - Array.IndexOf(order, c1));

Console.WriteLine(array.Aggregate("", (acc, curr) => acc +curr));
Console.ReadLine();