using System;
using System.Collections.Generic;
namespace PhysicsEngine
{
    /// <summary>
    /// Represents a point in the trajectory of a body, i.e. (time, x, y, z).
    /// </summary>
    internal struct TrajectoryPoint
    {
        double[] Components;

        public double T
        {
            get => Components[0];
            set => Components[0] = value;
        }
        public double X
        {
            get => Components[1];
            set => Components[1] = value;
        }
        public double Y
        {
            get => Components[2];
            set => Components[2] = value;
        }
        public double Z
        {
            get => Components[3];
            set => Components[3] = value;
        }

        public TrajectoryPoint(double t, double x, double y, double z)
        {
            Components = new double[4] { t, x, y, z };
        }

        public TrajectoryPoint(double t, Vector3 vector)
        {
            Components = new double[4];
            Components[0] = t;

            for (int i = 1; i < 4; i++)
            {
                Components[i] = vector.Components[i - 1];
            }
        }
    }
}
