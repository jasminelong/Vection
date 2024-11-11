import pandas as pd
import matplotlib.pyplot as plt

# File paths for six experiments (请替换为你的实际文件路径)
file_paths = [
    'D:/unity/Vection/Assets/ExperimentData/20241105_180723_continuous_cameraSpeed4_fps60_I_trialNumber1.csv',
    'D:/unity/Vection/Assets/ExperimentData/20241105_181512_continuous_cameraSpeed4_fps60_I_trialNumber2.csv',
    'D:/unity/Vection/Assets/ExperimentData/20241105_182431_continuous_cameraSpeed4_fps60_I_trialNumber3.csv',
    'D:/unity/Vection/Assets/ExperimentData/20241106_151409_continuous_cameraSpeed4_fps60_O_trialNumber1.csv',
    'D:/unity/Vection/Assets/ExperimentData/20241106_152000_continuous_cameraSpeed4_fps60_O_trialNumber2.csv',
    'D:/unity/Vection/Assets/ExperimentData/20241106_152448_continuous_cameraSpeed4_fps60_O_trialNumber3.csv'
]

# Loop through each file and create a plot for each experiment
for i, file_path in enumerate(file_paths):
    # Load the CSV file into a DataFrame
    df = pd.read_csv(file_path)

    # Extract 'Time' and 'Vection Response' columns
    time = df.iloc[:, 1] / 1000  # 第二列作为横轴 (s)
    vection_response = df.iloc[:, 2]  # 第三列作为纵轴

    # Create a separate line for time comparison (from 0 to 180 seconds, set to 1)
    time_comparison = [(1 if 0 <= t <= 180 else 0) for t in time]

    # Create a new figure with two subplots vertically
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(10, 12), sharex=True)  # 交换 ax1 和 ax2 的位置

    # Plotting the time comparison line chart in the first subplot (now on top)
    ax1.plot(time, time_comparison, marker='', linestyle='-', color='r', label='Time Comparison (0-180s)')
    ax1.fill_between(time, time_comparison, alpha=0.3, color='r')
    ax1.set_ylabel('Time Comparison (0 or 1)')
    ax1.set_ylim(0, 1.1)
    ax1.set_yticks([0, 1])
    ax1.legend()
    ax1.grid()

    # Plotting the vection response line chart in the second subplot (now on bottom)
    ax2.plot(time, vection_response, marker='', linestyle='-', color='b', label='Vection Response')
    ax2.fill_between(time, vection_response, alpha=0.3, color='b')
    ax2.set_xlabel('Time (s)')
    ax2.set_ylabel('Vection Response (0 or 1)')
    ax2.set_ylim(0, 1.1)
    ax2.set_yticks([0, 1])
    ax2.set_title(f'Vection Response vs Time (Experiment {i+1})')
    ax2.legend()
    ax2.grid()

    # Set x-axis limit to include the full range of time values
    ax2.set_xlim(left=min(time) - 5, right=max(time) + 5)

    # Annotate the position at 0 and 180 seconds on the x-axis for both subplots
    for ax in [ax1, ax2]:
        ax.axvline(x=0, color='purple', linestyle='--')
        ax.axvline(x=180, color='purple', linestyle='--')
        ax.text(0, 1.05, '0 s', color='purple', fontsize=9, horizontalalignment='center')
        ax.text(180, 1.05, '180 s', color='purple', fontsize=9, horizontalalignment='center')

    # Draw dashed lines connecting the two subplots at 0 and 180 seconds using x-axis values
    fig.lines.extend([plt.Line2D([ax2.get_position().x0, ax2.get_position().x0], [0.1, 0.9], transform=fig.transFigure, color='purple', linestyle='--'),
                      plt.Line2D([ax2.get_position().x1, ax2.get_position().x1], [0.1, 0.9], transform=fig.transFigure, color='purple', linestyle='--')])

    # Annotate the first and last values in the Time column on the x-axis
    first_time_value = min(time)
    last_time_value = max(time)
    ax2.text(first_time_value, -0.1, f'{first_time_value:.2f} s', color='black', fontsize=9, horizontalalignment='center')
    ax2.text(last_time_value, -0.1, f'{last_time_value:.2f} s', color='black', fontsize=9, horizontalalignment='center')

    # Annotate the first occurrence of Vection Response equal to 1 on the plot
    if any(vection_response == 1):
        first_occurrence_index = vection_response[vection_response == 1].index[0]
        first_occurrence_time = time[first_occurrence_index]
        ax2.axvline(x=first_occurrence_time, color='r', linestyle='--', label=f'First Response=1 at {first_occurrence_time:.2f} s')
        ax2.text(first_occurrence_time, 1.05, f'{first_occurrence_time:.2f} s', color='r', fontsize=9, horizontalalignment='center')

    # Calculate the total time when the 'Vection Response' is equal to 1
    time_diff = time.diff().fillna(0)
    time_intervals = time_diff[vection_response == 1]
    total_duration_1 = time_intervals.sum()

    # Annotate the total time when Vection Response is 1 on the plot
    ax2.text(0.95, 0.8, f'Total Time (Response=1): {total_duration_1:.2f} s',
             horizontalalignment='right',
             verticalalignment='top',
             transform=ax2.transAxes,
             fontsize=10,
             bbox=dict(facecolor='white', alpha=0.5))

    # Add a shared title for the entire figure
    plt.suptitle(f'Comparison of Time and Vection Response for Experiment {i+1}', fontsize=16)

    # Save the plot as a PNG image
    plt.savefig(f'Experiment_{i+1}_Vection_Response_Comparison_Plot.png', dpi=300)

    # Display the plot
    plt.show()
