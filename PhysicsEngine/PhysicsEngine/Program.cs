using System.Text;

namespace PhysicsEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Body sun = new(
                    pMass: 1.989e30F, 
                    pInitialVelocity: Vector3.Zero,
                    pInitialPosition: Vector3.Zero, 
                    pIsMassive: true, 
                    pIsFixed: true, 
                    pName: "Sun"
                );

            Body earth = new(
                    pMass: 5.972e24F,
                    pInitialVelocity : new Vector3(0, 30e3, 0),
                    pInitialPosition: new Vector3(150e9, 0, 0),
                    pIsMassive: true,
                    pIsFixed: false
                );

            double timeSpan = Convert.ToSingle(365 * 24 * Math.Pow(60, 2));
            double timeResolution = Convert.ToSingle(24 * Math.Pow(60, 2));

            Engine simulation = new(timeSpan, timeResolution, new List<Body> { sun, earth });
            simulation.Run();

            simulation.PrintToScreen();
            simulation.PrintToFile();
        }
    }
}