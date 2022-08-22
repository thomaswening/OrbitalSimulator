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

            Body earth = new(
                    pMass: 5.972e24,
                    pInitialVelocity : new Vector3(0, 30e3, 0),
                    pInitialPosition: new Vector3(150e9, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Earth"
                );
            simulationBodies.Add(earth);

            Body moon = new(
                    pMass: 7.348e22,
                    pInitialVelocity: new Vector3(0, 30e3 + 1.022e3, 0),
                    pInitialPosition: new Vector3(150e9 + 385e6, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false,
                    pName: "Moon"
                );
            simulationBodies.Add(moon);

            Body iss = new(
                    pMass: 444.615e3,
                    pInitialVelocity: new Vector3(0, 30e3 + 7.66e3, 0),
                    pInitialPosition: new Vector3(150e9 + 6.371e6 + 408e3, 0, 0),
                    pIsMassive: false,
                    pIsFixed: false,
                    pName: "ISS"
                );
            simulationBodies.Add(iss);

            double timeSpan = 365 * 24 * Math.Pow(60, 2);
            double timeResolution = 60;

            Engine simulation = new(timeSpan, timeResolution, simulationBodies);
            simulation.Run();

            //simulation.PrintToScreen();
            simulation.PrintToFile();

            MakePlot();
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