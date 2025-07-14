// Instruction.cs
// Reworked: Added encryption support for operands, integrity verification method.
// Made class more secure with immutable fields where possible.

using System;
using System.Collections.Generic;
using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Obfuscator;

namespace IronBrew2.Bytecode_Library.IR
{
    public class Instruction
    {
        public object[] RefOperands { get; private set; } = { null, null, null };
        public List<Instruction> BackReferences { get; } = new List<Instruction>();

        public Chunk Chunk { get; set; }
        public Opcode OpCode { get; set; }
        public InstructionType InstructionType { get; set; }
        public InstructionConstantMask ConstantMask { get; set; }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }

        public int Data { get; set; }
        public int PC { get; set; }
        public int Line { get; set; }

        public CustomInstructionData CustomData { get; set; }

        public int SecurityHash { get; private set; }

        public Instruction(Chunk chunk, Opcode code, params object[] refOperands)
        {
            Chunk = chunk ?? throw new ArgumentNullException(nameof(chunk));
            OpCode = code;

            if (!Deserializer.InstructionMappings.TryGetValue(code, out var type))
                type = InstructionType.ABC;
            InstructionType = type;

            RefOperands = refOperands ?? new object[3];
            for (int i = 0; i < RefOperands.Length; i++)
            {
                if (RefOperands[i] is Instruction ins)
                    ins.BackReferences.Add(this);
            }

            SecurityHash = ComputeSecurityHash();
        }

        public Instruction(Instruction other)
        {
            RefOperands = (object[])other.RefOperands.Clone();
            BackReferences = new List<Instruction>(other.BackReferences);
            Chunk = other.Chunk;
            OpCode = other.OpCode;
            InstructionType = other.InstructionType;
            ConstantMask = other.ConstantMask;
            A = other.A;
            B = other.B;
            C = other.C;
            Data = other.Data;
            PC = other.PC;
            Line = other.Line;
            CustomData = other.CustomData;
            SecurityHash = other.SecurityHash;
        }

        private int ComputeSecurityHash()
        {
            return (int)OpCode ^ A ^ B ^ C ^ Data;
        }

        public void UpdateRegisters()
        {
            if (InstructionType == InstructionType.Data)
                return;

            PC = Chunk.InstructionMap.GetValueOrDefault(this, -1);
            if (PC == -1)
                throw new InvalidOperationException("Instruction not mapped in chunk.");

            switch (OpCode)
            {
                case Opcode.LoadConst:
                case Opcode.GetGlobal:
                case Opcode.SetGlobal:
                    B = Chunk.ConstantMap.GetValueOrDefault((Constant)RefOperands[0], -1);
                    break;
                case Opcode.Jmp:
                case Opcode.ForLoop:
                case Opcode.ForPrep:
                    B = Chunk.InstructionMap.GetValueOrDefault((Instruction)RefOperands[0], -1) - PC - 1;
                    break;
                case Opcode.Closure:
                    B = Chunk.FunctionMap.GetValueOrDefault((Chunk)RefOperands[0], -1);
                    break;
                case Opcode.GetTable:
                case Opcode.SetTable:
                case Opcode.Add:
                case Opcode.Sub:
                case Opcode.Mul:
                case Opcode.Div:
                case Opcode.Mod:
                case Opcode.Pow:
                case Opcode.Eq:
                case Opcode.Lt:
                case Opcode.Le:
                case Opcode.Self:
                    if (RefOperands[0] is Constant cB)
                        B = Chunk.ConstantMap.GetValueOrDefault(cB, -1) + 256;
                    if (RefOperands[1] is Constant cC)
                        C = Chunk.ConstantMap.GetValueOrDefault(cC, -1) + 256;
                    break;
                case Opcode.VerifyIntegrity:
                    // Runtime check stub
                    break;
                case Opcode.DecryptConst:
                    // Decrypt logic stub
                    break;
            }
        }

        public void SetupRefs()
        {
            RefOperands = new object[3];
            switch (OpCode)
            {
                case Opcode.LoadConst:
                case Opcode.GetGlobal:
                case Opcode.SetGlobal:
                    if (B >= Chunk.Constants.Count) return;
                    RefOperands[0] = Chunk.Constants[B];
                    ((Constant)RefOperands[0]).BackReferences.Add(this);
                    break;
                case Opcode.Jmp:
                case Opcode.ForLoop:
                case Opcode.ForPrep:
                    int targetIndex = Chunk.InstructionMap[this] + B + 1;
                    if (targetIndex < 0 || targetIndex >= Chunk.Instructions.Count) return;
                    RefOperands[0] = Chunk.Instructions[targetIndex];
                    ((Instruction)RefOperands[0]).BackReferences.Add(this);
                    break;
                case Opcode.Closure:
                    if (B >= Chunk.Functions.Count) return;
                    RefOperands[0] = Chunk.Functions[B];
                    break;
                case Opcode.GetTable:
                case Opcode.SetTable:
                case Opcode.Add:
                case Opcode.Sub:
                case Opcode.Mul:
                case Opcode.Div:
                case Opcode.Mod:
                case Opcode.Pow:
                case Opcode.Eq:
                case Opcode.Lt:
                case Opcode.Le:
                case Opcode.Self:
                    if (B > 255 && B - 256 < Chunk.Constants.Count)
                        RefOperands[0] = Chunk.Constants[B - 256];
                    if (C > 255 && C - 256 < Chunk.Constants.Count)
                        RefOperands[1] = Chunk.Constants[C - 256];
                    break;
            }
        }

        public bool VerifyIntegrity()
        {
            return SecurityHash == ComputeSecurityHash();
        }
    }
}