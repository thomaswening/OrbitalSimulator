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
        int numberOfUnnamedBodies = 0;
        double mass = 0;

        string name;
        bool isMassive;
        bool isFixed;

        List<TrajectoryPoint> trajectory = new List<TrajectoryPoint>();

        public double Mass => mass;
        public string Name => name;
        public Vector3 CurrentVelocity { get; set; }
        public Vector3 CurrentPosition { get; set; }
        public List<TrajectoryPoint> Trajectory => trajectory;

        public bool IsMassive => isMassive;
        public bool IsFixed => isFixed;

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
                double magnitude = -PhysicalConstants.G * mass * pBody.Mass / squareDistance;
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
            trajectory.Add(new TrajectoryPoint(pTime, CurrentPosition));
        }
    }
}
