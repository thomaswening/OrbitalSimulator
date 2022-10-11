using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        static int numberOfUnnamedBodies;
        static int numberOfBodies;
        static int numberOfMassiveBodies;

        readonly List<TrajectoryPoint> trajectory = new();

        int cacheId;
        bool isFixed;
        bool isMassive;
        double mass;
        string name;        

        #endregion Fields

        #region Properties
        static public int NumberOfMassiveBodies => numberOfMassiveBodies;
        public int CacheId => cacheId;        
        public Vector3 CurrentAcceleration { get; set; } = Vector3.Zero;
        public Vector3 CurrentPosition { get; set; }
        public double CurrentPotentialEnergy { get; set; } = 0.0;
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
            Initialize(pMass, pInitialVelocity, pInitialPosition, pIsMassive, pIsFixed);

            name = pName;
        }

        public Body(double pMass, Vector3 pInitialVelocity, Vector3 pInitialPosition, bool pIsMassive, bool pIsFixed)
        {
            Initialize(pMass, pInitialVelocity, pInitialPosition, pIsMassive, pIsFixed);

            numberOfUnnamedBodies++;
            name = "Object" + numberOfUnnamedBodies;
        }

        #endregion Constructors

        #region Methods

        [MemberNotNull(nameof(CurrentVelocity), nameof(CurrentPosition))]
        public void Initialize(double pMass, Vector3 pInitialVelocity, Vector3 pInitialPosition, bool pIsMassive, bool pIsFixed)
        {
            numberOfBodies++;
            CurrentVelocity = pInitialVelocity;
            CurrentPosition = pInitialPosition;

            mass = pMass;
            isMassive = pIsMassive;
            isFixed = pIsFixed;

            if (isMassive)
            {
                cacheId = numberOfMassiveBodies;
                numberOfMassiveBodies++;
            }

            trajectory.Add(new TrajectoryPoint(0, pInitialPosition.X, pInitialPosition.Y, pInitialPosition.Z));
        }

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
                double magnitude = PhysicalConstants.G * mass * pBody.Mass / squareDistance;
                Vector3 direction = Vector3.Normalize(CurrentPosition - pBody.CurrentPosition);
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
