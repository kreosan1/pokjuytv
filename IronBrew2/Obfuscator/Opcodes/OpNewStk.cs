using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
    public class OpNewStk : VOpcode
    {
        public override bool IsInstruction(Instruction instruction) =>
            instruction.OpCode == Opcode.NewStack;

        public override string GetObfuscated(ObfuscationContext context) =>
            "local junk=math.floor(math.random()*0);Stk = {};for Idx = 0 + junk, PCount do if Idx < Params then Stk[Idx] = Args[Idx + 1]; else break end; end;";
    }
}