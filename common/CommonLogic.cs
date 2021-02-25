using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace common
{

    public class CommonLogic
    {
        public static void Run(string[] args)
        {
            if (args.Length != 6 && args.Length != 8 && args.Length != 10)
            {
                Console.WriteLine("Usage");
                Console.WriteLine("-m mode [ser|des] - required");
                Console.WriteLine("-f file");
                Console.WriteLine("-t [all|auto|none] - required");
                Console.WriteLine("-a [simple|full] - required");
                Console.WriteLine("-x [netcore|default]");
                return;
            }

            var flags = args.Where((_, i) => i % 2 == 0);
            var values = args.Where((_, i) => i % 2 == 1);
            args = flags.Zip(values, (flag, value) => $"{flag} {value}").ToArray();

            //args = new string[] {"-m ser", "-t all", "-a full"};
            var file = GetTargetPath(args);

            var mode = GetMode(args);
            var settings = GetJsonSerializerSettings(GetTypeNameHandling(args), GetAssemblyHandling(args));
            var data = File.ReadAllText(file);
            settings.SerializationBinder = GetBinder(args);

            switch (mode)
            {
                case CommonLogic.Mode.Ser:
                    {
                        if (data != string.Empty)
                        {
                            Console.WriteLine("warn: target file not empty");
                            Console.WriteLine(data);
                        }

                        var result = JsonConvert.SerializeObject(new Model(), Formatting.Indented, settings);

                        Console.WriteLine("successfully serialized");
                        Console.WriteLine(result);

                        File.WriteAllText(file, result);

                        break;
                    }
                case CommonLogic.Mode.Des:
                    {
                        if (data == string.Empty) throw new ArgumentException("target file doesn't exist or is empty");

                        var result = JsonConvert.DeserializeObject<Model>(data, settings);

                        Console.WriteLine("successfully deserialized");
                        Console.WriteLine(data);
                        Console.WriteLine();
                        Console.WriteLine("result");
                        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented, settings));
                        break;
                    }
            };

            Console.WriteLine("Done. Press any key to end...");
            Console.ReadKey();
        }


        private enum Mode
        {
            Ser,
            Des
        }

        private static Mode GetMode(string[] args)
        {
            var mode = args.SingleOrDefault(x => x.StartsWith("-m"));
            if (mode == null) throw new ArgumentNullException("Expecting mode -m switch with values [ser|des]");

            mode = mode.Split(' ')[1];

            if (!Enum.TryParse<Mode>(mode, true, out var result))
            {
                throw new ArgumentException("Expecting mode -m switch with values [ser|des]");
            }

            return result;
        }

        private static ISerializationBinder GetBinder(string[] args)
        {
            var mode = args.SingleOrDefault(x => x.StartsWith("-b"));
            if (mode == null) return new DefaultSerializationBinder();

            mode = mode.Split(' ')[1];
            
            return mode switch
            {
                "netcore" => new NetCoreSerializationBinder(),
                "default" => new DefaultSerializationBinder(),
                _ => throw new ArgumentException("Expecting mode -b switch with values [netcore|default]")
            };
        }

        private static string GetTargetPath(string[] args)
        {
            var path = args.SingleOrDefault(x => x.StartsWith("-f"));

            if (path == null)
            {
                Console.WriteLine("warning: target file not passed, using `../../../../data.json`");
                return Path.GetFullPath("../../../../data.json");
            }

            try
            {
                File.ReadAllText(path);
                return path;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"fail: failed to read target file, panic: {ex}");

                throw;
            }

        }

        private static TypeNameHandling GetTypeNameHandling(string[] args)
        {
            var typehandling = args.SingleOrDefault(x => x.StartsWith("-t"));

            if (typehandling == null) throw new ArgumentNullException("expecting argument -t with values [all|auto|none]");

            var value = typehandling.Split(' ')[1];

            return value switch
            {
                "all" => TypeNameHandling.All,
                "auto" => TypeNameHandling.Auto,
                "none" => TypeNameHandling.None,
                _ => throw new ArgumentException("expecting argument -t with values [all|auto|none]")
            };
        }

        private static TypeNameAssemblyFormatHandling GetAssemblyHandling(string[] args)
        {
            var typehandling = args.SingleOrDefault(x => x.StartsWith("-a"));

            if (typehandling == null) throw new ArgumentNullException("expecting argument -a with values [simple|full]");

            var value = typehandling.Split(' ')[1];

            return value switch
            {
                "simple" => TypeNameAssemblyFormatHandling.Simple,
                "full" => TypeNameAssemblyFormatHandling.Full,
                _ => throw new ArgumentException("expecting argument -a with values [simple|full]")
            };
        }

        private static JsonSerializerSettings GetJsonSerializerSettings(
            TypeNameHandling typeNameHandling,
            TypeNameAssemblyFormatHandling typeNameAssemblyFormatHandling)
            => new JsonSerializerSettings()
            {
                TypeNameHandling = typeNameHandling,
                TypeNameAssemblyFormatHandling = typeNameAssemblyFormatHandling
            };
    }
}
