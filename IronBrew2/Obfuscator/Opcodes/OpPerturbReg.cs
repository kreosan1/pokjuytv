using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
    // Custom Opcode: Temporarily perturbs a register value and restores it
    public class OpPerturbReg : VOpcode
    {
        public override bool IsInstruction(Instruction instruction) => false; // Inserted via mutation

        public override string GetObfuscated(ObfuscationContext context) =>
            "local temp = Stk[Inst[OP_A]]; Stk[Inst[OP_A]] = temp + math.floor(math.random() * 0); -- Perturb and restore";
    }
}