using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AuthenticationServer.API.Helpers
{
    public static class SecureStringHelpers
    {
        public static SecureString ConvertStringToSecureString(this string normalString)
        {
            var secureString = new SecureString();

            if (normalString.Length > 0)
            {
                foreach (var ch in normalString.ToCharArray())
                {
                    secureString.AppendChar(ch);
                }
            }

            return secureString;
        }

        public static string ConvertSecureStringToString(this SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                return Marshal.PtrToStringUni(unmanagedString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return null;
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}
