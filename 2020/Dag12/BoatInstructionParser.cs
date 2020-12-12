using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dag12
{
    class BoatInstructionParser : Parser<Instruction>
    {
        protected override Instruction ParseLine(string line)
        {
            var operation = line[0].ToString().ParseEnumValue<OperationType>();
            var argument = line.Substring(1);
            return new Instruction { Operation = operation, Argument = Convert.ToInt32(argument) };
        }
    }
}
