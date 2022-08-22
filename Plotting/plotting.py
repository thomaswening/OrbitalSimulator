import matplotlib.pyplot as plt
import numpy as np
# from mpl_toolkits.mplot3d import Axes3D

file_path = r"E:\Users\thoma\Desktop\simulation_run.txt"

print("Extracting data...")
data = np.transpose(np.genfromtxt(file_path, dtype=np.float64, delimiter=",", skip_header=8))
print("Done.\n")

# fig = plt.figure(figsize=(10,10))
# ax = fig.add_subplot(111, projection='3d')

# plt.plot(earth[0], earth[1], earth[2])
# plt.plot(moon[0], moon[1], moon[2])

print("Plotting data...")

fig, ax = plt.subplots()

for i in range(1, len(data), 3):
    print(f"{int((i + 2) / 3)} of {int((len(data) - 1) / 3)}...")
    ax.plot(data[i], data[i + 1])

#ax.set_aspect('equal', 'box')
fig.tight_layout()

print("Done.\n")
plt.show()