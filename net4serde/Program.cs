using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using common;
using Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace net4serde
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonLogic.Run(args);
        }
    }
}
