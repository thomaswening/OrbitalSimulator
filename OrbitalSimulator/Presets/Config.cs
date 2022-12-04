using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrbitalSimulator.Presets
{
    internal class Config
    {
        public string OutputPath { get; set; } = string.Empty;
        public string PresetPath { get; set; } = string.Empty;
        public string PlottingPath { get; set; } = string.Empty;

        public Config() { }

        public Config(string outputPath, string presetPath, string plottingPath) 
        {
            OutputPath = outputPath;
            PresetPath = presetPath;
            PlottingPath = plottingPath;
        }

        public static void RecreateDefaultConfig()
        {
            Config config = new(DefaultConfigConstants.OUTPUT_PATH, DefaultConfigConstants.PRESET_PATH, DefaultConfigConstants.PLOTTING_PATH);

            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);

            File.WriteAllText("config.json", json);

            Console.WriteLine("Default config recreated successfully.");
            Console.WriteLine();
        }
    }
}
