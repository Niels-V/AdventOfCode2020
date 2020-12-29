using System.Collections.Generic;

namespace Dag16
{
    public class TicketInfo
    {
        public List<TicketProperty> Properties { get; set; }
        public List<Ticket> NearbyTickets { get; set; }

        public Ticket MyTicket { get; set; }
        public TicketInfo()
        {
            Properties = new List<TicketProperty>();
            NearbyTickets = new List<Ticket>();
        }
    }

    public class Ticket
    {
        public List<int> Values { get; set; }
        public Ticket()
        {
            Values = new List<int>();
        }
    }
}