using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

using OrbitalSimulator.Models;
using OrbitalSimulator.PhysicsEngine;
using OrbitalSimulator.Presets;
using OrbitalSimulator.Utilities;

using static OrbitalSimulator.PhysicsEngine.Integrator;

namespace PhysicsEngine
{
    internal class Program
    {
        static readonly string dataPath = @"E:\Users\thoma\Desktop";
        static void Main(string[] args)
        {
            if (args.Length == 1) InitializeEngine(args[0]);
            else Console.WriteLine("Invalid input.");
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
                IntegrationType? integrationType;

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
                    integrationType = GetIntegrationType(preset.IntegrationType);

                    if (integrationType is null) throw new Exception($"Integration type {preset.IntegrationType} is invalid.");
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                    return null;
                }

                return new Engine(preset.TimeSpan, preset.TimeResolution, (IntegrationType)integrationType, bodies);
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
            catch(FileNotFoundException e)
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

        static void RunExample()
        {
            List<Body> simulationBodies = new();

            Body sun = new(
                    mass: 1.989e30,
                    initialVelocity: Vector3.Zero,
                    initialPosition: Vector3.Zero,
                    isMassive: true,
                    isFixed: true,
                    name: "Sun"
                );
            simulationBodies.Add(sun);

            //Body earth = new(
            //        pMass: 5.972e24,
            //        pInitialVelocity: new Vector3(0, 30e3, 0),
            //        pInitialPosition: new Vector3(150e9, 0, 0),
            //        pIsMassive: true,
            //        pIsFixed: false,
            //        pName: "Earth"
            //    );
            //simulationBodies.Add(earth);

            //Body moon = new(
            //        pMass: 7.348e22,
            //        pInitialVelocity: new Vector3(0, 30e3 + 1.022e3, 0),
            //        pInitialPosition: new Vector3(150e9 + 385e6, 0, 0),
            //        pIsMassive: true,
            //        pIsFixed: false,
            //        pName: "Moon"
            //    );
            //simulationBodies.Add(moon);

            //Body iss = new(
            //        pMass: 444.615e3,
            //        pInitialVelocity: new Vector3(0, 30e3 + 7.66e3, 0),
            //        pInitialPosition: new Vector3(150e9 + 6.371e6 + 412e3, 0, 0),
            //        pIsMassive: false,
            //        pIsFixed: false,
            //        pName: "ISS"
            //    );
            //simulationBodies.Add(iss);

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

            Body earth = new(
                    mass: 5.972e24,
                    initialVelocity: new Vector3(0, 30.29e3, 0),
                    initialPosition: new Vector3(1.471e11, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Earth"
                );
            simulationBodies.Add(earth);

            Body mars = new(
                    mass: 6.39e23,
                    initialVelocity: new Vector3(0, 26.50e3, 0),
                    initialPosition: new Vector3(2.066e11, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Earth"
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


            double timeSpan = 1 * 365 * 24 * Math.Pow(60, 2);
            double timeResolution = 24 * Math.Pow(60, 2);

            Engine simulation = new(timeSpan, timeResolution, IntegrationType.LeapFrog, simulationBodies);
            Clock runtimeClock = new(simulation.Run);

            Directory.CreateDirectory(Path.Combine(dataPath, @"Output"));
            simulation.PrintToFile(Path.Combine(dataPath, @"Output\output.dat"));
        }
    }
}