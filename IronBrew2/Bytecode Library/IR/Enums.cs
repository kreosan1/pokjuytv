// Enums.cs
// Reworked: Added Encrypted variants for ConstantType and InstructionType.

using System;

namespace IronBrew2.Bytecode_Library.IR
{
    public enum ConstantType
    {
        Nil,
        Boolean,
        Number,
        String,
        EncryptedString,  // New: Encrypted string
        EncryptedNumber   // New: Encrypted number
    }

    public enum InstructionType
    {
        ABC,
        ABx,
        AsBx,
        AsBxC,
        Data,
        SecureABC,  // New: Secure variant with hash
        SecureJump  // New: Encrypted jump
    }

    [Flags]
    public enum InstructionConstantMask
    {
        NK = 0,
        RA = 1,
        RB = 2,
        RC = 4,
        Encrypted = 8,  // New: Encrypted operand
        Verified = 16   // New: Verified integrity
    }
}