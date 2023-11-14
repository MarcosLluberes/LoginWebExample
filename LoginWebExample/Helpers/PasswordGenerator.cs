using System.Text.RegularExpressions;

namespace LoginWebExample.Helpers
{
    public static class PasswordGenerator
    {
        /// <summary>
        /// Generates a random password based on the rules passed in the parameters
        /// </summary>
        /// <param name="includeLowercase">The password must contain lowercase characters</param>
        /// <param name="includeUppercase">The password must contain uppercase characters</param>
        /// <param name="includeNumeric">The password must contain numeric characters</param>
        /// <param name="includeSpecial">The password must contain special characters</param>
        /// <param name="includeSpaces">The password must contain white spaces</param>
        /// <param name="passwordLen">Password length. Must be between 10 and 128</param>
        /// <returns></returns>
        public static string GeneratePassword(bool includeLowercase = true, bool includeUppercase = true, bool includeNumeric = true, bool includeSpecial = true, bool includeSpaces = false, int passwordLen = 10)
        {
            Random random;
            string characterSet = string.Empty;

            char[] password;
            int characterSetLength;

            const int maxIdenticalChars = 2;
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numericChars = "0123456789";
            const string specialChars = @"!#$%&*@\";
            const string spaceChar = " ";
            const int minPasswordLen = 6;
            const int maxPasswordLen = 128;


            if (passwordLen < minPasswordLen || passwordLen > maxPasswordLen)
                return $"INVALID_PASSWORD : Password length must be between {minPasswordLen} and {maxPasswordLen}.";

            if (includeLowercase)
                characterSet += lowercaseChars;

            if (includeUppercase)
                characterSet += uppercaseChars;

            if (includeNumeric)
                characterSet += numericChars;

            if (includeSpecial)
                characterSet += specialChars;

            if (includeSpaces)
                characterSet += spaceChar;

            password = new char[passwordLen];

            random = new();
            characterSetLength = characterSet.Length;

            for (int charPosition = 0; charPosition < passwordLen; charPosition++)
            {
                password[charPosition] = characterSet[random.Next(characterSetLength - 1)];

                bool moreThanTwoIdenticalInARow =
                    charPosition > maxIdenticalChars
                    && password[charPosition] == password[charPosition - 1]
                    && password[charPosition - 1] == password[charPosition - 2];

                if (moreThanTwoIdenticalInARow) charPosition--;
            }

            return string.Join(null, password);
        }

        /// <summary>
        /// Checks if a password match all security rules
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <param name="includeLowercase">Check if the password has lowercase characters</param>
        /// <param name="includeUppercase">Check if the password has uppercase characters</param>
        /// <param name="includeNumeric">Check if the password has numeric characters</param>
        /// <param name="includeSpecial">Check if the password has special characters</param>
        /// <param name="includeSpaces">Check if the password has white spaces</param>
        /// <returns>If True, the password is valid</returns>
        public static bool IsPasswordValid(string password, bool includeLowercase = true, bool includeUppercase = true, bool includeNumeric = true, bool includeSpecial = true, bool includeSpaces = false)
        {
            const string regexLowercase = @"[a-z]";
            const string regexUppercase = @"[A-Z]";
            const string regexNumeric = @"[\d]";
            const string regexSpecial = @"([?!#$%&*@\\])+";
            const string regexSpace = @"([ ])+";

            bool lowerCaseIsValid = !includeLowercase || (includeLowercase && Regex.IsMatch(password, regexLowercase));
            bool upperCaseIsValid = !includeUppercase || (includeUppercase && Regex.IsMatch(password, regexUppercase));
            bool numericIsValid = !includeNumeric || (includeNumeric && Regex.IsMatch(password, regexNumeric));
            bool symbolsAreValid = !includeSpecial || (includeSpecial && Regex.IsMatch(password, regexSpecial));
            bool spacesAreValid = !includeSpaces || (includeSpaces && Regex.IsMatch(password, regexSpace));

            return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid && spacesAreValid;
        }
    }
}