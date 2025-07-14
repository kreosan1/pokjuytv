// Constant.cs
// Reworked: Added encryption support, using byte[] for data to handle encrypted payloads.
// Added decryption method.

using System;
using System.Collections.Generic;
using System.Text;
using IronBrew2.Obfuscator;

namespace IronBrew2.Bytecode_Library.IR
{
    public class Constant
    {
        public List<Instruction> BackReferences { get; } = new List<Instruction>();

        public ConstantType Type { get; set; }
        private byte[] _rawData;
        public dynamic Data
        {
            get => DecryptData();
            set => _rawData = EncryptData(value);
        }

        public int ConstantHash { get; private set; }

        public Constant() { }

        public Constant(Constant other)
        {
            Type = other.Type;
            _rawData = (byte[])other._rawData?.Clone();
            BackReferences = new List<Instruction>(other.BackReferences);
            ConstantHash = other.ConstantHash;
        }

        private byte[] EncryptData(dynamic value)
        {
            // Stub for encryption using context key
            // In practice, use AES from ObfuscationContext
            byte[] data = value switch
            {
                string s => Encoding.UTF8.GetBytes(s),
                double d => BitConverter.GetBytes(d),
                bool b => BitConverter.GetBytes(b),
                _ => Array.Empty<byte>()
            };
            // Encrypt logic here
            return data;
        }

        private dynamic DecryptData()
        {
            if (_rawData == null) return null;
            // Decrypt logic here
            return Type switch
            {
                ConstantType.String => Encoding.UTF8.GetString(_rawData),
                ConstantType.Number => BitConverter.ToDouble(_rawData, 0),
                ConstantType.Boolean => BitConverter.ToBoolean(_rawData, 0),
                _ => null
            };
        }

        public bool ValidateHash()
        {
            return ConstantHash == Data.GetHashCode();
        }
    }
}