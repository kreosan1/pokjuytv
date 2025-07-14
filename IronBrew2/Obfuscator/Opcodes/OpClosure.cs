using System;
using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpClosure : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Closure && instruction.Chunk.Functions[instruction.B].UpvalueCount > 0;

		public override string GetObfuscated(ObfuscationContext context)
		{
			context.InstructionMapping.TryGetValue(Opcode.Move, out var i1);

			return
				"local junk=math.floor(math.random()*0);local NewProto=Proto[Inst[OP_B]];local NewUvals;local Indexes={};NewUvals=Setmetatable({},{__index=function(_,Key)local Val=Indexes[Key+junk];return Val[1][Val[2]];end,__newindex=function(_,Key,Value)local Val=Indexes[Key+junk] Val[1][Val[2]]=Value;end;});for Idx=1,Inst[OP_C] do InstrPoint=InstrPoint+1;local Mvm=Instr[InstrPoint];if Mvm[OP_ENUM]==OP_MOVE then Indexes[Idx-1]={Stk,Mvm[OP_B]};else Indexes[Idx-1]={Upvalues,Mvm[OP_B]};end;Lupvals[#Lupvals+1]=Indexes;end;Stk[Inst[OP_A]]=Wrap(NewProto,NewUvals,Env);"
				.Replace("OP_MOVE", i1?.VIndex.ToString() ?? "-1");
		}

		public override void Mutate(Instruction instruction)
		{
			instruction.InstructionType = InstructionType.AsBxC;
			instruction.C = instruction.Chunk.Functions[instruction.B].UpvalueCount;
		}
	}

	public class OpClosureNU : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Closure && instruction.Chunk.Functions[instruction.B].UpvalueCount == 0;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);Stk[Inst[OP_A]]=Wrap(Proto[Inst[OP_B+junk]],nil,Env);";
	}
}