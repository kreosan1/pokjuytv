// Opcode.cs
// Completely reworked: Added security-related opcodes for runtime verification.
// Integrated anti-tampering ops that can be executed in VM.

namespace IronBrew2.Bytecode_Library.Bytecode
{
    public enum Opcode
    {
        Move,
        LoadConst,
        LoadBool,
        LoadNil,
        GetUpval,
        GetGlobal,
        GetTable,
        SetGlobal,
        SetUpval,
        SetTable,
        NewTable,
        Self,
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        Pow,
        Unm,
        Not,
        Len,
        Concat,
        Jmp,
        Eq,
        Lt,
        Le,
        Test,
        TestSet,
        Call,
        TailCall,
        Return,
        ForLoop,
        ForPrep,
        TForLoop,
        SetList,
        Close,
        Closure,
        VarArg,

        // Original custom
        SetTop,
        PushStack,
        NewStack,
        SetFenv,

        // New security opcodes
        VerifyIntegrity,  // Checks HMAC of chunk
        DecryptConst,     // Decrypts a constant at runtime
        AntiDumpTrap      // Trap for dumpers (e.g., infinite loop if tampered)
    }
}