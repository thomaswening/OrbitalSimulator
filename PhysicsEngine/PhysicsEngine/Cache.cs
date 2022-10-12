using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    internal class ForceCache
    {
        readonly int dimension;
        Vector3?[,] grid;

        public ForceCache(int pDimension)
        {
            dimension = pDimension;
            grid = new Vector3?[pDimension, pDimension];
        }

        public Vector3? Fetch(int i, int j)
        {
            if (i == j) throw new Exception("Diagonal elements cannot ever be fetched because they are undefined.");

            if (j > i) return grid[j, i];

            if (grid[i, j] is null) return null;

            return grid[i, j];
        }

        public void Clear() => grid = new Vector3?[dimension, dimension];
        
        public void Post(int i, int j, Vector3 pValue)
        {
            if (i == j) throw new Exception("Diagonal elements must remain undefined.");

            if (grid[i, j] is not null) throw new Exception("Cache element is already initialized.");

            if (j > i) Post(j, i, pValue);            

            grid[i, j] = pValue;
        }
    }
}
