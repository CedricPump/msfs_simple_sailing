using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace msfs_simple_sail_core.Core
{
    internal class VersionHelper
    {
        private const string currentVersion = "v0.0.1";

        public static string GetVersion() 
        {
            return currentVersion;
        }
    }
}
