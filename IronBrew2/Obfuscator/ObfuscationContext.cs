// ObfuscationContext.cs
// Reworked: Added crypto keys, IV, HMAC support. Removed shuffling if not needed for security.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator
{
    public enum ChunkStep
    {
        ParameterCount,
        StringTable,
        Instructions,
        Functions,
        LineInfo,
        StepCount
    }

    public enum InstructionStep1
    {
        Type,
        A,
        B,
        C,
        StepCount
    }

    public enum InstructionStep2
    {
        Op,
        Bx,
        D,
        StepCount
    }

    public class ObfuscationContext
    {
        public Chunk HeadChunk { get; }
        public ChunkStep[] ChunkSteps { get; }
        public InstructionStep1[] InstructionSteps1 { get; }
        public InstructionStep2[] InstructionSteps2 { get; }
        public int[] ConstantMapping { get; }

        public Dictionary<Opcode, VOpcode> InstructionMapping { get; } = new Dictionary<Opcode, VOpcode>();

        public int PrimaryXorKey { get; }
        public int IXorKey1 { get; }
        public int IXorKey2 { get; }

        // New crypto fields
        public byte[] AesKey { get; }
        public byte[] AesIV { get; }
        public byte[] HmacKey { get; }

        public ObfuscationContext(Chunk chunk, ObfuscationSettings settings = null)
        {
            settings ??= new ObfuscationSettings();

            HeadChunk = chunk;
            ChunkSteps = Enum.GetValues(typeof(ChunkStep)).Cast<ChunkStep>().ToArray();
            InstructionSteps1 = Enum.GetValues(typeof(InstructionStep1)).Cast<InstructionStep1>().ToArray();
            InstructionSteps2 = Enum.GetValues(typeof(InstructionStep2)).Cast<InstructionStep2>().ToArray();
            ConstantMapping = Enumerable.Range(0, Enum.GetValues(typeof(ConstantType)).Length).ToArray();

            var rand = new Random();
            PrimaryXorKey = rand.Next(0, 256);
            IXorKey1 = rand.Next(0, 256);
            IXorKey2 = rand.Next(0, 256);

            if (settings.EnableAES)
            {
                AesKey = new byte[32]; // AES-256
                AesIV = new byte[16];
                rand.NextBytes(AesKey);
                rand.NextBytes(AesIV);
            }

            if (settings.EnableHMACIntegrity)
            {
                HmacKey = new byte[32];
                rand.NextBytes(HmacKey);
            }
        }
    }
}