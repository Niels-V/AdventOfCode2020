namespace Common
{
    public class IntMapParser : CharMapParser<int>
    {
        private static IntMapParser instance = null;
        public static IntMapParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IntMapParser();
                }
                return instance;
            }
        }
        protected override int Convert(char input) => System.Convert.ToInt32(input.ToString());
    }
}
