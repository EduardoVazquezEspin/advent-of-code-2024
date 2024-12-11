namespace AoC2024;

public struct Dia9Input
{
    public int[] Memory { get; set; }
    public List<Tuple<int, int>> FileLocations { get; init; }
}

public class Dia9 : ProblemSolution<Dia9Input>
{
    public override int CurrentDay()
    {
        return 9;
    }

    public override Dia9Input ReadInput(string[] rawInput)
    {
        var line = rawInput[0];
        var totalMemory = 0;
        for (int i = 0; i < line.Length; i++)
            totalMemory += Int32.Parse(line[i].ToString());

        var memory = new int[totalMemory];
        
        var index = 0;
        var fileLocations = new List<Tuple<int, int>>();
        for (int i = 0; i < line.Length; i++)
        {
            var value = Int32.Parse(line[i].ToString());
            var sum = index + value;
            var fileNumber = i % 2 == 0 ? i / 2 : -1;
            if (fileNumber != -1)
                fileLocations.Add(new Tuple<int, int>(index, sum));

            for (int j = index; j < sum; j++)
                memory[j] = fileNumber;

            index = sum;
        }

        return new Dia9Input
        {
            Memory = memory,
            FileLocations = fileLocations
        };
    }

    private bool Fragmentate(int[] memory, Tuple<int, int> fileLocation, ref int memoryIndex)
    {
        for (int index = fileLocation.Item2 - 1; index >= fileLocation.Item1; index--)
        {
            while (memory[memoryIndex] != -1)
                memoryIndex++;
            if (memoryIndex >= index)
                return false;
            (memory[memoryIndex], memory[index]) = (memory[index], memory[memoryIndex]);
        }
        return true;
    }
    
    private void Defragmentate(int[] memory, Tuple<int, int> fileLocation)
    {
        var length = fileLocation.Item2 - fileLocation.Item1;
        var index = 0;
        do
        {
            var numberOfSpaces = 0;
            var origin = index;
            while (numberOfSpaces < length && memory[index++] == -1)
                numberOfSpaces++;
            if (numberOfSpaces == length)
            {
                for (int i = 0; i < length; i++)
                    (memory[origin + i], memory[fileLocation.Item1 + i]) =
                        (memory[fileLocation.Item1 + i], memory[origin + i]);
            }
        } while (index < memory.Length - length && index < fileLocation.Item1);
    }

    public long EvaluateMemory(int[] memory)
    {
        long total = 0;
        for (int i = 0; i < memory.Length; i++)
        {
            if (memory[i] != -1)
                total += i * memory[i];
        }
        return total;
    }

    public void PrintMemory(int[] memory)
    {
        for(int i =0; i<memory.Length; i++)
            Console.Write(memory[i] == -1 ? '.' : (memory[i] % 10).ToString());
        Console.WriteLine();
    }

    public override IFormattable Part1(Dia9Input input)
    {
        int memoryIndex = 0;
        int locationIndex = input.FileLocations.Count - 1;
        while (Fragmentate(input.Memory, input.FileLocations[locationIndex], ref memoryIndex))
            locationIndex--;
        return EvaluateMemory(input.Memory);
    }

    public override IFormattable Part2(Dia9Input input)
    {
        for (int locationIndex = input.FileLocations.Count - 1; locationIndex >= 0; locationIndex--)
            Defragmentate(input.Memory, input.FileLocations[locationIndex]);

        return EvaluateMemory(input.Memory);
    }
}