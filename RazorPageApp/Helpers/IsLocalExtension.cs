// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace RazorPageApp.Helpers
{
    public static class IsLocalExtension
    {
        private const string NullIpAddress = "::1";

        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress.IsSet())
            {
                //We have a remote address set up
                return connection.LocalIpAddress.IsSet() 
                      //Is local is same as remote, then we are local
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress) 
                      //else we are remote if the remote IP address is not a loopback address
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }

        public static void ThrowErrorIfNotLocal(this HttpRequest req)
        {
            if (!req.IsLocal())
                throw new InvalidOperationException("You can only call this command if you are running locally");
        }

        private static bool IsSet(this IPAddress address)
        {
            return address != null && address.ToString() != NullIpAddress;
        }
    }
}