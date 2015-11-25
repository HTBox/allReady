using System.Text;

namespace AllReady.Security
{
    /// <summary>
    /// Central class for password requirements and user helpers
    /// </summary>
    public class PasswordRequirements
    {
        /// <summary>
        /// The password length used within the application
        /// </summary>
        public const int PASSWORD_LENGTH = 10;

        /// <summary>
        /// If the password requires a digit
        /// </summary>
        public const bool REQUIRE_DIGIT = true;

        /// <summary>
        /// If the password requires a digit or non alphabetical letter
        /// </summary>
        public const bool REQUIRE_NON_ALPHA_OR_DIGIT = false;

        /// <summary>
        /// If the password must have an uppercase character
        /// </summary>
        public const bool REQUIRE_UPPERCASE = false;

        /// <summary>
        /// A user helper string describing the password policy requirements
        /// </summary>
        public static string PasswordGuidance
        {
            get
            {
                var output = new StringBuilder();

                output.Append("Your password must be ");
                output.Append(PASSWORD_LENGTH);
                output.Append(" characters in length");

                if (REQUIRE_NON_ALPHA_OR_DIGIT && !REQUIRE_UPPERCASE)
                    output.Append(" and one symbol or number");
                else if (REQUIRE_NON_ALPHA_OR_DIGIT && REQUIRE_UPPERCASE)
                    output.Append(", one symbol or number and one uppercase letter");
                else if (REQUIRE_DIGIT && REQUIRE_UPPERCASE)
                    output.Append(", one number and one uppercase letter");
                else if (REQUIRE_DIGIT)
                    output.Append(" and must include one number");

                output.Append(".");

                return output.ToString();
            }
        }
    }
}
