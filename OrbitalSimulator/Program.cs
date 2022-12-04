using System;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

using OrbitalSimulator.Models;
using OrbitalSimulator.PhysicsEngine;
using OrbitalSimulator.Presets;
using OrbitalSimulator.Utilities;

namespace PhysicsEngine
{
    internal class Program
    {
        static string outputPath = string.Empty;
        static string plottingPath = string.Empty;
        static string presetPath = string.Empty;
        static void Main(string[] args)
        {
            LoadConfig();

            if (args.Length == 1) Engine.Initialize(Path.GetFullPath(args[0]));
            else Console.WriteLine("Invalid input.");
        }

        static void LoadConfig()
        {
            string configPath = Path.GetFullPath("config.json");

            try
            {
                Config? config = JsonSerializer.Deserialize<Config>(configPath);

                if (config == null) throw new Exception("The file config.json could not be deserialized. Recreating with default paths and reloading.");

                outputPath = Path.GetFullPath(config.OutputPath);
                plottingPath = Path.GetFullPath(config.PlottingPath);
                presetPath = Path.GetFullPath(config.PresetPath);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("The file config.json was not found. Recreating with default paths and reloading.");
                Console.WriteLine(e.Message);
                Console.WriteLine();

                Config.RecreateDefaultConfig();
                LoadConfig();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();

                Config.RecreateDefaultConfig();
                LoadConfig();
            }
        }
    }
}