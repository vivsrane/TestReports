using System;
using System.ComponentModel;
using System.Security.Principal;

namespace VB.Common.Impersonation
{
    public class ImpersonationCodeSection : IDisposable
    {
        #region Private members
        /// <summary>
        /// <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Holds the created impersonation context and will be used
        /// for reverting to previous user.
        /// </summary>
        private WindowsImpersonationContext _impersonationContext;
        #endregion

        #region Ctor & Dtor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpersonationCodeSection"/> class and
        /// impersonates with the specified credentials.
        /// </summary>
        /// <param name="userName">his is the name of the user account to log on 
        /// to. If you use the user principal name (UPN) format, 
        /// user@DNS_domain_name, the lpszDomain parameter must be <c>null</c>.</param>
        /// <param name="domain">The name of the domain or server whose account 
        /// database contains the lpszUsername account. If this parameter is 
        /// <c>null</c>, the user name must be specified in UPN format. If this 
        /// parameter is ".", the function validates the account by using only the 
        /// local account database.</param>
        /// <param name="password">The plaintext password for the user account.</param>
        public ImpersonationCodeSection(String userName, String domain, String password)
        {
            IntPtr userToken = IntPtr.Zero;
            IntPtr userTokenDuplication = IntPtr.Zero;

            // Logon with user and get token.
            bool loggedOn = NativeMethods.LogonUser(userName, domain, password,
                LogonType.Interactive, LogonProvider.Default,
                out userToken);           

            if (loggedOn)
            {
                try
                {
                    // Create a duplication of the usertoken, this is a solution
                    // for the known bug that is published under KB article Q319615.
                    if (NativeMethods.DuplicateToken(userToken, 2, ref userTokenDuplication))
                    {
                        // Create windows identity from the token and impersonate the user.
                        WindowsIdentity identity = new WindowsIdentity(userTokenDuplication);
                        _impersonationContext = identity.Impersonate();
                    }
                    else
                    {
                        // Token duplication failed!
                        // Use the default ctor overload
                        // that will use Mashal.GetLastWin32Error();
                        // to create the exceptions details.
                        throw new Win32Exception();
                    }
                }
                finally
                {
                    // Close usertoken handle duplication when created.
                    if (!userTokenDuplication.Equals(IntPtr.Zero))
                    {
                        // Closes the handle of the user.
                        NativeMethods.CloseHandle(userTokenDuplication);
                        userTokenDuplication = IntPtr.Zero;
                    }

                    // Close usertoken handle when created.
                    if (!userToken.Equals(IntPtr.Zero))
                    {
                        // Closes the handle of the user.
                        NativeMethods.CloseHandle(userToken);
                        userToken = IntPtr.Zero;
                    }
                }
            }
            else
            {
                // Logon failed!
                // Use the default ctor overload that 
                // will use Mashal.GetLastWin32Error();
                // to create the exceptions details.
                throw new Win32Exception();
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ImpersonationCodeSection"/> is reclaimed by garbage collection.
        /// </summary>
        ~ImpersonationCodeSection()
        {
            Dispose(false);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Reverts to the previous user.
        /// </summary>
        public void Revert()
        {
            if (_impersonationContext != null)
            {
                // Revert to previour user.
                _impersonationContext.Undo();
                _impersonationContext = null;
            }
        }
        #endregion

        #region IDisposable implementation.
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources and will revent to the previous user when
        /// the impersonation still exists.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources and will revent to the previous user when
        /// the impersonation still exists.
        /// </summary>
        /// <param name="disposing">Specify <c>true</c> when calling the method directly
        /// or indirectly by a user's code; Otherwise <c>false</c>.
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Revert();

                _disposed = true;
            }
        }
        #endregion
    }
}
