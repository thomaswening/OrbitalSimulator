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
        static readonly string dataPath = @"E:\Users\thoma\Desktop";
        static void Main(string[] args)
        {
            //SavePreset(CreatePresetFromEngine(InitializeExample(Example.SolarSystem), "SolarSystem"), Path.Combine(dataPath, "SolarSystem"));
            //SavePreset(CreatePresetFromEngine(InitializeExample(Example.EarthMoonIss), "EarthMoonIss"), Path.Combine(dataPath, "EarthMoonIss"));

            Engine? engine = InitializePreset(Path.Combine(dataPath, "SolarSystem.json"));
            engine?.Run();
            engine?.PrintToFile(Path.Combine(dataPath, "SolarSystem.dat"));

            //engine = InitializePreset(Path.Combine(dataPath, "EarthMoonIss.json"));
            //engine?.Run();
            //engine?.PrintToFile(Path.Combine(dataPath, "EarthMoonIss.dat"));

            //if (args.Length == 1) InitializeEngine(args[0]);
            //else Console.WriteLine("Invalid input.");
        }

        static void InitializeEngine(string presetName)
        {
            Engine? engine = InitializePreset(presetName);
            engine?.Run();
        }

        
        static Engine? InitializePreset(string presetName) 
        { 
            SimulationPreset? preset = LoadPreset(presetName);

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

        static SimulationPreset? LoadPreset(string presetName)
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

        static void SavePreset(SimulationPreset preset, string path)
        {
            JsonSerializerOptions options = new() { WriteIndented = true };
            string json = JsonSerializer.Serialize(preset, options);

            File.WriteAllText(path + ".json", json);

            Console.WriteLine("Preset saved successfully.");
            Console.WriteLine();
        }

        static SimulationPreset CreatePresetFromEngine(Engine engine, string name)
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

        enum Example
        {
            SolarSystem,
            EarthMoonIss
        }

        static Engine InitializeExample(Example example)
        {
            List<Body> simulationBodies = new();

            Body earth = new(
                    mass: 5.972e24,
                    initialVelocity: new Vector3(0, 30.29e3, 0),
                    initialPosition: new Vector3(1.471e11, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Earth"
                );
            simulationBodies.Add(earth);

            switch (example)
            {
                case Example.EarthMoonIss:
                    AddEarthMoonIssBodies(simulationBodies);
                    break;

                case Example.SolarSystem:
                    AddSolarSystemBodies(simulationBodies);
                    break;

                default: throw new NotImplementedException();
            }            

            double timeSpan = 1 * 365 * 24 * Math.Pow(60, 2);   // 1 Earth year
            double timeResolution = 24 * Math.Pow(60, 2);       // 1 day

            return new Engine(timeSpan, timeResolution, Integrator.IntegrationType.LeapFrog, simulationBodies);
        }

        private static void AddSolarSystemBodies(List<Body> simulationBodies)
        {
            Body sun = new(
                mass: 1.989e30,
                initialVelocity: Vector3.Zero,
                initialPosition: Vector3.Zero,
                isMassive: true,
                isFixed: true,
                name: "Sun"
                );
            simulationBodies.Add(sun);

            Body mercury = new(
                mass: 3.285e23,
                initialVelocity: new Vector3(0, 58.98e3, 0),
                initialPosition: new Vector3(4.600e10, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Mercury"
                );
            simulationBodies.Add(mercury);

            Body venus = new(
                mass: 4.867e24,
                initialVelocity: new Vector3(0, 35.26e3, 0),
                initialPosition: new Vector3(1.075e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Venus"
                );
            simulationBodies.Add(venus);

            Body mars = new(
                mass: 6.39e23,
                initialVelocity: new Vector3(0, 26.50e3, 0),
                initialPosition: new Vector3(2.066e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Mars"
                );
            simulationBodies.Add(mars);

            Body jupiter = new(
                mass: 1.898e27,
                initialVelocity: new Vector3(0, 13.72e3, 0),
                initialPosition: new Vector3(7.407e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Jupiter"
                );
            simulationBodies.Add(jupiter);

            Body saturn = new(
                mass: 5.683e27,
                initialVelocity: new Vector3(0, 10.18e3, 0),
                initialPosition: new Vector3(1.349e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Saturn"
                );
            simulationBodies.Add(saturn);

            Body uranus = new(
                mass: 8.681e25,
                initialVelocity: new Vector3(0, 7.11e3, 0),
                initialPosition: new Vector3(2.736e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Uranus"
                );
            simulationBodies.Add(uranus);

            Body neptune = new(
                mass: 1.024e26,
                initialVelocity: new Vector3(0, 5.5e3, 0),
                initialPosition: new Vector3(4.459e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Neptune"
                );
            simulationBodies.Add(neptune);
        }

        private static void AddEarthMoonIssBodies(List<Body> simulationBodies)
        {
            Body moon = new(
                    mass: 7.348e22,
                    initialVelocity: new Vector3(0, 30e3 + 1.022e3, 0),
                    initialPosition: new Vector3(150e9 + 385e6, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Moon"
                );
            simulationBodies.Add(moon);

            Body iss = new(
                    mass: 444.615e3,
                    initialVelocity: new Vector3(0, 30e3 + 7.66e3, 0),
                    initialPosition: new Vector3(150e9 + 6.371e6 + 412e3, 0, 0),
                    isMassive: false,
                    isFixed: false,
                    name: "ISS"
                );
            simulationBodies.Add(iss);
        }
    }
}