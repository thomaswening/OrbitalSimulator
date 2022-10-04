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
        public async Task RunAsync()
        {
            Console.WriteLine("Running simulation... ");
            using (var progress = new ProgressBar())
            {
                double simulationTime = timeResolution;

                while (simulationTime <= timeSpan)
                {
                    timeSamples.Add(simulationTime);
                    await ProceedOneStepAsync(simulationTime == timeResolution);

                    await UpdateAsync(simulationTime);
                    progress.Report(simulationTime / timeSpan);
                    simulationTime += timeResolution;
                }
            }

            Console.WriteLine("\nDone. Starting plotting script...");
        }

        async Task ProceedOneStepAsync(bool pIsFirstStep)
        {
            List<Task> calculateNextPositionTasks = new();
            foreach (Body body in bodies)
            {
                calculateNextPositionTasks.Add(CalculateNextPositionAsync(pIsFirstStep, body));
            }

            await Task.WhenAll(calculateNextPositionTasks);
        }

        private async Task CalculateNextPositionAsync(bool pIsFirstStep, Body body)
        {
            if (!body.IsFixed)
            {
                if (body.Mass == 0.0)
                {
                    body.NextPosition = timeResolution * body.CurrentVelocity;
                }
                else
                {
                    Vector3 netForce;
                    Task<Vector3> netForceTask = EvaluateNetForceOnAsync(body);
                    Task<double> potentialTask = EvaluatePotentialOfAsync(body);

                    netForce = await netForceTask;
                    body.CurrentAcceleration = netForce / body.Mass;
                    body.CurrentPotentialEnergy = await potentialTask;

                    Integrator.Integrate(IntegrationType.LeapFrog, body, timeResolution, pIsFirstStep);
                }
            }
        }

        async Task UpdateAsync(double pSimulationTime)
        {
            List<Task> updateTrajectoryTask = new();

            foreach (Body body in this.bodies)
            {
                updateTrajectoryTask.Add(Task.Run(() => body.UpdateTrajectory(pSimulationTime)));
            }

            await Task.WhenAll(updateTrajectoryTask);
        }

        async Task<Vector3> EvaluateNetForceOnAsync(Body pBody)
        {
            Vector3 netForce = Vector3.Zero;
            List<Task> sumForceContributionsTasks = new();

            foreach (Body body in bodies)
            {
                if (body.IsMassive && body != pBody)
                {
                    sumForceContributionsTasks.Add(Task.Run(() => netForce += body.GetForceOn(pBody)));
                }
            }

            await Task.WhenAll(sumForceContributionsTasks);
            return netForce;
        }

        async Task<double> EvaluatePotentialOfAsync(Body pBody)
        {
            double potential = 0;
            List<Task> sumPotentialContributionsTasks = new();

            foreach (Body body in bodies)
            {
                if (body.IsMassive && body != pBody)
                {
                    sumPotentialContributionsTasks.Add(Task.Run(() => potential += (-1) * body.GetForceOn(pBody).Length * Vector3.Distance(body.CurrentPosition, pBody.CurrentPosition)));
                }
            }

            await Task.WhenAll(sumPotentialContributionsTasks);
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
