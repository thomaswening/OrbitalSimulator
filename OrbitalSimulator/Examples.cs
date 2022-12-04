using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OrbitalSimulator.PhysicsEngine;

namespace OrbitalSimulator
{
    internal class Examples
    {
        #region Enums

        public enum Example
        {
            SolarSystem,
            EarthMoonIss
        }

        #endregion Enums

        #region Public Methods

        public static Engine InitializeExample(Example example)
        {
            List<Body> simulationBodies = new();
            double timeSpan;
            double timeResolution;

            switch (example)
            {
                case Example.EarthMoonIss:
                    AddEarthMoonIssBodies(simulationBodies);
                    timeSpan = 24 * Math.Pow(60, 2);    // 1 day
                    timeResolution = 60;                // 1 minute
                    break;

                case Example.SolarSystem:
                    AddSolarSystemBodies(simulationBodies);
                    timeSpan = 1 * 365 * 24 * Math.Pow(60, 2);   // 1 Earth year
                    timeResolution = 24 * Math.Pow(60, 2);       // 1 Earth day
                    break;

                default: throw new NotImplementedException();
            }

            return new Engine(timeSpan, timeResolution, Integrator.IntegrationType.LeapFrog, simulationBodies);
        }

        #endregion Public Methods

        #region Private Methods

        private static void AddSolarSystemBodies(List<Body> simulationBodies)
        {
            Body sun = new(
                mass: 1.989e30,
                initialVelocity: Vector3.Zero,
                initialPosition: Vector3.Zero,
                isMassive: true,
                isFixed: true,
                name: "Sun"
                );
            simulationBodies.Add(sun);

            Body mercury = new(
                mass: 3.285e23,
                initialVelocity: new Vector3(0, 58.98e3, 0),
                initialPosition: new Vector3(4.600e10, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Mercury"
                );
            simulationBodies.Add(mercury);

            Body venus = new(
                mass: 4.867e24,
                initialVelocity: new Vector3(0, 35.26e3, 0),
                initialPosition: new Vector3(1.075e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Venus"
                );
            simulationBodies.Add(venus);

            Body earth = new(
                    mass: 5.972e24,
                    initialVelocity: new Vector3(0, 30.29e3, 0),
                    initialPosition: new Vector3(1.471e11, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Earth"
                );
            simulationBodies.Add(earth);

            Body mars = new(
                mass: 6.39e23,
                initialVelocity: new Vector3(0, 26.50e3, 0),
                initialPosition: new Vector3(2.066e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Mars"
                );
            simulationBodies.Add(mars);

            Body jupiter = new(
                mass: 1.898e27,
                initialVelocity: new Vector3(0, 13.72e3, 0),
                initialPosition: new Vector3(7.407e11, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Jupiter"
                );
            simulationBodies.Add(jupiter);

            Body saturn = new(
                mass: 5.683e27,
                initialVelocity: new Vector3(0, 10.18e3, 0),
                initialPosition: new Vector3(1.349e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Saturn"
                );
            simulationBodies.Add(saturn);

            Body uranus = new(
                mass: 8.681e25,
                initialVelocity: new Vector3(0, 7.11e3, 0),
                initialPosition: new Vector3(2.736e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Uranus"
                );
            simulationBodies.Add(uranus);

            Body neptune = new(
                mass: 1.024e26,
                initialVelocity: new Vector3(0, 5.5e3, 0),
                initialPosition: new Vector3(4.459e12, 0, 0),
                isMassive: true,
                isFixed: false,
                name: "Neptune"
                );
            simulationBodies.Add(neptune);
        }

        private static void AddEarthMoonIssBodies(List<Body> simulationBodies)
        {
            double radiusEarth = 6.3781e6;
            double apogeeIss = 422e3;

            Body earth = new(
                    mass: 5.972e24,
                    initialVelocity: Vector3.Zero,
                    initialPosition: Vector3.Zero,
                    isMassive: true,
                    isFixed: true,
                    name: "Earth"
                );
            simulationBodies.Add(earth);

            Body moon = new(
                    mass: 7.348e22,
                    initialVelocity: new Vector3(0, 1.022e3, 0),
                    initialPosition: new Vector3(385e6, 0, 0),
                    isMassive: true,
                    isFixed: false,
                    name: "Moon"
                );
            simulationBodies.Add(moon);

            Body iss = new(
                    mass: 444.615e3,
                    initialVelocity: new Vector3(0, 7.6e3, 0),
                    initialPosition: new Vector3(radiusEarth + apogeeIss, 0, 0),
                    isMassive: false,
                    isFixed: false,
                    name: "ISS"
                );
            simulationBodies.Add(iss);
        }

        #endregion Private Methods
    }
}
