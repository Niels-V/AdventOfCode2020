using System;
using System.Linq;
using System.Collections.Generic;

namespace Dag4
{
    public class PassportParser
    {
        static IEnumerable<string> readlines(string filePath) => System.IO.File.ReadLines(filePath);
        static IEnumerable<string> ReadRawPassportdata(string filePath)
        {
            var lines = readlines(filePath);
            var passportLine = string.Empty;
            foreach (var line in lines)
            {
                if (line.Length == 0)
                {
                    if (passportLine == string.Empty) { yield break; }
                    var returnPassport = passportLine;
                    passportLine = string.Empty;
                    yield return returnPassport;
                }
                passportLine += line + " ";
            }
            yield return passportLine;
        }
        internal static IEnumerable<Passport> ReadPassports(string filePath)
        {
            var rawPassports = ReadRawPassportdata(filePath).ToList();
            var passportList = new List<Passport>();
            foreach (var passportString in rawPassports)
            {
                var passport = new Passport();
                var properties = passportString.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var property in properties)
                {
                    var nameValuePair = property.Split(":");
                    switch (nameValuePair[0])
                    {
                        case "byr":
                            passport.BirthYear = Convert.ToInt32(nameValuePair[1]);
                            break;
                        case "iyr":
                            passport.IssueYear = Convert.ToInt32(nameValuePair[1]);
                            break;
                        case "eyr":
                            passport.ExpirationYear = Convert.ToInt32(nameValuePair[1]);
                            break;
                        case "hgt":
                            passport.Height = nameValuePair[1];
                            break;
                        case "hcl":
                            passport.HairColor = nameValuePair[1];
                            break;
                        case "ecl":
                            passport.EyeColor = nameValuePair[1];
                            break;
                        case "pid":
                            passport.PassportId = nameValuePair[1];
                            break;
                        case "cid":
                            passport.CountryId = nameValuePair[1];
                            break;
                        default:
                            break;
                    }
                }
                passportList.Add(passport);
            }
            return passportList;
        }
    }
}
