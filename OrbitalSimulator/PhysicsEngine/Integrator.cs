using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbitalSimulator.PhysicsEngine
{
    internal static class Integrator
    {
        public enum IntegrationType
        {
            BruteForce,
            LeapFrog,
            Verlet
        }

        public static IntegrationType GetIntegrationType(string input)
        {
            return input.ToLower() switch
            {
                "bruteforce" => IntegrationType.BruteForce,
                "leapfrog" => IntegrationType.LeapFrog,
                "verlet" => IntegrationType.Verlet,

                _ => throw new NotImplementedException()
            };
        }

        public static string GetIntegrationTypeStr(IntegrationType integrationType)
        {
            return integrationType switch
            {
                IntegrationType.BruteForce => "bruteforce",
                IntegrationType.LeapFrog => "leapfrog",
                IntegrationType.Verlet => "verlet",

                _ => throw new NotImplementedException()
            };
        }

        #region Public Methods

        public static void Integrate(IntegrationType integrationType, Body body, double timeResolution, bool isFirstStep)
        {
            switch (integrationType)
            {
                case IntegrationType.BruteForce:
                    BruteForceIntegrate(body, timeResolution);
                    break;

                case IntegrationType.LeapFrog:
                    LeapFrogIntegrate(body, timeResolution, isFirstStep);
                    break;

                case IntegrationType.Verlet:
                    VerletIntegrate(body, timeResolution, isFirstStep);
                    break;

                default:
                    throw new NotImplementedException("This integration method is not implemented!");
            }
        }

        #endregion Public Methods

        #region Private Methods

        static void BruteForceIntegrate(Body body, double timeResolution)
        {
            body.NextPosition = body.CurrentPosition + 0.5 * Math.Pow(timeResolution, 2) * body.CurrentAcceleration + timeResolution * body.CurrentVelocity;
            body.CurrentVelocity += timeResolution * body.CurrentAcceleration;
        }

        static void LeapFrogIntegrate(Body body, double timeResolution, bool isFirstStep)
        {
            // In the first step we want to calculate the next position from the current velocity,
            // which however is still simultaneous with the current position, so we use Euler for the position
            // and then calculate the next velocity half a time step ahead

            if (isFirstStep)
            {
                body.NextPosition = body.CurrentPosition + 0.5 * Math.Pow(timeResolution, 2) * body.CurrentAcceleration + timeResolution * body.CurrentVelocity;
                body.CurrentVelocity += timeResolution / 2.0 * body.CurrentAcceleration;
            }
            else
            {
                body.CurrentVelocity += body.CurrentAcceleration * timeResolution;
                body.NextPosition = body.CurrentPosition + body.CurrentVelocity * timeResolution;
            }
        }

        static void VerletIntegrate(Body body, double timeResolution, bool isFirstStep)
        {
            // Verlet integration only works with two previous positions already known,
            // so we use naive integration for the first time step

            if (isFirstStep)
            {
                body.NextPosition = body.CurrentPosition + 0.5 * Math.Pow(timeResolution, 2) * body.CurrentAcceleration + timeResolution * body.CurrentVelocity;
            }
            else
            {
                body.NextPosition = 2.0 * body.CurrentPosition - body.LastPosition + body.CurrentAcceleration * Math.Pow(timeResolution, 2);
            }
        }

        #endregion Private Methods
    }
}
