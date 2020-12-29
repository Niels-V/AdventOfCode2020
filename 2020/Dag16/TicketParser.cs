using Common;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dag16
{
    public class TicketParser : LineParser
    {
        static readonly Regex propertyRule = new Regex("^(?<name>.+?):\\s(?<min1>\\d+)-(?<max1>\\d+) or (?<min2>\\d+)-(?<max2>\\d+)$", RegexOptions.Compiled);

        public TicketInfo ParseFile(string filePath)
        {
            var info = new TicketInfo();
            var mode = 0;
            foreach (var line in ReadData(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    mode++;
                    continue;
                }
                if (mode == 0)
                {
                    var regResult = propertyRule.Match(line);
                    var property = new TicketProperty(regResult.Groups.Values.First(g => g.Name == "name").Captures[0].Value
                        , System.Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "min1").Captures[0].Value)
                        , System.Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "max1").Captures[0].Value)
                        , System.Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "min2").Captures[0].Value)
                        , System.Convert.ToInt32(regResult.Groups.Values.First(g => g.Name == "max2").Captures[0].Value)
                    );
                    info.Properties.Add(property);
                }
                else if (mode == 1)
                {
                    if (line == "your ticket:") continue;
                    var ticket = new Ticket();
                    info.MyTicket = ticket;
                    info.MyTicket.Values.AddRange(line.Split(",").Select(s => System.Convert.ToInt32(s)));
                }
                else
                {
                    if (line == "nearby tickets:") continue;
                    var ticket = new Ticket();
                    ticket.Values.AddRange(line.Split(",").Select(s => System.Convert.ToInt32(s)));
                    info.NearbyTickets.Add(ticket);
                }
            }
            return info;
        }
    }
}
