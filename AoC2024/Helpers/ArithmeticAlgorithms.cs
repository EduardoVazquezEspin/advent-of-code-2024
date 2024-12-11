namespace AoC2024.Helpers;

public static class ArithmeticAlgorithms
{
    public static int GreatestCommonDivisor(int a, int b) => b == 0 ? a : GreatestCommonDivisor(b, a % b);

    public static int GreatestCommonDivisor(int[] values)
    {
        if (values.Length == 0)
            return 0;
        
        int result = values[0];
        for(int i= 1; i<values.Length; i++)
            result = GreatestCommonDivisor(result, values[i]);

        return result;
    }

    public static uint NumberOfDigits(IFormattable number) => (uint) number.ToString()!.Length;
    
    public static ulong Power(ulong x, uint n)
    {
        if (n <= 0)
            return 1;
        var squareRoot = Power(x, n / 2);
        var result = squareRoot * squareRoot;
        if (n % 2 == 1)
            result *= x;
        return result;
    }
}