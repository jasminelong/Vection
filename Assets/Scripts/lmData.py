import pandas as pd
import matplotlib.pyplot as plt

# Load the CSV file into a DataFrame
file_path = 'D:/unity/Vection/Assets/ExperimentData/20241113_165322_luminanceMixture_cameraSpeed4_fps30_G_trialNumber3.csv'  # 请替换为你的实际文件路径
df = pd.read_csv(file_path)

# Extract data from the DataFrame
time = df['Time'] / 1000  # 将时间列作为横轴 (秒)，除以1000将ms转换为s
vection_response = df['Vection Response']

# Extract luminance values based on FrondFrameNum and BackFrameNum
frond_frame_num = df['FrondFrameNum']
back_frame_num = df['BackFrameNum']
frond_frame_luminance = df['FrondFrameLuminance']
back_frame_luminance = df['BackFrameLuminance']

# Replace BackFrameNum odd values with corresponding BackFrameLuminance and FrondFrameNum even values
for i in range(len(frond_frame_num)):
    if back_frame_num[i] % 2 != 0:
        temp_back_frame_num = back_frame_num[i]
        temp_back_frame_luminance = back_frame_luminance[i]
        back_frame_num[i] = frond_frame_num[i]
        back_frame_luminance[i] = frond_frame_luminance[i]
        frond_frame_num[i] = temp_back_frame_num
        frond_frame_luminance[i] = temp_back_frame_luminance

# Create the figure and axes for plotting
fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(12, 10), sharex=True, gridspec_kw={'hspace': 0.3})

# Plot Frond Frame Luminance and Back Frame Luminance on the first subplot
ax1.plot(time, frond_frame_luminance, linestyle='-', color='b', label='Frond Frame Luminance', alpha=0.5)
ax1.plot(time, back_frame_luminance, linestyle='-', color='g', label='Back Frame Luminance', alpha=0.5)

# Set labels for luminance
ax1.set_ylabel('Luminance Value (0-1)')
ax1.set_title('Luminance Value vs Time')
ax1.legend(loc='upper right')
ax1.grid()
# Limit x-axis to 10 seconds
ax1.set_xlim([20, 25])
 
# Plot Vection Response on the second subplot
ax2.plot(time, vection_response, linestyle='-', color='r', label='Vection Response', alpha=0.7)
ax2.fill_between(time, vection_response, alpha=0.3, color='r')
ax2.set_xlabel('Time (s)')
ax2.set_ylabel('Vection Response (0-1)')
ax2.set_title('Vection Response vs Time')
ax2.set_yticks([0, 1])
ax2.legend(loc='upper right')
ax2.grid()

# Limit x-axis to 10 seconds
ax2.set_xlim([20, 25])

# Annotate important points
# Annotate the initial and final time points
ax2.text(time.min(), -0.1, f'{time.min():.2f} s', color='black', fontsize=9, horizontalalignment='center')
ax2.text(time.max(), -0.1, f'{time.max():.2f} s', color='black', fontsize=9, horizontalalignment='center')

# Annotate 180s mark
ax2.axvline(x=180, color='purple', linestyle='--', label='Time at 180 s')
ax2.text(180, -0.1, '180 s', color='purple', fontsize=9, horizontalalignment='center')

# Draw vertical dashed lines connecting the two subplots at specific points (e.g., 0s and 180s)
fig.lines.extend([plt.Line2D([0.125, 0.125], [0.1, 0.9], transform=fig.transFigure, color='purple', linestyle='--'),
                  plt.Line2D([0.9, 0.9], [0.1, 0.9], transform=fig.transFigure, color='purple', linestyle='--')])
# Display the plot
plt.show()
