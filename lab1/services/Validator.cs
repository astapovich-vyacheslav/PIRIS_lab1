using Google.Protobuf;
using lab1.DB;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1.services
{
    public static class Validator
    {
        public enum ErrorCode {
            OK,
            WRONG_NAME,
            WRONG_SURNAME,
            WRONG_LASTNAME,
            WRONG_DATE_OF_BIRTH,
            WRONG_GENDER,
            WRONG_PASSPORT_SERIES,
            WRONG_PASSPORT_NUMBER,
            WRONG_PASSPORT_ISSUED_BY,
            WRONG_PASSPORT_DATE_OF_ISSUE,
            WRONG_PASSPORT_ID,
            WRONG_BIRTH_PLACE,
            WRONG_ADDRESS,
            WRONG_PHONE_NUMBER,
            WRONG_STATIONARY_PHONE_NUMBER,
            WRONG_EMAIL,
            WRONG_PLACE_OF_WORK,
            WRONG_JOB_TITLE,
            WRONG_IS_RETIRED,
            WRONG_MONTHLY_INCOME,
            WRONG_CONSCRIPT,
            NOT_FOUND_CITY_OF_RESIDENCE,
            NOT_FOUND_FAMILY_STATUS,
            NOT_FOUND_CITIZENSHIP,
            NOT_FOUND_DISABILITY,
            ALREADY_EXISTS_PASSPORT_SERIES_AND_NUMBER,
            UNKNOWN_ERROR
        }   




        public static ErrorCode ValidateRow(DataGridViewRow row)
        {
            try
            {
                int? id = Convert.ToInt32(row.Cells["id"].Value);

                if (row.Cells["name"].Value == null)
                {
                    return ErrorCode.WRONG_NAME;
                }
                string name = row.Cells["name"].Value.ToString();
                if (ContainsNonLetters(name))
                {
                    return ErrorCode.WRONG_NAME;
                }

                if (row.Cells["surname"].Value == null)
                {
                    return ErrorCode.WRONG_SURNAME;
                }
                string surname = row.Cells["surname"].Value.ToString();
                if (ContainsNonLetters(surname))
                {
                    return ErrorCode.WRONG_SURNAME;
                }

                if (row.Cells["lastname"].Value == null)
                {
                    return ErrorCode.WRONG_LASTNAME;
                }
                string lastname = row.Cells["lastname"].Value.ToString();
                if (ContainsNonLetters(lastname))
                {
                    return ErrorCode.WRONG_LASTNAME;
                }

                if (row.Cells["dateOfBirth"].Value == null)
                {
                    return ErrorCode.WRONG_DATE_OF_BIRTH;
                }
                string dateString = row.Cells["dateOfBirth"].Value.ToString();
                DateTime date;
                if (!DateTime.TryParse(dateString, out date))
                {
                    return ErrorCode.WRONG_DATE_OF_BIRTH;
                }

                if (row.Cells["gender"].Value == null)
                {
                    return ErrorCode.WRONG_GENDER;
                }
                string gender = row.Cells["gender"].Value.ToString();
                if (gender != "m" && gender != "f")
                {
                    return ErrorCode.WRONG_GENDER;
                }

                if (row.Cells["passportSeries"].Value == null)
                {
                    return ErrorCode.WRONG_PASSPORT_SERIES;
                }
                string passportSeries = row.Cells["passportSeries"].Value.ToString();
                if (!ContainsOnlyTwoLetters(passportSeries))
                {
                    return ErrorCode.WRONG_PASSPORT_SERIES;
                }

                if (row.Cells["passportNumber"].Value == null)
                {
                    return ErrorCode.WRONG_PASSPORT_NUMBER;
                }
                string passportNumber = row.Cells["passportNumber"].Value.ToString();
                if (!ContainsSixDigits(passportNumber))
                {
                    return ErrorCode.WRONG_PASSPORT_NUMBER;
                }

                if (Queries.IsPassportExists(passportSeries, passportNumber, id))
                {
                    return ErrorCode.ALREADY_EXISTS_PASSPORT_SERIES_AND_NUMBER;
                }

                if (row.Cells["passportIssuedBy"].Value == null)
                {
                    return ErrorCode.WRONG_PASSPORT_ISSUED_BY;
                }
                string passportIssuedBy = row.Cells["passportIssuedBy"].Value.ToString();
                if (string.IsNullOrEmpty(passportIssuedBy))
                {
                    return ErrorCode.WRONG_PASSPORT_ISSUED_BY;
                }

                if (row.Cells["passportDateOfIssue"].Value == null)
                {
                    return ErrorCode.WRONG_PASSPORT_DATE_OF_ISSUE;
                }
                string passportDateOfIssue = row.Cells["passportDateOfIssue"].Value.ToString();
                if (!DateTime.TryParse(passportDateOfIssue, out date))
                {
                    return ErrorCode.WRONG_PASSPORT_DATE_OF_ISSUE;
                }

                if (row.Cells["passportId"].Value == null)
                {
                    return ErrorCode.WRONG_PASSPORT_ID;
                }
                string passportId = row.Cells["passportId"].Value.ToString();
                if (string.IsNullOrEmpty(passportId) || !ValidatePassportID(passportId) || Queries.IsInDB("client", "passportId", passportId, id))
                {
                    return ErrorCode.WRONG_PASSPORT_ID;
                }

                if (row.Cells["birthPlace"].Value == null)
                {
                    return ErrorCode.WRONG_BIRTH_PLACE;
                }
                string birthPlace = row.Cells["birthPlace"].Value.ToString();
                if (string.IsNullOrEmpty(birthPlace))
                {
                    return ErrorCode.WRONG_BIRTH_PLACE;
                }

                if (row.Cells["address"].Value == null)
                {
                    return ErrorCode.WRONG_ADDRESS;
                }

                if (row.Cells["phoneNumber"].Value != null)
                {
                    string phoneNumber = row.Cells["phoneNumber"].Value.ToString();
                    if (!IsValidPhoneNumber(phoneNumber) && !string.IsNullOrEmpty(phoneNumber))
                    {
                        return ErrorCode.WRONG_PHONE_NUMBER;
                    }
                }

                if (row.Cells["stationaryPhoneNumber"].Value != null)
                {
                    string stationaryPhoneNumber = row.Cells["stationaryPhoneNumber"].Value.ToString();
                    if (!IsValidPhoneNumber(stationaryPhoneNumber) && !string.IsNullOrEmpty(stationaryPhoneNumber))
                    {
                        return ErrorCode.WRONG_STATIONARY_PHONE_NUMBER;
                    }
                }

                if (row.Cells["email"].Value != null)
                {
                    string email = row.Cells["email"].Value.ToString();
                    if (!IsValidEmail(email) && !string.IsNullOrEmpty(email))
                    {
                        return ErrorCode.WRONG_EMAIL;
                    }
                }

                if (row.Cells["isRetired"].Value == null)
                {
                    return ErrorCode.WRONG_IS_RETIRED;
                }
                string isRetired = row.Cells["isRetired"].Value.ToString();
                if (isRetired != "yes" && isRetired != "no")
                {
                    return ErrorCode.WRONG_IS_RETIRED;
                }

                if (row.Cells["monthlyIncome"].Value != null)
                {
                    string monthlyIncome = row.Cells["monthlyIncome"].Value.ToString();
                    if (!double.TryParse(monthlyIncome, out _) && monthlyIncome != "")
                    {
                        return ErrorCode.WRONG_MONTHLY_INCOME;
                    }
                }

                if (row.Cells["conscript"].Value == null)
                {
                    return ErrorCode.WRONG_CONSCRIPT;
                }
                string conscript = row.Cells["conscript"].Value.ToString();
                if (conscript != "yes" && conscript != "no")
                {
                    return ErrorCode.WRONG_CONSCRIPT;
                }

                if (row.Cells["cityOfResidence"].Value == null)
                {
                    return ErrorCode.NOT_FOUND_CITY_OF_RESIDENCE;
                }
                string cityOfResidence = row.Cells["cityOfResidence"].Value.ToString();
                if (!Queries.IsInDB("cityOfResidence", "name", cityOfResidence, null))
                {
                    return ErrorCode.NOT_FOUND_CITY_OF_RESIDENCE;
                }

                if (row.Cells["familyStatus"].Value == null)
                {
                    return ErrorCode.NOT_FOUND_FAMILY_STATUS;
                }
                string familyStatus = row.Cells["familyStatus"].Value.ToString();
                if (!Queries.IsInDB("familyStatus", "status", familyStatus, null))
                {
                    return ErrorCode.NOT_FOUND_FAMILY_STATUS;
                }

                if (row.Cells["citizenship"].Value == null)
                {
                    return ErrorCode.NOT_FOUND_CITIZENSHIP;
                }
                string citizenship = row.Cells["citizenship"].Value.ToString();
                if (!Queries.IsInDB("citizenship", "citizenship", citizenship, null))
                {
                    return ErrorCode.NOT_FOUND_CITIZENSHIP;
                }

                if (row.Cells["disability"].Value == null)
                {
                    return ErrorCode.NOT_FOUND_DISABILITY;
                }
                string disability = row.Cells["disability"].Value.ToString();
                if (!Queries.IsInDB("disability", "type", disability, null))
                {
                    return ErrorCode.NOT_FOUND_DISABILITY;
                }

                

                return ErrorCode.OK;
            }
            catch
            {
                return ErrorCode.UNKNOWN_ERROR;
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^\+\d{7,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }
        public static bool ContainsNonLetters(string input)
        {
            Regex regex = new Regex("[^a-zA-Z]");
            return regex.IsMatch(input);
        }

        public static bool ContainsOnlyTwoLetters(string input)
        {
            Regex regex = new Regex("^[a-zA-Z]{2}$");
            return regex.IsMatch(input);
        }

        public static bool ContainsSixDigits(string input)
        {
            Regex regex = new Regex("^[0-9]{6}$");
            return regex.IsMatch(input);
        }

        public static bool ValidatePassportID(string input)
        {
            Regex regex = new Regex(@"^\d{7}[a-zA-Z]\d{3}[a-zA-Z]{2}\d$");
            return regex.IsMatch(input);
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}
