using System;

namespace AllReady.NotificationsWebJob
{
    static class EnvironmentHelper
    {
        public static string TryGetEnvironmentVariable(string variable)
        {
            var candidate = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
            if (!string.IsNullOrEmpty(candidate))
            {
                return candidate;
            }

            candidate = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrWhiteSpace(candidate))
            {
                throw new Exception($"Can't find `{variable}` environment variable.");
            }

            return candidate;
        }
    }
}