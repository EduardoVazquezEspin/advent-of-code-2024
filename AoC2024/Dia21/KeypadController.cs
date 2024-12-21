namespace AoC2024.Dia21;

public class KeypadController : RobotController
{
    protected override char[] GetNeighbours(char c)
    {
        switch (c)
        {
            case '^':
                return new[] {'v', 'A'};
            case 'A':
                return new[] {'>', '^'};
            case '<':
                return new[] {'v'};
            case 'v':
                return new[] {'<', '^', '>'};
            case '>':
                return new[] {'v', 'A'};
            default:
                throw new Exception("Invalid GetNeighbours Input: " + c);
        }
    }

    protected override char GetDirection(char @from, char to)
    {
        switch (from.ToString() + to)
        {
            case "^v":
                return 'v';
            case "^A":
                return '>';
            case "A>":
                return 'v';
            case "A^":
                return '<';
            case "<v":
                return '>';
            case "v<":
                return '<';
            case "v^":
                return '^';
            case "v>":
                return '>';
            case ">v":
                return '<';
            case ">A":
                return '^';
            default:
                throw new Exception("Invalid GetDirection Input: " + from + to);
        }
    }
}