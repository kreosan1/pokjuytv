using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpGetTable : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.GetTable && instruction.C <= 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Stk[Inst[OP_A]]=Stk[Inst[OP_B]][Stk[Inst[OP_C]]];";
	}

	public class OpGetTableConst : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.GetTable && instruction.C > 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Stk[Inst[OP_A]]=Stk[Inst[OP_B]][Inst[OP_C]];";

		public override void Mutate(Instruction instruction)
		{
			instruction.C -= 255;
			instruction.ConstantMask |= InstructionConstantMask.RC;
		}
	}
}