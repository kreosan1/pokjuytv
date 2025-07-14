using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpConcat : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Concat;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local B=Inst[OP_B];local junk=math.floor(math.random()*0);local K=Stk[B] for Idx=B+1+junk,Inst[OP_C] do K=K..Stk[Idx];end;Stk[Inst[OP_A]]=K;";
	}
}