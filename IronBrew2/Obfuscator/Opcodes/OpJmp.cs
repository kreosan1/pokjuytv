using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpJmp : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Jmp;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);InstrPoint=Inst[OP_B + junk];";

		public override void Mutate(Instruction instruction)
		{
			instruction.B += instruction.PC + 1;
		}
	}
}