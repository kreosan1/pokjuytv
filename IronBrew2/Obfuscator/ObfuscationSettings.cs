// ObfuscationSettings.cs
// Reworked: Added security-related settings for crypto, integrity levels.

namespace IronBrew2.Obfuscator
{
    public class ObfuscationSettings
    {
        public bool EncryptStrings { get; set; } = true;
        public bool EncryptImportantStrings { get; set; } = true;
        public bool ControlFlow { get; set; } = true;
        public bool BytecodeCompress { get; set; } = true;
        public int DecryptTableLen { get; set; } = 500;
        public bool PreserveLineInfo { get; set; } = false;
        public bool Mutate { get; set; } = true;
        public bool SuperOperators { get; set; } = true;
        public int MaxMiniSuperOperators { get; set; } = 120;
        public int MaxMegaSuperOperators { get; set; } = 120;
        public int MaxMutations { get; set; } = 200;

        // New security settings
        public bool EnableAES { get; set; } = true;
        public bool EnableHMACIntegrity { get; set; } = true;
        public int IntegrityLevel { get; set; } = 2; // 1: Basic, 2: Full, 3: Paranoid
        public bool AntiDump { get; set; } = true;
    }
}