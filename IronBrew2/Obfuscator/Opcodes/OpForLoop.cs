using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpForLoop : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.ForLoop;

		public override string GetObfuscated(ObfuscationContext context) =>
			@"
local A = Inst[OP_A];
local junk = math.floor(math.random()*0); -- Neutral junk
local Step = Stk[A + 2];
local Index = Stk[A] + Step + junk;
Stk[A] = Index;
if (Step > 0 + junk) then 
	if (Index <= Stk[A+1]) then
		InstrPoint = Inst[OP_B];
		Stk[A+3] = Index;
	end
elseif (Index >= Stk[A+1]) then
	InstrPoint = Inst[OP_B];
	Stk[A+3] = Index;
end
";
		public override void Mutate(Instruction instruction)
		{
			instruction.B += instruction.PC + 1;
		}
	}
}