// See https://aka.ms/new-console-template for more information

using AoC2024;

var _solution = new Dia14();
var path = Directory.GetCurrentDirectory();
var finalPath = Path.Join(path, "../../../../TestAoC2024/dia" + _solution.CurrentDay() + "/part2.txt");
var text = File.ReadLines(Path.Join(path, "../../../../TestAoC2024/dia"+ _solution.CurrentDay() + "/part2.txt"));
var array = text.ToArray();
var input = _solution.ReadInput(array);
_solution.Part2(input);

