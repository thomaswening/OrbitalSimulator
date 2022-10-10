using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a body to partake in the orbital simulation
    /// </summary>
    internal class Body
    {
        #region Fields

        static int numberOfUnnamedBodies = 0;

        readonly bool isFixed;
        readonly bool isMassive;
        readonly double mass = 0;
        readonly string name;
        readonly List<TrajectoryPoint> trajectory = new();

        #endregion Fields

        #region Properties

        public Vector3 CurrentAcceleration { get; set; } = Vector3.Zero;
        public Vector3 CurrentPosition { get; set; }
        public double CurrentPotentialEnergy { get; internal set; } = 0.0;
        public Vector3 CurrentVelocity { get; set; }
        public bool IsFixed => isFixed;
        public bool IsMassive => isMassive;
        public double KineticEnergy => 0.5 * mass * CurrentVelocity.LengthSquared;
        public Vector3 LastPosition => trajectory[^2].ToVector3();
        public double Mass => mass;
        public string Name => name;
        public Vector3 NextPosition { get; set; } = Vector3.Zero;
        public List<TrajectoryPoint> Trajectory => trajectory;

        #endregion Properties

        #region Constructors

        public Body(double pMass, Vector3 pInitialVelocity, Vector3 pInitialPosition, bool pIsMassive, bool pIsFixed, string pName)
        {
            CurrentVelocity = pInitialVelocity;

            CurrentPosition = pInitialPosition;

            mass = pMass;
            isMassive = pIsMassive;
            isFixed = pIsFixed;
            name = pName;

            trajectory.Add(new TrajectoryPoint(0, pInitialPosition.X, pInitialPosition.Y, pInitialPosition.Z));
        }

        public Body(double pMass, Vector3 pInitialVelocity, Vector3 pInitialPosition, bool pIsMassive, bool pIsFixed)
        {
            numberOfUnnamedBodies++;
            CurrentVelocity = pInitialVelocity;

            CurrentPosition = pInitialPosition;

            mass = pMass;
            isMassive = pIsMassive;
            isFixed = pIsFixed;
            name = "Object" + numberOfUnnamedBodies;

            trajectory.Add(new TrajectoryPoint(0, pInitialPosition.X, pInitialPosition.Z, pInitialPosition.Z));
        }

        #endregion Constructors

        #region Methods

        public void AppendPointToTrajectory(TrajectoryPoint pPoint) => trajectory.Add(pPoint);
        /// <summary>
        /// Evaluates the graviational force this body exerts on another body.
        /// </summary>
        /// <param name="pBody">Body for which this body's gravitational attraction is to be evaluated</param>
        /// <returns>A vector representing the gravitational force exerted by this body on pBody.</returns>
        public Vector3 GetForceOn(Body pBody)
        {
            if (isMassive)
            {
                double squareDistance = Vector3.DistanceSquared(CurrentPosition, pBody.CurrentPosition);
                double magnitude = (-1) * PhysicalConstants.G * mass * pBody.Mass / squareDistance;
                Vector3 direction = (-1) * Vector3.Normalize(CurrentPosition - pBody.CurrentPosition);
                return magnitude * direction;
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public void UpdateTrajectory(double pTime)
        {
            if (IsFixed)
            {
                trajectory.Add(new TrajectoryPoint(pTime, CurrentPosition));
            }
            else
            {
                CurrentPosition = NextPosition;
                trajectory.Add(new TrajectoryPoint(pTime, CurrentPosition));
            }
        }

        public void ClearTrajectory() => trajectory.RemoveRange(1, trajectory.Count - 2);

        #endregion Methods
    }
}
