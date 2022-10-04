using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine
{
    internal static class Integrator
    {
        public static void Integrate(IntegrationType pIntegrationType, Body pBody, double pTimeResolution, bool pIsFirstStep)
        {
            switch (pIntegrationType)
            {
                case IntegrationType.BruteForce:
                    BruteForceIntegrate(pBody, pTimeResolution);
                    break;

                case IntegrationType.LeapFrog:
                    LeapFrogIntegrate(pBody, pTimeResolution, pIsFirstStep);
                    break;

                case IntegrationType.Verlet:
                    VerletIntegrate(pBody, pTimeResolution, pIsFirstStep);
                    break;

                default:
                    throw new NotImplementedException("This integration method is not implemented!");
            }
        }

        static void BruteForceIntegrate(Body pBody, double pTimeResolution)
        {
            pBody.NextPosition = pBody.CurrentPosition +  0.5 * Math.Pow(pTimeResolution, 2) * pBody.CurrentAcceleration + pTimeResolution * pBody.CurrentVelocity;
            pBody.CurrentVelocity += pTimeResolution * pBody.CurrentAcceleration;
        }

        static void LeapFrogIntegrate(Body pBody, double pTimeResolution, bool pIsFirstStep)
        {
            // In the first step we want to calculate the next position from the current velocity,
            // which however is still simultaneous with the current position, so we use Euler for the position
            // and then calculate the next velocity half a time step ahead

            if (pIsFirstStep)
            {
                pBody.NextPosition = pBody.CurrentPosition +  0.5 * Math.Pow(pTimeResolution, 2) * pBody.CurrentAcceleration + pTimeResolution * pBody.CurrentVelocity;
                pBody.CurrentVelocity += pTimeResolution / 2.0 * pBody.CurrentAcceleration;
            }
            else
            {
                pBody.CurrentVelocity += pBody.CurrentAcceleration * pTimeResolution;
                pBody.NextPosition = pBody.CurrentPosition + pBody.CurrentVelocity * pTimeResolution;
            }
        }

        static void VerletIntegrate(Body pBody, double pTimeResolution, bool pIsFirstStep)
        {
            // Verlet integration only works with two previous positions already known,
            // so we use naive integration for the first time step

            if (pIsFirstStep)
            {
                pBody.NextPosition = pBody.CurrentPosition +  0.5 * Math.Pow(pTimeResolution, 2) * pBody.CurrentAcceleration + pTimeResolution * pBody.CurrentVelocity;
            }
            else
            {
                pBody.NextPosition = 2.0 * pBody.CurrentPosition - pBody.LastPosition + pBody.CurrentAcceleration * Math.Pow(pTimeResolution, 2);
            }
        }
    }
}
