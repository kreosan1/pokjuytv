using IronBrew2.Bytecode_Library.Bytecode;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpLt : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Lt && instruction.A == 0 && instruction.B <= 255 && instruction.C <= 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);if(Stk[Inst[OP_A]] < Stk[Inst[OP_C]])then InstrPoint=InstrPoint+1 + junk;else InstrPoint=Inst[OP_B];end;";

		public override void Mutate(Instruction instruction)
		{
			instruction.A = instruction.B;

			instruction.B = instruction.PC + instruction.Chunk.Instructions[instruction.PC + 1].B + 2;
			instruction.InstructionType = InstructionType.AsBxC;
		}
	}

	public class OpLtB : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Lt && instruction.A == 0 && instruction.B > 255 && instruction.C <= 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);if(Inst[OP_A] < Stk[Inst[OP_C]])then InstrPoint=InstrPoint+1 + junk;else InstrPoint=Inst[OP_B];end;";

		public override void Mutate(Instruction instruction)
		{
			instruction.A = instruction.B - 255;

			instruction.B = instruction.PC + instruction.Chunk.Instructions[instruction.PC + 1].B + 2;
			instruction.InstructionType = InstructionType.AsBxC;
			instruction.ConstantMask |= InstructionConstantMask.RA;
		}
	}

	public class OpLtC : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Lt && instruction.A == 0 && instruction.B <= 255 && instruction.C > 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);if(Stk[Inst[OP_A]] < Inst[OP_C])then InstrPoint=InstrPoint+1 + junk;else InstrPoint=Inst[OP_B];end;";

		public override void Mutate(Instruction instruction)
		{
			instruction.A = instruction.B;
			instruction.C -= 255;

			instruction.B = instruction.PC + instruction.Chunk.Instructions[instruction.PC + 1].B + 2;
			instruction.InstructionType = InstructionType.AsBxC;
			instruction.ConstantMask |= InstructionConstantMask.RC;
		}
	}

	public class OpLtBC : VOpcode
	{
		public override bool IsInstruction(Instruction instruction) =>
			instruction.OpCode == Opcode.Lt && instruction.A == 0 && instruction.B > 255 && instruction.C > 255;

		public override string GetObfuscated(ObfuscationContext context) =>
			"local junk=math.floor(math.random()*0);if(Inst[OP_A] < Inst[OP_C])then InstrPoint=InstrPoint+1 + junk;else InstrPoint=Inst[OP_B];end;";

		public override void Mutate(Instruction instruction)
		{
			instruction.A = instruction.B - 255;
			instruction.C -= 255;

			instruction.B = instruction.PC + instruction.Chunk.Instructions[instruction.PC + 1].B + 2;
			instruction.InstructionType = InstructionType.AsBxC;
			instruction.ConstantMask |= InstructionConstantMask.RA | InstructionConstantMask.RC;
		}
	}
}