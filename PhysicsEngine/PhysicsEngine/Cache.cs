using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    /// <summary>
    /// Stores the gravitational forces the bodies each extert on the other bodies in the simulation.
    /// They are stored in a upper triangular matrix of Vector3 objects to save memory.
    /// </summary>
    internal class ForceCache
    {

        #region Fields

        readonly int dimension;
        Vector3?[,] grid;

        #endregion Fields

        #region Public Constructors

        public ForceCache(int pDimension)
        {
            dimension = pDimension;
            grid = new Vector3?[pDimension, pDimension];
        }

        #endregion Public Constructors

        #region Public Methods

        public void Refresh(List<Body> bodies)
        {
            Clear();

            foreach (Body body1 in bodies)
            {
                foreach (Body body2 in bodies.Where(x => x.CacheId < body1.CacheId))
                {
                    Post(body1, body2);
                }
            }
        }

        public Vector3 Fetch(Body body1, Body body2) => Fetch(body1.CacheId, body2.CacheId);

        public void Post(Body body1, Body body2) => Post(body1.CacheId, body2.CacheId, body2.GetForceOn(body1));

        #endregion Public Methods

        #region Private Methods

        Vector3 Fetch(int i, int j)
        {
            if (i == j) throw new ArgumentException("Diagonal elements cannot ever be fetched because they are undefined.", nameof(j));

            if (i < j) return (-1) * Fetch(j, i); // prefactor comes from antisymmetry of force matrix

            if (grid[i, j] is null) throw new Exception("Cache has not been initialized.");

            return grid[i, j]!;
        }
        void Post(int i, int j, Vector3 pValue)
        {
            if (i == j) throw new Exception("Diagonal elements must remain undefined.");

            if (grid[i, j] is not null) throw new Exception("Cache element is already initialized.");

            if (j > i) throw new ArgumentException("Force cache must be upper triangular matrix.", nameof(j));

            grid[i, j] = pValue;
        }

        void Clear() => grid = new Vector3?[dimension, dimension];

        #endregion Private Methods

    }
}
