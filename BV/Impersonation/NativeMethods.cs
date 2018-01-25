using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace VB.Common.Impersonation
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">A handle to an open object.</param>
        /// <returns><c>True</c> when succeeded; otherwise <c>false</c>.</returns>
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        /// <summary>
        /// Attempts to log a user on to the local computer.
        /// </summary>
        /// <param name="username">This is the name of the user account to log on to. 
        /// If you use the user principal name (UPN) format, user@DNSdomainname, the 
        /// domain parameter must be <c>null</c>.</param>
        /// <param name="domain">Specifies the name of the domain or server whose 
        /// account database contains the lpszUsername account. If this parameter 
        /// is <c>null</c>, the user name must be specified in UPN format. If this 
        /// parameter is ".", the function validates the account by using only the 
        /// local account database.</param>
        /// <param name="password">The password</param>
        /// <param name="logonType">The logon type</param>
        /// <param name="logonProvider">The logon provides</param>
        /// <param name="userToken">The out parameter that will contain the user 
        /// token when method succeeds.</param>
        /// <returns><c>True</c> when succeeded; otherwise <c>false</c>.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return:MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Logon", Justification = "Win32 Naming Convention")]
        public static extern bool LogonUser(
            [MarshalAs(UnmanagedType.LPTStr)] string username,
            [MarshalAs(UnmanagedType.LPTStr)] string domain,
            [MarshalAs(UnmanagedType.LPTStr)] string password,
            LogonType logonType, LogonProvider logonProvider, out IntPtr userToken);

        /// <summary>
        /// Creates a new access token that duplicates one already in existence.
        /// </summary>
        /// <param name="token">Handle to an access token.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="duplication">Reference to the token to duplicate.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateToken(IntPtr token, int impersonationLevel,
                                                  ref IntPtr duplication);

        /// <summary>
        /// The ImpersonateLoggedOnUser function lets the calling thread impersonate the 
        /// security context of a logged-on user. The user is represented by a token handle.
        /// </summary>
        /// <param name="userToken">Handle to a primary or impersonation access token that represents a logged-on user.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static extern bool ImpersonateLoggedOnUser(IntPtr userToken);
    }
}
