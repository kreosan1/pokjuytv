using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpReturn : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Return && instruction.B > 3;

		public override string GetObfuscated(ObfuscationContext context) =>
			@"
local A = Inst[OP_A];
local junk=math.floor(math.random()*0); -- Neutral junk
do return Unpack(Stk, A + junk, A + Inst[OP_B]) end;
";

		public override void Mutate(Instruction instruction)
		{
			instruction.B -= 2;
		}
	}

	public class OpReturnB2 : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Return && instruction.B == 2;

		public override string GetObfuscated(ObfuscationContext context) =>
			@"
local junk=math.floor(math.random()*0); -- Neutral junk
do return Stk[Inst[OP_A + junk]] end
";
	}

	public class OpReturnB3 : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Return && instruction.B == 3;

		public override string GetObfuscated(ObfuscationContext context) =>
			@"
local A = Inst[OP_A]; 
local junk=math.floor(math.random()*0); -- Neutral junk
do return Stk[A + junk], Stk[A + 1] end
";
	}

	public class OpReturnB0 : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Return && instruction.B == 0;

		public override string GetObfuscated(ObfuscationContext context) =>
			@"
local A = Inst[OP_A]; 
local junk=math.floor(math.random()*0); -- Neutral junk
do return Unpack(Stk, A + junk, Top) end;";
	}

	public class OpReturnB1 : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Return && instruction.B == 1;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0); -- Neutral junk\ndo return end;";
	}
}