namespace AoC2024.Helpers;

public static class FileSystemHelper
{
    public static string[] GetInput(int day, string fileName)
    {
        var path = Directory.GetCurrentDirectory();
        var text = File.ReadLines(Path.Join(path, "../../../../TestAoC2024/dia"+ day + "/"+ fileName +".txt"));
        return text.ToArray();
    }
}