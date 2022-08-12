import matplotlib.pyplot as plt
import numpy as np

from mpl_toolkits.mplot3d import Axes3D

filename = r"E:\Users\thoma\Desktop\simulation_run.txt"
data = np.transpose(np.genfromtxt(filename, dtype=np.float64, delimiter=",", skip_header=8))

time = data[0]
sun = data[1:4]
earth = data[4:7]
moon = data[7:10]
iss = data[10:13]

# fig = plt.figure(figsize=(10,10))
# ax = fig.add_subplot(111, projection='3d')

# plt.plot(earth[0], earth[1], earth[2])
# plt.plot(moon[0], moon[1], moon[2])

plt.plot(earth[0], earth[1])
plt.plot(moon[0], moon[1])
plt.plot(iss[0], iss[1])

plt.show()