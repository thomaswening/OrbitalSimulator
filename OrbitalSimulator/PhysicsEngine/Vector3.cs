using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalSimulator.PhysicsEngine
{
    /// <summary>
    /// Represents a three-dimensional, Euclidean vector
    /// </summary>
    internal class Vector3
    {
        #region Properties

        public static Vector3 Zero => new(0, 0, 0);
        public double[] Components { get; set; }

        public double X
        {
            get => Components[0];
            set => Components[0] = value;
        }
        public double Y
        {
            get => Components[1];
            set => Components[1] = value;
        }
        public double Z
        {
            get => Components[2];
            set => Components[2] = value;
        }

        public double LengthSquared
        {
            get
            {
                double lengthSquared = 0;
                foreach (double component in Components)
                {
                    lengthSquared += Math.Pow(component, 2);
                }

                return lengthSquared;
            }
        }

        public double Length => Math.Sqrt(LengthSquared);

        #endregion Properties

        #region Public Constructors

        public Vector3(double x, double y, double z)
        {
            Components = new double[3] { x, y, z };
        }

        public Vector3(double[] components)
        {
            if (components.Length != 3) throw new ArgumentException("Must have length == 3.", nameof(components));

            Components = components;
        }

        #endregion Public Constructors

        #region Public Methods

        public static Vector3 Normalize(Vector3 vector)
        {
            double[] normalizedComponents = new double[3];
            for (int i = 0; i < 3; i++)
            {
                normalizedComponents[i] = vector.Components[i] / vector.Length;
            }

            return new Vector3(normalizedComponents[0], normalizedComponents[1], normalizedComponents[2]);
        }

        public static Vector3 Add(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3 Subtract(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static double Multiply(Vector3 left, Vector3 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vector3 Multiply(double scalar, Vector3 vector)
        {
            return new Vector3(scalar * vector.X, scalar * vector.Y, scalar * vector.Z);
        }

        public static Vector3 Divide(Vector3 vector, double scalar)
        {
            return new Vector3(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
        }

        public static double DistanceSquared(Vector3 left, Vector3 right)
        {
            Vector3 difference = Subtract(left, right);
            return difference.LengthSquared;
        }

        public static double Distance(Vector3 left, Vector3 right) => Math.Sqrt(DistanceSquared(left, right));

        public static Vector3 operator +(Vector3 left, Vector3 right) => Add(left, right);

        public static Vector3 operator -(Vector3 left, Vector3 right) => Subtract(left, right);

        public static double operator *(Vector3 left, Vector3 right) => Multiply(left, right);

        public static Vector3 operator *(double scalar, Vector3 vector) => Multiply(scalar, vector);

        public static Vector3 operator *(Vector3 vector, double scalar) => Multiply(scalar, vector);

        public static Vector3 operator /(Vector3 vector, double scalar) => Divide(vector, scalar);

        public void Normalize()
        {
            for (int i = 0; i < 3; i++)
            {
                Components[i] /= Length;
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new("<");
            sb.Append(X + ", ");
            sb.Append(Y + ", ");
            sb.Append(Z + ">");

            return sb.ToString();
        }

        #endregion Public Methods
    }
}
