using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dag11
{
    public class SeatMapParser : CharMapParser<SeatStatus>
    {
        private static SeatMapParser instance = null;
        public static SeatMapParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SeatMapParser();
                }
                return instance;
            }
        }
        protected override SeatStatus Convert(char input)
        {
            return input.ToString().ParseEnumValue<SeatStatus>();
        }
    }
}
