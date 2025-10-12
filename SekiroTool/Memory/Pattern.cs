namespace SekiroTool.Memory;

public class Pattern(
    byte[] bytes,
    string mask,
    int instructionOffset,
    AddressingMode addressingMode,
    int offsetLocation = 0,
    int instructionLength = 0)
{
    public byte[] Bytes { get; } = bytes;
    public string Mask { get; } = mask;
    public int InstructionOffset { get; } = instructionOffset;
    public AddressingMode AddressingMode { get; } = addressingMode;
    public int OffsetLocation { get; } = offsetLocation;
    public int InstructionLength { get; } = instructionLength;
}

public enum AddressingMode
{
    Absolute,
    Relative,
}

public static class Patterns
{
    public static readonly Pattern WorldChrMan = new Pattern(
        [0x48, 0x8B, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x40, 0x32],
        "xxx????xx",
        0,
        AddressingMode.Relative,
        3,
        7
    );


    #region Functions

    public static readonly Pattern AddSen = new Pattern(
        [0x7E, 0x69, 0x8B, 0x97],
        "xxxx",
        -0x40,
        AddressingMode.Absolute
    );

    #endregion


    #region Hooks

    public static readonly Pattern LockedTarget = new Pattern(
        [0x48, 0x8B, 0x80, 0xF8, 0x1F, 0x00, 0x00, 0x48, 0x8B, 0x08, 0x48, 0xB8],
        "xxxxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );

    public static readonly Pattern FreezeTargetPosture = new Pattern(
        [0x48, 0x89, 0x5C, 0x24, 0x30, 0x48, 0x8B, 0xCF, 0x8B, 0x9F],
        "xxxxxxxxxx",
        0,
        AddressingMode.Absolute
    );


    #endregion
}