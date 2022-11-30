from decimal import ROUND_DOWN
from math import floor
import matplotlib.pyplot as plt
from matplotlib import animation 
import numpy as np

def get_data():
    print("Extracting data...")
    data = group_data(np.transpose(np.genfromtxt(file_path, dtype=np.float64, delimiter=",", skip_header=8)))        

    print("Done.\n")
    return data


def group_data(data):
    number_of_bodies = int((len(data) - 1) / 3)
    grouped_data = np.zeros((number_of_bodies, 3, np.size(data, axis=1)))

    for i in range(1, len(data)):
        body_number = floor((i - 1) / 3)
        grouped_data[body_number, (i - 1) % 3] = data[i]

    return grouped_data

def update(t):
    progress_bar(np.round(100 * t / number_of_datapoints, 2))

    for i_body in range(len(data)):
        bodies[2 * i_body].set_data(data[i_body, 0, t], data[i_body, 1, t])
        bodies[2 * i_body + 1].set_data(data[i_body, 0, max(0 , t - trail_size):t], data[i_body, 1, max(0 , t - trail_size):t])

    return bodies

def progress_bar(progress):
    bar = "["
    for i in range(0, floor(progress / 10)):
        bar += "="

    bar += ">"

    for i in range(floor(progress / 10), 10):
        bar += " "

    bar += "]"
    print(f"\rProgress: {progress:.2f} % " + bar, end='', flush=True)

file_path = r"E:\Users\thoma\Desktop\simulation_run.txt"
data = get_data()[1:5]
print("Plotting data...")

number_of_datapoints = np.size(data, axis=2)
trail_size = floor(0.01 * number_of_datapoints)
scale = 1/1.496e11
limit = 5e12
data = scale * data

fig, ax = plt.subplots()
fig.set_size_inches(10, 10)
ax.set_aspect('equal')
ax.set_xlim(-2, 2)
ax.set_ylim(-2, 2)
ax.set_xlabel('AU')
ax.set_ylabel('AU')

bodies = []
for i_body in range(len(data)):
    body, = ax.plot(data[i_body, 0, 0], data[i_body, 1, 0], '.', markersize=10)
    trail, = ax.plot(data[i_body, 0, 0], data[i_body, 1, 0], lw=1, c='grey')
    bodies.append(body)
    bodies.append(trail)

ani = animation.FuncAnimation(fig, update, frames=number_of_datapoints, interval=1, blit=True)
ani.save('test.mp4', fps=60, extra_args=['-vcodec', 'libx264'])
plt.show()