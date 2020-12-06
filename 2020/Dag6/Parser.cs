using System;
using System.Linq;
using System.Collections.Generic;

namespace Dag6
{
    public class Parser
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        internal static IEnumerable<AnswerForm> ReadData(string filePath)
        {
            var lines = readlines(filePath);
            var groupId = 0;
            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    groupId++;
                } else { 
                    yield return new AnswerForm { GroupId = groupId, PositiveAnswers = line };
                }
            }
            yield break;
        }
    }
}
