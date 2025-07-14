// Chunk.cs
// Reworked: Added methods for computing and verifying chunk integrity.
// Made mappings lazy-loaded for performance.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using IronBrew2.Bytecode_Library.Bytecode;

namespace IronBrew2.Bytecode_Library.IR
{
    public class Chunk
    {
        public string Name { get; set; }
        public int Line { get; set; }
        public int LastLine { get; set; }
        public byte UpvalueCount { get; set; }
        public byte ParameterCount { get; set; }
        public byte VarargFlag { get; set; }
        public byte StackSize { get; set; }
        public int CurrentOffset { get; private set; } = 0;
        public int CurrentParamOffset { get; private set; } = 0;
        public List<Instruction> Instructions { get; set; } = new List<Instruction>();
        private Dictionary<Instruction, int> _instructionMap;
        public Dictionary<Instruction, int> InstructionMap => _instructionMap ??= GenerateInstructionMap();
        public List<Constant> Constants { get; set; } = new List<Constant>();
        private Dictionary<Constant, int> _constantMap;
        public Dictionary<Constant, int> ConstantMap => _constantMap ??= GenerateConstantMap();
        public List<Chunk> Functions { get; set; } = new List<Chunk>();
        private Dictionary<Chunk, int> _functionMap;
        public Dictionary<Chunk, int> FunctionMap => _functionMap ??= GenerateFunctionMap();
        public List<string> Upvalues { get; set; } = new List<string>();

        private Dictionary<Instruction, int> GenerateInstructionMap()
        {
            var map = new Dictionary<Instruction, int>();
            for (int i = 0; i < Instructions.Count; i++)
                map[Instructions[i]] = i;
            return map;
        }

        private Dictionary<Constant, int> GenerateConstantMap()
        {
            var map = new Dictionary<Constant, int>();
            for (int i = 0; i < Constants.Count; i++)
                map[Constants[i]] = i;
            return map;
        }

        private Dictionary<Chunk, int> GenerateFunctionMap()
        {
            var map = new Dictionary<Chunk, int>();
            for (int i = 0; i < Functions.Count; i++)
                map[Functions[i]] = i;
            return map;
        }

        public void UpdateMappings()
        {
            _instructionMap = GenerateInstructionMap();
            _constantMap = GenerateConstantMap();
            _functionMap = GenerateFunctionMap();
        }

        public override int GetHashCode()
        {
            using var sha256 = SHA256.Create();
            var hashData = Instructions.Select(i => i.SecurityHash).Aggregate(0, (acc, h) => acc ^ h);
            return BitConverter.ToInt32(sha256.ComputeHash(BitConverter.GetBytes(hashData)), 0);
        }

        public bool VerifyIntegrity()
        {
            return Instructions.All(i => i.VerifyIntegrity()) && Constants.All(c => c.ValidateHash());
        }

        public int Rebase(int offset, int paramOffset = 0)
        {
            offset -= CurrentOffset;
            paramOffset -= CurrentParamOffset;

            CurrentOffset += offset;
            CurrentParamOffset += paramOffset;

            StackSize = (byte)(StackSize + offset);

            var paramsCount = ParameterCount - 1;
            for (var i = 0; i < Instructions.Count; i++)
            {
                var instr = Instructions[i];

                switch (instr.OpCode)
                {
                    case Opcode.Move:
                    case Opcode.LoadNil:
                    case Opcode.Unm:
                    case Opcode.Not:
                    case Opcode.Len:
                    case Opcode.TestSet:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        instr.B += instr.B > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.LoadConst:
                    case Opcode.LoadBool:
                    case Opcode.GetGlobal:
                    case Opcode.SetGlobal:
                    case Opcode.GetUpval:
                    case Opcode.SetUpval:
                    case Opcode.Call:
                    case Opcode.TailCall:
                    case Opcode.Return:
                    case Opcode.VarArg:
                    case Opcode.Test:
                    case Opcode.ForPrep:
                    case Opcode.ForLoop:
                    case Opcode.TForLoop:
                    case Opcode.NewTable:
                    case Opcode.SetList:
                    case Opcode.Close:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.GetTable:
                    case Opcode.SetTable:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        if (instr.B < 255)
                            instr.B += instr.B > paramsCount ? offset : paramOffset;
                        instr.C += instr.C > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.Add:
                    case Opcode.Sub:
                    case Opcode.Mul:
                    case Opcode.Div:
                    case Opcode.Mod:
                    case Opcode.Pow:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        if (instr.B < 255)
                            instr.B += instr.B > paramsCount ? offset : paramOffset;
                        if (instr.C < 255)
                            instr.C += instr.C > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.Concat:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        instr.B += instr.B > paramsCount ? offset : paramOffset;
                        instr.C += instr.C > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.Self:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        instr.B += instr.B > paramsCount ? offset : paramOffset;
                        if (instr.C < 255)
                            instr.C += instr.C > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.Eq:
                    case Opcode.Lt:
                    case Opcode.Le:
                        if (instr.B < 255)
                            instr.B += instr.B > paramsCount ? offset : paramOffset;
                        if (instr.C < 255)
                            instr.C += instr.C > paramsCount ? offset : paramOffset;
                        break;
                    case Opcode.Closure:
                        instr.A += instr.A > paramsCount ? offset : paramOffset;
                        if (instr.B >= Functions.Count) continue;
                        var nProto = Functions[instr.B];
                        for (var i2 = 0; i2 < nProto.UpvalueCount; i2++)
                        {
                            if (i + i2 + 1 >= Instructions.Count) break;
                            var cInst = Instructions[i + i2 + 1];
                            if (cInst.OpCode != Opcode.Move) continue;
                            cInst.B += cInst.B > paramsCount ? offset : paramOffset;
                        }
                        i += nProto.UpvalueCount;
                        break;
                }
            }

            return ParameterCount;
        }
    }
}