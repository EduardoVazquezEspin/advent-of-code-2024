namespace AoC2024.Dia21;

public class NumpadController : RobotController
{ 
    protected override char[] GetNeighbours(char c)
    {
        switch (c)
        {
            case '7':
                return new[] {'4', '8'};
            case '8':
                return new[] {'5', '7', '9'};
            case '9':
                return new[] {'6', '8'};
            case '4':
                return new[] {'1', '5', '7'};
            case '5':
                return new[] {'2', '4', '6', '8'};
            case '6':
                return new[] {'3', '5', '9'};
            case '1':
                return new[] {'2', '4'};
            case '2':
                return new[] {'0', '1', '3', '5'};
            case '3':
                return new[] {'A', '2', '6'};
            case '0':
                return new[] {'A', '2'};
            case 'A':
                return new[] {'0', '3'};
            default:
                throw new Exception("Invalid GetNeighbours Input: " + c);
        }
    }

    protected override char GetDirection(char from, char to)
    {
        switch (from.ToString() + to)
        {
            case "74":
                return 'v';
            case "78":
                return '>';
            case "85":
                return 'v';
            case "87":
                return '<';
            case "89":
                return '>';
            case "96":
                return 'v';
            case "98":
                return '<';
            case "41":
                return 'v';
            case "45":
                return '>';
            case "47":
                return '^';
            case "52":
                return 'v';
            case "54":
                return '<';
            case "56":
                return '>';
            case "58":
                return '^';
            case "63":
                return 'v';
            case "65":
                return '<';
            case "69":
                return '^';
            case "12":
                return '>';
            case "14":
                return '^';
            case "20":
                return 'v';
            case "21":
                return '<';
            case "23":
                return '>';
            case "25":
                return '^';
            case "3A":
                return 'v';
            case "32":
                return '<';
            case "36":
                return '^';
            case "02":
                return '^';
            case "0A":
                return '>';
            case "A0":
                return '<';
            case "A3":
                return '^';
            default:
                throw new Exception("Invalid GetDirection Input: " + from + to);
        }
    }
}