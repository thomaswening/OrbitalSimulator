using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OrbitalSimulator.Presets;

namespace OrbitalSimulator.Models
{
    internal class SimulationPreset
    {

        #region Fields

        readonly List<BodyPreset> bodies = new();

        #endregion Fields

        #region Properties

        public string Name { get; set; }

        public string IntegrationType { get; set; }

        public double TimeSpan { get; set; }

        public double TimeResolution { get; set; }

        public List<BodyPreset> Bodies => bodies;

        #endregion Properties

        #region Public Constructors

        public SimulationPreset(string name, string integrationType, double timeSpan, double timeResolution)
        { 
            Name = name;
            IntegrationType = integrationType;
            TimeSpan = timeSpan;
            TimeResolution = timeResolution;
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddBody(BodyPreset body) => bodies.Add(body);
        public void AddBodies(List<BodyPreset> bodies) => this.bodies.AddRange(bodies);

        #endregion Public Methods

    }
}
