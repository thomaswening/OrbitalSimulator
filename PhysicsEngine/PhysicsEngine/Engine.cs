﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    /// <summary>
    /// Evaluates the physics between all bodies and calculates the bodies' trajectories.
    /// </summary>
    internal class Engine
    {
        List<Body> bodies = new List<Body>();
        double timeSpan;
        double timeResolution;
        List<double> timeSamples = new List<double> { 0 };

        public Engine(double pTimeSpan, double pTimeResolution)
        {
            if (pTimeSpan < 0)
            {
                Console.WriteLine("The time span must be greater than zero.");
                return;
            }

            if (pTimeSpan < pTimeResolution)
            {
                Console.WriteLine("The time span must be greater than the time resolution.");
                return;
            }

            timeSpan = pTimeSpan;
            timeResolution = pTimeResolution;
        }

        public Engine(double pTimeSpan, double timeResolution, List<Body> pBodies) : this(pTimeSpan, timeResolution)
        {
            bodies.AddRange(pBodies);
        }

        public Engine(double pTimeSpan, double timeResolution, Body pBody) : this(pTimeSpan, timeResolution)
        {
            bodies.Add(pBody);
        }

        public void AddBody(Body pBody) => bodies.Add(pBody);

        /// <summary>
        /// Runs the simulation and updates the bodies' trajectory along the way.
        /// </summary>
        /// <param name="pPrint"></param>
        public void Run()
        {
            Console.WriteLine("Running simulation... ");
            using (var progress = new ProgressBar())
            {
                double simulationTime = timeResolution;

                while (simulationTime <= timeSpan)
                {
                    timeSamples.Add(simulationTime);

                    foreach (Body body in bodies)
                    {
                        if (body.IsFixed)
                        {
                            body.UpdateTrajectory(simulationTime);
                        }
                        else
                        {
                            body.CurrentPosition += timeResolution * body.CurrentVelocity;

                            if (body.Mass != 0)
                            {
                                Vector3 netForce = EvaluateNetForceOn(body);
                                body.CurrentPosition += Convert.ToSingle(0.5F * Math.Pow(timeResolution, 2) / body.Mass) * netForce;
                                body.CurrentVelocity += timeResolution / body.Mass * netForce;
                            }
            
                            body.UpdateTrajectory(simulationTime);
                        }
                    }

                    progress.Report(simulationTime / timeSpan);
                    simulationTime += timeResolution;
                }
            }

            Console.WriteLine("\nDone. Starting plotting script...");
        }

        Vector3 EvaluateNetForceOn(Body pBody)
        {
            Vector3 netForce = Vector3.Zero;

            foreach (Body body in bodies)
            {
                if (body.IsMassive && body != pBody)
                {
                    netForce += body.GetForceOn(pBody);
                }
            }

            return netForce;
        }

        public string ToString(string pDelimiter = ",")
        {
            StringBuilder sb = new();

            // Header

            sb.Append($"Simulation Run Results\n\n");
            sb.Append($"time span (s): {timeSpan}\n");
            sb.Append($"time resolution (s): {timeResolution}\n");
            sb.Append($"=> number of time samples: {timeSamples.Count}\n\n");

            sb.Append($"time (s)" + pDelimiter);
            foreach(Body body in bodies)
            {
                sb.Append($"{body.Name} X (km)" + pDelimiter);
                sb.Append($"{body.Name} Y (km)" + pDelimiter);
                sb.Append($"{body.Name} Z (km)" + pDelimiter);
            }

            EndLine(sb, pDelimiter);
            sb.Append("\n");

            // Body

            for (int i = 0; i < timeSamples.Count; i++)
            {
                sb.Append(timeSamples[i] + pDelimiter);
                foreach (Body body in bodies)
                {
                    sb.Append(body.Trajectory[i].X + pDelimiter);
                    sb.Append(body.Trajectory[i].Y + pDelimiter);
                    sb.Append(body.Trajectory[i].Z + pDelimiter);
                }

                EndLine(sb, pDelimiter);
            }

            return sb.ToString();
        }

        public override string ToString() => ToString();

        public void PrintToScreen(string pDelimiter = ",") => Console.WriteLine(ToString());

        public void PrintToFile(string pDelimiter = ",")
        {
            using StreamWriter file = new("E:\\Users\\thoma\\Desktop\\simulation_run.txt");
            file.WriteLine(ToString());
        }

        static void EndLine(StringBuilder pStringBuilder, string pDelimiter)
        {
            pStringBuilder.Remove(pStringBuilder.Length - pDelimiter.Length, pDelimiter.Length);
            pStringBuilder.Append('\n');
        }

        void PrintProgressBar(double pSimulationTime, double pTimeSpan)
        {
            double progressPercent = 100 * pSimulationTime / pTimeSpan;
            StringBuilder bar = new();

            for (int i = 0; i < progressPercent/10; i++)
            {
                bar.Append('=');
            }

            bar.Append('>');

            for (int i = bar.Length; i < 10; i++)
            {
                bar.Append(' ');
            }

            ClearCurrentConsoleLine();
            Console.Write("[" + bar.ToString() + "]");
        }
        static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
