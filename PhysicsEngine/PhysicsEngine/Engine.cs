using System;
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

        public Engine(double pTimeSpan, double pTimeResolution, List<Body> pBodies) : this(pTimeSpan, pTimeResolution)
        {
            bodies.AddRange(pBodies);
        }

        public Engine(double pTimeSpan, double pTimeResolution, Body pBody) : this(pTimeSpan, pTimeResolution)
        {
            bodies.Add(pBody);
        }

        public void AddBody(Body pBody) => bodies.Add(pBody);

        /// <summary>
        /// Runs the simulation and updates the bodies' trajectory along the way.
        /// </summary>
        public void Run()
        {
            Console.WriteLine("Running simulation... ");
            using (var progress = new ProgressBar())
            {
                double simulationTime = timeResolution;

                while (simulationTime <= timeSpan)
                {
                    timeSamples.Add(simulationTime);
                    ProceedOneStep(simulationTime == timeResolution);

                    Update(simulationTime);
                    progress.Report(simulationTime / timeSpan);
                    simulationTime += timeResolution;
                }
            }

            Console.WriteLine("\nDone. Starting plotting script...");
        }

        void ProceedOneStep(bool pIsFirstStep)
        {
            foreach (Body body in bodies)
            {
                if (!body.IsFixed)
                {
                    CalculateNextPosition(pIsFirstStep, body);
                }
            }
        }

        private void CalculateNextPosition(bool pIsFirstStep, Body pBody)
        {
            if (pBody.Mass == 0.0)
            {
                pBody.NextPosition = timeResolution * pBody.CurrentVelocity;
            }
            else
            {
                pBody.CurrentAcceleration = EvaluateNetForceOn(pBody) / pBody.Mass;
                pBody.CurrentPotentialEnergy = EvaluatePotentialOf(pBody);

                Integrator.Integrate(IntegrationType.LeapFrog, pBody, timeResolution, pIsFirstStep);
            }
        }

        void Update(double pSimulationTime)
        {
            foreach (Body body in this.bodies)
            {
                body.UpdateTrajectory(pSimulationTime);
            }
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

        double EvaluatePotentialOf(Body pBody)
        {
            double potential = 0;

            foreach (Body body in bodies)
            {
                if (body.IsMassive && body != pBody)
                {
                    potential += (-1) * body.GetForceOn(pBody).Length * Vector3.Distance(body.CurrentPosition, pBody.CurrentPosition);
                }
            }

            return potential;
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
    }
}
