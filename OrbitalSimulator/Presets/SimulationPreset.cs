using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using OrbitalSimulator.PhysicsEngine;
using OrbitalSimulator.Presets;

namespace OrbitalSimulator.Models
{
    internal class SimulationPreset
    {

        #region Fields

        readonly List<BodyPreset> bodies = new();

        #endregion Fields

        #region Properties

        public string Name { get; set; }

        public string IntegrationType { get; set; }

        public double TimeSpan { get; set; }

        public double TimeResolution { get; set; }

        public List<BodyPreset> Bodies => bodies;

        #endregion Properties

        #region Public Constructors

        public SimulationPreset(string name, double timeSpan, double timeResolution, string integrationType)
        { 
            Name = name;
            IntegrationType = integrationType;
            TimeSpan = timeSpan;
            TimeResolution = timeResolution;
        }

        [JsonConstructor]
        public SimulationPreset(string name, double timeSpan, double timeResolution, string integrationType, List<BodyPreset> bodies) : this(name, timeSpan, timeResolution, integrationType)
        {
            this.bodies = bodies;
        }

        #endregion Public Constructors

        #region Public Methods

        public static Engine? Initialize(string presetName)
        {
            SimulationPreset? preset = Load(presetName);

            if (preset is not null)
            {
                List<Body> bodies = new();
                Integrator.IntegrationType? integrationType;

                foreach (BodyPreset bodyPreset in preset.Bodies)
                {
                    Vector3 initialVelocity = new(bodyPreset.InitialVelocity);
                    Vector3 initialPosition = new(bodyPreset.InitialPosition);

                    Body body = new(bodyPreset.Mass, bodyPreset.IsMassive, bodyPreset.IsFixed, initialVelocity, initialPosition);

                    if (!string.IsNullOrEmpty(bodyPreset.Name)) body.Name = bodyPreset.Name;

                    bodies.Add(body);
                }

                try
                {
                    integrationType = Integrator.GetIntegrationType(preset.IntegrationType);

                    if (integrationType is null) throw new Exception($"Integration type {preset.IntegrationType} is invalid.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    return null;
                }

                return new Engine(preset.TimeSpan, preset.TimeResolution, (Integrator.IntegrationType)integrationType, bodies);
            }
            return null;
        }
        public static void Save(SimulationPreset preset, string path)
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(preset, options);

            File.WriteAllText(path + ".json", json);

            Console.WriteLine("Preset saved successfully.");
            Console.WriteLine();
        }

        public static SimulationPreset CreateFromEngine(Engine engine, string name)
        {
            string integrationType = Integrator.GetIntegrationTypeStr(engine.IntegrationType);
            SimulationPreset? preset = new(name, engine.TimeSpan, engine.TimeResolution, integrationType);

            List<BodyPreset> bodyPresets = new();

            foreach (Body body in engine.Bodies)
            {
                BodyPreset bodyPreset = new(body.IsMassive, body.IsFixed, body.Mass, body.InitialPosition.Components, body.InitialVelocity.Components, body.Name);
                bodyPresets.Add(bodyPreset);
            }

            preset.AddBodies(bodyPresets);

            return preset;
        }

        public static SimulationPreset? Load(string presetName)
        {
            try
            {
                string jsonStr = File.ReadAllText(Path.GetFullPath(presetName));

                SimulationPreset? preset = JsonSerializer.Deserialize<SimulationPreset>(jsonStr);

                if (preset is null) throw new Exception($"The preset {Path.GetFileNameWithoutExtension(presetName)} is corrupted.");

                return preset!;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The preset {Path.GetFullPath(presetName)} does not exist.");
                Console.WriteLine(e.Message);
                Console.WriteLine();

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();

                return null;
            }
        }

        public void AddBody(BodyPreset body) => bodies.Add(body);
        public void AddBodies(List<BodyPreset> bodies) => this.bodies.AddRange(bodies);

        #endregion Public Methods

    }
}
