using System;
using System.Text.RegularExpressions;

namespace Dag4
{
    internal struct Passport
    {
        public int? BirthYear { get; set; }
        public int? IssueYear { get; set; }
        public int? ExpirationYear { get; set; }
        public string Height { get; set; }
        public string HairColor { get; set; }
        public string EyeColor { get; set; }
        public string PassportId { get; set; }
        public string CountryId { get; set; }

        public bool HeightValid()
        {
            if (Height.EndsWith("cm"))
            {
                var cms = Convert.ToInt32(Height.Substring(0, Height.Length - 2));
                return cms >= 150 && cms <= 193;
            }
            else if (Height.EndsWith("in"))
            {
                var ins = Convert.ToInt32(Height.Substring(0, Height.Length - 2));
                return ins >= 59 && ins <= 76;
            }
            return false;
        }

        private readonly static Regex regHairColor = new Regex("^#[0-9a-f]{6}$");
        public bool HairColorValid()
        {
            return regHairColor.IsMatch(HairColor);
        }

        private readonly static Regex regEyeColor = new Regex("^(amb)|(blu)|(brn)|(gry)|(grn)|(hzl)|(oth)$");
        public bool EyeColorValid()
        {
            return regEyeColor.IsMatch(EyeColor);
        }

        private readonly static Regex regPid = new Regex("^\\d{9}$");
        public bool PassportIdValid()
        {
            return regPid.IsMatch(PassportId);
        }


        public bool IsValid()
        {
            return BirthYear.HasValue &&
                IssueYear.HasValue &&
                ExpirationYear.HasValue &&
                !string.IsNullOrEmpty(Height) &&
                !string.IsNullOrEmpty(HairColor) &&
                !string.IsNullOrEmpty(EyeColor) &&
                !string.IsNullOrEmpty(PassportId);
        }
        public bool IsValid2()
        {
            return BirthYear.HasValue && BirthYear.Value >= 1920 && BirthYear.Value <= 2002 &&
                IssueYear.HasValue && IssueYear.Value >= 2010 && IssueYear.Value <= 2020 &&
                ExpirationYear.HasValue && ExpirationYear.Value >= 2020 && ExpirationYear.Value <= 2030 &&
                !string.IsNullOrEmpty(Height) && HeightValid() &&
                !string.IsNullOrEmpty(HairColor) && HairColorValid() &&
                !string.IsNullOrEmpty(EyeColor) && EyeColorValid() &&
                !string.IsNullOrEmpty(PassportId) && PassportIdValid();
        }
    }
}
