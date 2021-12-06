using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Dag19
{
    public class RuleResult
    {
        public bool IsValid { get; set; }
        public int NextIndex { get; set; }
    }
    public abstract class MessageRule
    {


        public MessageRule(MessageRules rulesCollection)
        {
            RulesCollection = rulesCollection;
        }
        public MessageRules RulesCollection { get; protected set; }
        public int Id { get; set; }
        public abstract RuleResult IsValid(string message, int index);

        public abstract Tuple<int,int> FindWordLength();
    }
    public class CharacterRule : MessageRule
    {
        public CharacterRule(MessageRules rulesCollection) : base(rulesCollection) { }

        public char Character { get; set; }

        public override Tuple<int, int> FindWordLength()
        {
            return new Tuple<int,int>(1,1);
        }

        public override RuleResult IsValid(string message, int index)
        {
            if (index >= message.Length) { return new RuleResult { IsValid = false, NextIndex = 0 }; }
            var isValid = message[index] == Character;
            var result = new RuleResult { IsValid = isValid, NextIndex = index + 1 };
            return result;
        }
    }

    public class FollowingRule : MessageRule
    {
        public FollowingRule(MessageRules rulesCollection) : base(rulesCollection)
        {
        }
        public int[] Ids { get; set; }

        public override Tuple<int, int> FindWordLength()
        {
            var wordLengths = Ids.Select(r=>RulesCollection[r].FindWordLength());
            var minValuesSum = wordLengths.Sum(w => w.Item1);
            var maxValuesSum = wordLengths.Sum(w => w.Item2);
            return new Tuple<int,int>(minValuesSum, maxValuesSum);
        }

        public override RuleResult IsValid(string message, int index)
        {
            if (index >= message.Length) { return new RuleResult { IsValid = false, NextIndex = 0 }; }
            var lastIndex = index;
            foreach (int ruleId in Ids)
            {
                var ruleResult = RulesCollection[ruleId].IsValid(message, lastIndex);
                if (!ruleResult.IsValid) { return ruleResult; }
                lastIndex = ruleResult.NextIndex;
            }
            return new RuleResult { IsValid = true, NextIndex = lastIndex };
        }
    }

    public class FollowingOrRule : MessageRule
    {
        public FollowingOrRule(MessageRules rulesCollection) : base(rulesCollection)
        {
        }
        public int[] LeftIds { get; set; }

        public int[] RightIds { get; set; }

        public override Tuple<int, int> FindWordLength()
        {
            var wordLengthsLeft = LeftIds.Select(r => RulesCollection[r].FindWordLength());
            var wordLengthsRight = LeftIds.Select(r => RulesCollection[r].FindWordLength());
            var minValuesSum = wordLengthsLeft.Sum(w => w.Item1);
            var minR = wordLengthsRight.Sum(w => w.Item1);
            var maxValuesSum = wordLengthsRight.Sum(w => w.Item2);
            var maxR = wordLengthsRight.Sum(w => w.Item2);
            return new Tuple<int, int>(minValuesSum<minR ? minValuesSum:minR, maxValuesSum >maxR ? maxValuesSum:maxR);
        }

        public override RuleResult IsValid(string message, int index)
        {
            if (index >= message.Length) { return new RuleResult { IsValid = false, NextIndex = 0 }; }

            RuleResult lastRuleResult = null;
            var lastIndex = index;
            foreach (int ruleId in LeftIds)
            {
                lastRuleResult = RulesCollection[ruleId].IsValid(message, lastIndex);
                if (!lastRuleResult.IsValid) { break; }
                lastIndex = lastRuleResult.NextIndex;
            }
            var leftResult = lastRuleResult;

            lastIndex = index;
            foreach (int ruleId in RightIds)
            {
                lastRuleResult = RulesCollection[ruleId].IsValid(message, lastIndex);
                if (!lastRuleResult.IsValid) { break; }
                lastIndex = lastRuleResult.NextIndex;
            }
            var rightResult = lastRuleResult;
            if (leftResult.IsValid && rightResult.IsValid)
            {
                string leftKey = $"{Id}-{index}-L";
                string rightKey = $"{Id}-{index}-R";
                //choose which one to take
                if (RulesCollection.DecissionPonts.Contains(leftKey))
                {
                    RulesCollection.DecissionPonts.Remove(leftKey);
                    RulesCollection.DecissionPonts.Add(rightKey);
                    return rightResult;
                }
                if (RulesCollection.DecissionPonts.Contains(rightKey))
                {
                    RulesCollection.DecissionPonts.Remove(rightKey);
                }
                RulesCollection.DecissionPonts.Add(leftKey);
            }
            if (leftResult.IsValid) { return leftResult; }
            return rightResult;
        }
    }
    [TestCategory("2020")]
    [TestClass]
    public class MessageRules
    {
        public MessageRule this[int ruleId] => Rules[ruleId];
        public Dictionary<int, MessageRule> Rules { get; private set; }


        public HashSet<string> DecissionPonts { get; private set; }

        public MessageRules()
        {
            Rules = new Dictionary<int, MessageRule>();
            DecissionPonts = new HashSet<string>();
        }

        public void AddConstantRule(int id, char character)
        {
            Rules.Add(id, new CharacterRule(this) { Id = id, Character = character });
        }

        public void AddFollowingRule(int id, int[] ruleId)
        {
            Rules.Add(id, new FollowingRule(this) { Id = id, Ids = ruleId });
        }
        public void AddFollowingOrRule(int id, int[] leftPart, int[] rightPart)
        {
            Rules.Add(id, new FollowingOrRule(this) { Id = id, LeftIds = leftPart, RightIds = rightPart });
        }

        public void ChangeFollowingOrRule(int id, int[] leftPart, int[] rightPart)
        {
            Rules[id] = new FollowingOrRule(this) { Id = id, LeftIds = leftPart, RightIds = rightPart };
        }

        public bool IsValid(string message)
        {
            var rulesValid = Rules[0].IsValid(message, 0);
            var allCharactersVisited = rulesValid.NextIndex == message.Length;
            var isValid = rulesValid.IsValid && allCharactersVisited;
             if(!isValid && DecissionPonts.Count > 0)
            {   
                rulesValid = Rules[0].IsValid(message, 0);
                allCharactersVisited = rulesValid.NextIndex == message.Length;
                isValid = rulesValid.IsValid && allCharactersVisited;
            }
            return isValid;
        }

        [DataTestMethod]
        [DataRow("ababbb", true)]
        [DataRow("bababa", false)]
        [DataRow("abbbab", true)]
        [DataRow("aaabbb", false)]
        [DataRow("aaaabb", true)]
        [DataRow("aaaabbb", false)]
        public void Test_Rules(string message, bool expectedMatch)
        {
            var rules = new MessageRules();
            rules.AddFollowingRule(0, new[] { 4, 1, 5 });
            rules.AddFollowingOrRule(1, new[] { 2, 3 }, new[] { 3, 2 });
            rules.AddFollowingOrRule(2, new[] { 4, 4 }, new[] { 5, 5 });
            rules.AddFollowingOrRule(3, new[] { 4, 5 }, new[] { 5, 4 });
            rules.AddConstantRule(4, 'a');
            rules.AddConstantRule(5, 'b');

            var result = rules.IsValid(message);
            Assert.AreEqual(expectedMatch, result);
        }

    }


}
