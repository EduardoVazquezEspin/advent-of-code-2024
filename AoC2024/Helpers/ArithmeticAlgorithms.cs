namespace AoC2024.Helpers;

public static class ArithmeticAlgorithms
{
    public static int GreatestCommonDivisor(int a, int b) => b == 0 ? a : GreatestCommonDivisor(b, a % b);

    public static int GreatestCommonDivisor(int[] values)
    {
        if (values.Length == 0)
            throw new Exception("Infinite Value Achieved");
        
        int result = values[0];
        for(int i= 1; i<values.Length; i++)
            result = GreatestCommonDivisor(result, values[i]);

        return result;
    }
}