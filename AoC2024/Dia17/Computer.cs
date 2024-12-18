namespace AoC2024.Dia17;

public class Computer
{
    private long _registerA;
    private long _registerB;
    private long _registerC;

    private int _index;
    private int[] _instructions = Array.Empty<int>();
    private readonly List<long> _output;

    private readonly InstructionRunner[] _runners;
    
    public Computer(long registerA, long registerB, long registerC)
    {
        _registerA = registerA;
        _registerB = registerB;
        _registerC = registerC;

        _index = 0;
        _output = new List<long>();

        _runners = new InstructionRunner[]
        {
            new adv(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new bxl(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new bst(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new jnz(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new bxc(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new ovt(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new bdv(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer),
            new cdv(GetRegister,SetRegister, GetCombo, GetLiteral, Output, GetPointer, SetPointer)
        };
    }

    public List<long> Run(int[] instructions, int maxOutputSize = 0, int maxMs = 0)
    {
        this._instructions = instructions;
        var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        while (
            _index < instructions.Length && 
            _index >= 0 && 
            (maxOutputSize == 0 || _output.Count <= maxOutputSize)  &&
            (maxMs == 0 || current - start < maxMs)
        )
        {
            var runner = _runners[_instructions[_index]];
            _index++;
            runner.Run();
            current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        if (maxOutputSize != 0 && _output.Count > maxOutputSize)
            throw new Exception("Maximum Output Size Surpassed");

        if (maxMs != 0 && current - start >= maxMs)
            throw new Exception("Timeout");
        
        return _output;
    }

    private long GetRegister(char c)
    {
        switch (c)
        {
            case 'A':
                return _registerA;
            case 'B':
                return _registerB;
            case 'C':
                return _registerC;
            default:
                throw new Exception("Invalid Register Identifier");
        }
    }

    private void SetRegister(char c, long value)
    {
        switch (c)
        {
            case 'A':
                _registerA = value;
                break;
            case 'B':
                _registerB = value;
                break;
            case 'C':
                _registerC = value;
                break;
            default:
                throw new Exception("Invalid Register Identifier");
        }
    }

    private long GetCombo()
    {
        if (_index >= _instructions.Length && _index < 0)
            throw new Exception("Invalid Index Position: " + _index);

        var operand = _instructions[_index];
        switch (operand)
        {
            case 4:
                return _registerA;
            case 5:
                return _registerB;
            case 6:
                return _registerC;
            case 7:
                throw new Exception("Invalid Combo Index 7");
            default:
                return operand;
        }
    }

    private long GetLiteral()
    {
        if (_index >= _instructions.Length && _index < 0)
            throw new Exception("Invalid Index Position: " + _index);
        return _instructions[_index];
    }

    private void Output(long output)
    {
        _output.Add(output);
    }

    private int GetPointer() => _index;

    private void SetPointer(int index) => _index = index;
}