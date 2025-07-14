// VanillaSerializer.cs
// Fix: Use local vars instead of ref to properties.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Bytecode_Library.Bytecode
{
    public class VanillaSerializer
    {
        private Chunk _chunk;
        private Encoding _fuckingLua = Encoding.GetEncoding(28591);
        private byte _hashKey = 0xCD; // For simple hash

        public VanillaSerializer(Chunk chunk) =>
            _chunk = chunk;

        public byte[] Serialize()
        {
            List<byte> res = new List<byte>();

            #region Helper Methods
            void WriteByte(byte b) =>
                res.Add((byte)(b ^ _hashKey));

            void WriteBytes(byte[] bs) =>
                res.AddRange(bs.Select(x => (byte)(x ^ _hashKey)));

            void WriteInt(int i) =>
                WriteBytes(BitConverter.GetBytes(i));

            void WriteUInt(uint i) =>
                WriteBytes(BitConverter.GetBytes(i));

            void WriteNum(double d) =>
                WriteBytes(BitConverter.GetBytes(d));

            void WriteString(string str)
            {
                byte[] bytes = _fuckingLua.GetBytes(str);

                WriteInt(bytes.Length + 1);
                WriteBytes(bytes);
                WriteByte(0);
            }
            #endregion

            void WriteChunk(Chunk chunk)
            {
                if (chunk.Name != "")
                    WriteString(chunk.Name);
                else
                    WriteInt(0);

                WriteInt(chunk.Line);
                WriteInt(chunk.LastLine);
                WriteByte(chunk.UpvalueCount);
                WriteByte(chunk.ParameterCount);
                WriteByte(chunk.VarargFlag);
                WriteByte(chunk.StackSize);

                chunk.UpdateMappings();

                WriteInt(chunk.Instructions.Count);
                foreach (var i in chunk.Instructions)
                {
                    i.UpdateRegisters();

                    int a = i.A;
                    int b = i.B;
                    int c = i.C;

                    uint result = 0;

                    result |= (uint)i.OpCode;
                    result |= ((uint)a << 6);

                    switch (i.InstructionType)
                    {
                        case InstructionType.ABx:
                            result |= ((uint)b << (6 + 8));
                            break;

                        case InstructionType.AsBx:
                            b += 131071;
                            result |= ((uint)b << (6 + 8));
                            break;

                        case InstructionType.ABC:
                            result |= ((uint)c << (6 + 8));
                            result |= ((uint)b << (6 + 8 + 9));
                            break;
                    }

                    WriteUInt(result);
                }

                WriteInt(chunk.Constants.Count);
                foreach (var constant in chunk.Constants)
                {
                    switch (constant.Type)
                    {
                        case ConstantType.Nil:
                            WriteByte(0);
                            break;

                        case ConstantType.Boolean:
                            WriteByte(1);
                            WriteByte((byte)((bool)constant.Data ? 1 : 0));
                            break;

                        case ConstantType.Number:
                            WriteByte(3);
                            WriteNum(constant.Data);
                            break;

                        case ConstantType.String:
                            WriteByte(4);
                            WriteString(constant.Data);
                            break;
                    }
                }

                WriteInt(chunk.Functions.Count);
                foreach (var sChunk in chunk.Functions)
                    WriteChunk(sChunk);

                WriteInt(0);
                WriteInt(0);
                WriteInt(0);

                // Insert integrity hash
                WriteInt(chunk.GetHashCode() ^ _hashKey);
            }

            WriteByte(27);
            WriteBytes(_fuckingLua.GetBytes("Lua"));
            WriteByte(0x51);
            WriteByte(0);
            WriteByte(1);
            WriteByte(4);
            WriteByte(4);
            WriteByte(4);
            WriteByte(8);
            WriteByte(0);

            WriteChunk(_chunk);

            return res.ToArray();
        }
    }
}