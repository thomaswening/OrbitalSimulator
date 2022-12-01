using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalSimulator.PhysicsEngine
{
    /// <summary>
    /// Represents a body to partake in the orbital simulation
    /// </summary>
    internal class Body
    {

        #region Fields

        static int numberOfUnnamedBodies;
        static int numberOfMassiveBodies;

        readonly List<TrajectoryPoint> trajectory = new();

        bool isFixed;
        bool isMassive;
        double mass;
        int cacheId;

        #endregion Fields

        #region Properties

        public bool IsFixed => isFixed;
        public bool IsMassive => isMassive;
        public double CurrentPotentialEnergy { get; set; } = 0.0;
        public double KineticEnergy => 0.5 * mass * CurrentVelocity.LengthSquared;
        public double Mass => mass;
        public int CacheId => cacheId;
        public List<TrajectoryPoint> Trajectory => trajectory;
        public string Name { get; set; } = string.Empty;
        public Vector3 CurrentAcceleration { get; set; } = Vector3.Zero;
        public Vector3 CurrentPosition { get; set; }
        public Vector3 CurrentVelocity { get; set; }
        public Vector3 LastPosition => trajectory[^2].ToVector3();
        public Vector3 NextPosition { get; set; } = Vector3.Zero;
        static public int NumberOfMassiveBodies => numberOfMassiveBodies;

        #endregion Properties

        #region Public Constructors

        public Body(double mass, bool isMassive, bool isFixed, Vector3 initialVelocity, Vector3 initialPosition, string name)
        {
            Initialize(mass, isMassive, isFixed, initialVelocity, initialPosition);

            Name = name;
        }

        public Body(double mass, bool isMassive, bool isFixed, Vector3 initialVelocity, Vector3 initialPosition)
        {
            Initialize(mass, isMassive, isFixed, initialVelocity, initialPosition);

            numberOfUnnamedBodies++;
            Name = "Object" + numberOfUnnamedBodies;
        }

        #endregion Public Constructors

        #region Public Methods

        [MemberNotNull(nameof(CurrentVelocity), nameof(CurrentPosition))]
        public void Initialize(double mass, bool isMassive, bool isFixed, Vector3 initialVelocity, Vector3 initialPosition)
        {
            CurrentVelocity = initialVelocity;
            CurrentPosition = initialPosition;

            this.mass = mass;
            this.isMassive = isMassive;
            this.isFixed = isFixed;

            if (this.isMassive)
            {
                cacheId = numberOfMassiveBodies;
                numberOfMassiveBodies++;
            }

            trajectory.Add(new TrajectoryPoint(0, initialPosition.X, initialPosition.Y, initialPosition.Z));
        }

        public void AppendPointToTrajectory(TrajectoryPoint point) => trajectory.Add(point);

        /// <summary>
        /// Evaluates the graviational force this body exerts on another body.
        /// </summary>
        /// <param name="body">Body for which this body's gravitational attraction is to be evaluated</param>
        /// <returns>A vector representing the gravitational force exerted by this body on pBody.</returns>
        public Vector3 GetForceOn(Body body)
        {
            if (isMassive)
            {
                double squareDistance = Vector3.DistanceSquared(CurrentPosition, body.CurrentPosition);
                double magnitude = PhysicalConstants.G * mass * body.Mass / squareDistance;
                Vector3 direction = Vector3.Normalize(CurrentPosition - body.CurrentPosition);
                return magnitude * direction;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public void UpdateTrajectory(double time)
        {
            if (IsFixed)
            {
                trajectory.Add(new TrajectoryPoint(time, CurrentPosition));
            }
            else
            {
                CurrentPosition = NextPosition;
                trajectory.Add(new TrajectoryPoint(time, CurrentPosition));
            }
        }

        public void ClearTrajectory() => trajectory.RemoveRange(1, trajectory.Count - 2);

        #endregion Public Methods

    }
}
