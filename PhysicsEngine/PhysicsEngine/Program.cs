using System;
using System.Diagnostics;
using System.Text;

namespace PhysicsEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Body> simulationBodies = new List<Body>();

            Body sun = new(
                    pMass: 1.989e30,
                    pInitialVelocity: Vector3.Zero,
                    pInitialPosition: Vector3.Zero,
                    pIsMassive: true,
                    pIsFixed: true,
                    pName: "Sun"
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
                   pMass: 3.285e23,
                   pInitialVelocity: new Vector3(0, 58.98e3, 0),
                   pInitialPosition: new Vector3(4.600e10, 0, 0),
                   pIsMassive: true,
                   pIsFixed: false,
                   pName: "Mercury"
               );
            simulationBodies.Add(mercury);

            Body venus = new(
                   pMass: 4.867e24,
                   pInitialVelocity: new Vector3(0, 35.26e3, 0),
                   pInitialPosition: new Vector3(1.075e11, 0, 0),
                   pIsMassive: true,
                   pIsFixed: false,
                   pName: "Venus"
               );
            simulationBodies.Add(venus);

            Body earth = new(
                    pMass: 5.972e24,
                    pInitialVelocity: new Vector3(0, 30.29e3, 0),
                    pInitialPosition: new Vector3(1.471e11, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Earth"
                );
            simulationBodies.Add(earth);

            Body mars = new(
                    pMass: 6.39e23,
                    pInitialVelocity: new Vector3(0, 26.50e3, 0),
                    pInitialPosition: new Vector3(2.066e11, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Earth"
                );
            simulationBodies.Add(mars);

            Body jupiter = new(
                    pMass: 1.898e27,
                    pInitialVelocity: new Vector3(0, 13.72e3, 0),
                    pInitialPosition: new Vector3(7.407e11, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Jupiter"
                );
            simulationBodies.Add(jupiter);

            Body saturn = new(
                    pMass: 5.683e27,
                    pInitialVelocity: new Vector3(0, 10.18e3, 0),
                    pInitialPosition: new Vector3(1.349e12, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Saturn"
                );
            simulationBodies.Add(saturn);

            Body uranus = new(
                    pMass: 8.681e25,
                    pInitialVelocity: new Vector3(0, 7.11e3, 0),
                    pInitialPosition: new Vector3(2.736e12, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Uranus"
                );
            simulationBodies.Add(uranus);

            Body neptune = new(
                    pMass: 1.024e26,
                    pInitialVelocity: new Vector3(0, 5.5e3, 0),
                    pInitialPosition: new Vector3(4.459e12, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Neptune"
                );
            simulationBodies.Add(neptune);


            double timeSpan = 1 * 365 * 24 * Math.Pow(60, 2);
            double timeResolution = Math.Pow(60, 2);

            Engine simulation = new(timeSpan, timeResolution, simulationBodies);
            Clock runtimeClock = new(simulation.Run);

            ////simulation.PrintToScreen();
            //simulation.PrintToFile();
            //MakePlot();
            //Console.WriteLine("Plotting done!");
        }

        public static void MakePlot()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "python.exe",
                Arguments = @"E:\Users\thoma\source\repos\OrbitalSimulator\Plotting\plotting.py",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process process = new() 
            { 
                StartInfo = startInfo,
                EnableRaisingEvents = true 
            };

            process.OutputDataReceived += new DataReceivedEventHandler((sender, args) => Console.WriteLine(args.Data));
            process.ErrorDataReceived += new DataReceivedEventHandler((sender, args) => Console.WriteLine(args.Data));
            process.Exited += new EventHandler((sender, args) =>
            {
                Console.WriteLine(string.Format("process exited with code {0}\n", process.ExitCode.ToString()));
            });

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }
    }
}