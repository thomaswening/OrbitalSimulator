using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalSimulator.Presets
{
    internal class BodyPreset
    {

        #region Fields

        private readonly double[] initialPosition = new double[3];
        private readonly double[] initialVelocity = new double[3];
        private double mass;

        #endregion Fields

        #region Properties

        public bool IsFixed { get; set; }
        public bool IsMassive { get; set; }
        public double Mass 
        { 
            get
            {
                if (IsMassive) return mass;
                return 0;
            }

            set { mass = value; }
        }

        public string Name { get; set; } = string.Empty;
        public double[] InitialPosition => initialPosition;
        public double[] InitialVelocity
        {
            get
            {
                if (IsFixed) return new double[] { 0, 0, 0 };
                return initialVelocity;
            }
        }

        #endregion Properties

        #region Public Methods

        public void SetInitialPosition(double x, double y, double z)
        {
            SetInitialComponents(initialPosition, x, y, z);
        }

        public void SetInitialVelocity(double x, double y, double z)
        {
            SetInitialComponents(initialVelocity, x, y, z);
        }

        #endregion Public Methods

        #region Private Methods

        static void SetInitialComponents(double[] vector, double x, double y, double z)
        {
            vector[0] = x; 
            vector[1] = y;
            vector[2] = z;
        }

        #endregion Private Methods

    }
}
