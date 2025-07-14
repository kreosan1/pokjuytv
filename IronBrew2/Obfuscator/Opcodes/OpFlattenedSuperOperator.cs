using System;
using System.Collections.Generic;
using System.Linq;
using IronBrew2.Bytecode_Library.IR;
using IronBrew2.Obfuscator.VM_Generation;
using IronBrew2.Extensions;

namespace IronBrew2.Obfuscator.Opcodes
{
    public class OpFlattenedSuperOperator : VOpcode
    {
        public VOpcode[] SubOpcodes { get; set; }

        public override bool IsInstruction(Instruction instruction)
        {
            return false;
        }

        public bool IsInstruction(List<Instruction> instructions)
        {
            if (instructions.Count != SubOpcodes.Length)
                return false;

            for (int i = 0; i < instructions.Count; i++)
            {
                if (!SubOpcodes[i].IsInstruction(instructions[i]))
                    return false;
            }

            return true;
        }

        public override string GetObfuscated(ObfuscationContext context)
        {
            if (SubOpcodes == null || SubOpcodes.Length == 0)
                return ";";

            Random rng = new Random();

            // Генерируем уникальные случайные состояния (чтобы избежать коллизий, используем большой диапазон)
            HashSet<int> uniqueStates = new HashSet<int>();
            while (uniqueStates.Count < SubOpcodes.Length + 2)
            {
                uniqueStates.Add(rng.Next(10000, 1000000));
            }
            int[] states = uniqueStates.ToArray();

            // states[0] - начальное состояние
            // states[1] до states[SubOpcodes.Length] - состояния для каждого sub-opcode
            // states[SubOpcodes.Length + 1] - состояние выхода

            string code = "do local _state_ = " + states[0] + "; while true do ";

            // Подготавливаем блоки кода как пары (condition, body)
            List<(string cond, string body)> branches = new List<(string, string)>();

            // Начальный блок: установка первого состояния
            branches.Add((states[0].ToString(), "_state_ = " + states[1] + ";"));

            // Блоки для sub-opcodes
            for (int i = 0; i < SubOpcodes.Length; i++)
            {
                string subCode = SubOpcodes[i].GetObfuscated(context);
                int nextState = (i == SubOpcodes.Length - 1) ? states[SubOpcodes.Length + 1] : states[i + 2];
                branches.Add((states[i + 1].ToString(), subCode + " _state_ = " + nextState + ";"));
            }

            // Блок выхода
            branches.Add((states[SubOpcodes.Length + 1].ToString(), "break;"));

            // Перемешиваем порядок ветвей для дополнительной обфускации
            branches.Shuffle();

            // Собираем все ветви с правильными ключевыми словами if/elseif
            string branchesStr = "";
            for (int k = 0; k < branches.Count; k++)
            {
                var (cond, body) = branches[k];
                string keyword = (k == 0) ? "if" : "elseif";
                branchesStr += keyword + " _state_ == " + cond + " then " + body + " ";
            }

            // Завершаем конструкцию
            code += branchesStr + "end; end; end;";

            return code;
        }
    }
}