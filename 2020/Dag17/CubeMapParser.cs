using Common;

namespace Dag17
{
    public class CubeMapParser : CharMapParser<CubeStatus>
    {
        private static CubeMapParser instance = null;
        public static CubeMapParser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CubeMapParser();
                }
                return instance;
            }
        }
        protected override CubeStatus Convert(char input)
        {
            return input.ToString().ParseEnumValue<CubeStatus>();
        }
    }
}
