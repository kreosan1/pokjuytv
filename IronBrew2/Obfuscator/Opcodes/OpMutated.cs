using System;
using System.Collections.Generic;
using System.Linq;
using IronBrew2.Bytecode_Library.IR;

namespace IronBrew2.Obfuscator.Opcodes
{
	public class OpMutated : VOpcode
	{
		public static Random rand = new Random();

		public VOpcode Mutated;
		public int[] Registers;
		public int PerturbValue; // Added for enhanced mutation: used to insert junk perturbations in generated Lua code

		public static string[] RegisterReplacements = { "OP__A", "OP__B", "OP__C" };

		public override bool IsInstruction(Instruction instruction) =>
			false;

		public bool CheckInstruction() =>
			rand.Next(1, 15) == 1;

		public override string GetObfuscated(ObfuscationContext context)
		{
			// Modified to include perturbation: Insert junk code that doesn't change semantics but breaks pattern-based deobfuscators
			string original = Mutated.GetObfuscated(context);
			string junk = $"local perturb = {PerturbValue}; perturb = perturb * math.floor(math.random()*0);"; // Enhanced neutral junk operation
			return junk + original; // Prepend junk to the opcode's Lua snippet
		}

		public override void Mutate(Instruction instruction)
		{
			Mutated.Mutate(instruction);
			// Optionally perturb instruction values if applicable, but keep semantics intact
		}
	}
}