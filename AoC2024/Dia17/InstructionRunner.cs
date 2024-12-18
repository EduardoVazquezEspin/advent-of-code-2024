namespace AoC2024.Dia17;

public abstract class InstructionRunner
{
    protected Func<char, long> GetRegister;
    protected Action<char, long> SetRegister;
    protected Func<long> GetCombo;
    protected Func<long> GetLiteral;
    protected Action<long> Output;
    protected Func<int> GetPointer;
    protected Action<int> SetPointer;

    protected InstructionRunner(
        Func<char, long> getRegister, 
        Action<char, long> setRegister, 
        Func<long> getCombo,
        Func<long> getLiteral,
        Action<long> output,
        Func<int> getPointer,
        Action<int> setPointer
    )
    {
        this.GetRegister = getRegister;
        this.SetRegister = setRegister;
        this.GetCombo = getCombo;
        this.GetLiteral = getLiteral;
        this.Output = output;
        this.GetPointer = getPointer;
        this.SetPointer = setPointer;
    }

    public virtual void Run()
    {
        var index = this.GetPointer();
        this.SetPointer(index + 1);
    }
}