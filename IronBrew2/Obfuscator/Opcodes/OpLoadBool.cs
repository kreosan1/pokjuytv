using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpLoadBool : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.LoadBool && instruction.C == 0;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Stk[Inst[OP_A]]=(Inst[OP_B + junk]~=0);";
	}

	public class OpLoadBoolC : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.LoadBool && instruction.C != 0;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Stk[Inst[OP_A]]=(Inst[OP_B + junk]~=0);InstrPoint=InstrPoint+1;";

		public override void Mutate(Instruction ins) =>
			ins.C = 0;
	}
}