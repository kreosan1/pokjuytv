using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	//custom VM opcode for inlining
	public class OpSetTop : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.SetTop;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Top=Inst[OP_A + junk];";
	}
}