using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OrbitalSimulator.Utilities;

using PhysicsEngine;

using static OrbitalSimulator.PhysicsEngine.Integrator;

namespace OrbitalSimulator.PhysicsEngine
{
    /// <summary>
    /// Evaluates the physics between all bodies and calculates the bodies' trajectories.
    /// </summary>
    internal class Engine
    {
        #region Fields

        readonly List<Body> bodies = new();
        readonly double timeSpan;
        readonly double timeResolution;
        readonly List<double> timeSamples = new() { 0 };
        ForceCache? forceCache;

        #endregion Fields

        #region Public Constructors

        public Engine(double timeSpan, double timeResolution, IntegrationType integrationType)
        {
            if (timeSpan < 0) throw new ArgumentException("The time span must be greater than zero.", nameof(timeSpan));

            if (timeSpan < timeResolution) throw new ArgumentException("The time span must be greater than the time resolution.", nameof(timeResolution));

            this.timeSpan = timeSpan;
            this.timeResolution = timeResolution;
        }

        public Engine(double timeSpan, double timeResolution, IntegrationType integrationType, List<Body> bodies) : this(timeSpan, timeResolution, integrationType)
        {
            this.bodies.AddRange(bodies);
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddBody(Body body) => bodies.Add(body);

        /// <summary>
        /// Runs the simulation and updates the bodies' trajectory along the way.
        /// </summary>
        public void Run()
        {
            forceCache = new ForceCache(Body.NumberOfMassiveBodies);

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

            Console.WriteLine("\nDone.");
        }

        public string ToString(string delimiter = ",")
        {
            StringBuilder sb = new();

            // Header

            sb.Append($"Simulation Run Results\n\n");
            sb.Append($"time span (s): {timeSpan}\n");
            sb.Append($"time resolution (s): {timeResolution}\n");
            sb.Append($"=> number of time samples: {timeSamples.Count}\n\n");

            sb.Append($"time (s)" + delimiter);
            foreach (Body body in bodies)
            {
                sb.Append($"{body.Name} X (km)" + delimiter);
                sb.Append($"{body.Name} Y (km)" + delimiter);
                sb.Append($"{body.Name} Z (km)" + delimiter);
            }

            EndLine(sb, delimiter);
            sb.Append('\n');

            // Body

            for (int i = 0; i < timeSamples.Count; i++)
            {
                sb.Append(timeSamples[i] + delimiter);
                foreach (Body body in bodies)
                {
                    sb.Append(body.Trajectory[i].X + delimiter);
                    sb.Append(body.Trajectory[i].Y + delimiter);
                    sb.Append(body.Trajectory[i].Z + delimiter);
                }

                EndLine(sb, delimiter);
            }

            return sb.ToString();
        }

        public override string ToString() => ToString();

        public void PrintToScreen() => Console.WriteLine(ToString());

        public void PrintToFile(string path)
        {
            using StreamWriter file = new(path);
            file.WriteLine(ToString());
        }

        #endregion Public Methods

        #region Private Methods

        static void EndLine(StringBuilder stringBuilder, string delimiter)
        {
            stringBuilder.Remove(stringBuilder.Length - delimiter.Length, delimiter.Length);
            stringBuilder.Append('\n');
        }

        void ProceedOneStep(bool isFirstStep)
        {
            forceCache!.Refresh(bodies);

            foreach (Body body in bodies)
            {
                if (!body.IsFixed) CalculateNextPosition(isFirstStep, body);
                body.CurrentPotentialEnergy = body.IsMassive ? EvaluatePotentialOf(body) : 0;
            }
        }

        private void CalculateNextPosition(bool isFirstStep, Body body)
        {
            if (body.Mass == 0.0)
            {
                body.NextPosition = timeResolution * body.CurrentVelocity;
            }
            else
            {
                body.CurrentAcceleration = EvaluateNetForceOn(body) / body.Mass;

                Integrator.Integrate(IntegrationType.LeapFrog, body, timeResolution, isFirstStep);
            }
        }

        void Update(double simulationTime)
        {
            foreach (Body body in bodies)
            {
                body.UpdateTrajectory(simulationTime);
            }
        }

        Vector3 EvaluateNetForceOn(Body body)
        {
            Vector3 netForce = Vector3.Zero;

            foreach (Body body2 in bodies.Where(x => x.IsMassive))
            {
                if (body2 != body) netForce += forceCache!.Fetch(body, body2);
            }

            return netForce;
        }

        double EvaluatePotentialOf(Body body)
        {
            double potential = 0;

            if (forceCache is null) throw new Exception($"{nameof(forceCache)} has not been initialized.");

            foreach (Body body2 in bodies.Where(x => x.IsMassive))
            {
                if (body2 != body)
                {
                    Vector3 cachedForce = forceCache.Fetch(body, body2);
                    potential += -1 * cachedForce.Length * Vector3.Distance(body2.CurrentPosition, body.CurrentPosition);
                }
            }

            return potential;
        }

        #endregion Private Methods
    }
}
