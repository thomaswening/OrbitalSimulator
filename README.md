# OrbitalSimulator
Gravity simulation for non-relativistic astromechanics

![](https://github.com/thomaswening/media/blob/main/SolarSystem.gif?raw=true)

## Decription
*OrbitalSimulator* is an n-body gravity simulation. I am building this project just out of curiosity and to practice writing C# code. 

## Features
*OrbitalSimulator* simulates the evolution of a set of gravitationally interacting bodies. Such a body can be initialised as fixed if for example its mass is very high compared to the other bodys' masses or non-fixed. It can also be initialised as non-massive if its mass is negligible compared to the other masses or as massive. Bodies can also be given a name and and must be given an initial position. If the body is massive, it must be given a mass, and if it is non-fixed, it must also be given an initial velocity. The simulation currently implements three types of integrators: a naive integrator with increasing numerical instability especially for close encounters and the numerically stable leap-frog and Verlet integrators. The number of gravitating bodies, the simulation length and time resolution are the dominant factors determining the simulation runtime. All parameters are in SI units. The application also features a Python script for creating plots and animations using Numpy and Matplotlib.

## Upcoming Features (not necessarily in this order)
- full integration of the Python plotting script 
- preset files
- comprehensive console interface 
- higher order Runge-Kutta integration
- GUI using WPF
- Save and load initial simulation configurations
- calculation of orbital elements
- initialisation of object using orbital elements instead of initial position and initial velocity
- orbital maneuvers and calculation of relevant parameters like delta-v
- realtime simulation in application window

## How to use
Since there currently is neither a GUI nor a functional console interface, all objects and parameters must be hard-coded in Program.cs. Mind you, at the moment the path for saving the data must be set appropriatly for your own machine. As soon as all objects are properly initialised and the simulations is provided with all necessary parameters, it can be compiled and run. The data files will be written to the location provided in the string variable dataPath in Program.cs. Also, for the time being all parameters for the plotting script must be set manually in the python code itself, including most importantly the path to the simulation data. It should be automatically run after each simulation. Unfortunately, there is still a bug pertaining to the streaming of console output from the Python script during its execution to the console main window. Its output files are currently saved to the same folder it is located in and it can also be executed on its own. All of the manual hard-coding of parameters and paths will be resolved in a future update including preset files, a comprehensive console interface and save/open functionality.

## Technologies
I use C# for the simulation logic and Python for the plotting logic. I will probably use WPF and the MVVM pattern for the GUI in the future and if I feel adventurous enough at some point, I might ditch C# in favour of C++ for the numerical part altogether.

## License
GNU General Public License v3.0
