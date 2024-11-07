import pandas as pd
import matplotlib.pyplot as plt

# File paths for six experiments (please replace with your actual file paths)
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

    # Calculate the total time when the 'Vection Response' is equal to 1
    time_diff = time.diff().fillna(0)
    time_intervals = time_diff[vection_response == 1]
    total_duration_1 = time_intervals.sum()

    # Find the first occurrence of Vection Response equal to 1
    first_occurrence_index = vection_response[vection_response == 1].index[0]
    first_occurrence_time = time[first_occurrence_index]

    # Find the first negative value in the 'Time' column and its index
    if any(time < 0):
        first_negative_index = time[time < 0].index[0]
        first_negative_value = time[first_negative_index]
    else:
        first_negative_value = None

    # Find the last value in the 'Time' column
    last_time_value = time.iloc[-1]

    # Plotting the line chart
    plt.figure(figsize=(10, 6))
    plt.plot(time, vection_response, marker='', linestyle='-', color='b', label='Vection Response')
    plt.fill_between(time, vection_response, alpha=0.3, color='b')

    # Adding labels and title
    plt.xlabel('Time (s)')
    plt.ylabel('Vection Response')
    plt.title(f'Vection Response vs Time (Experiment {i+1})')
    plt.legend()

    # Set y-axis to display only 0 and 1
    plt.yticks([0, 1])

    # Set x-axis limit to include negative values
    plt.xlim(left=min(time) - 5, right=max(time) + 5)

    # Annotate the total time when Vection Response is 1 on the plot
    plt.text(0.95, 0.8, f'Total Time (Response=1): {total_duration_1:.2f} s',
             horizontalalignment='right',
             verticalalignment='top',
             transform=plt.gca().transAxes,
             fontsize=10,
             bbox=dict(facecolor='white', alpha=0.5))

    # Annotate the first occurrence of Vection Response equal to 1 on the plot
    plt.axvline(x=first_occurrence_time, color='r', linestyle='--', label=f'First Response=1 at {first_occurrence_time:.2f} s')
    plt.text(first_occurrence_time, -0.1, f'{first_occurrence_time:.2f} s', color='r', fontsize=9, horizontalalignment='center')

    # Annotate the first negative value in the Time column on the x-axis
    if first_negative_value is not None:
        plt.axvline(x=first_negative_value, color='green', linestyle=':', label=f'First Negative Time at {first_negative_value:.2f} s')
        plt.text(first_negative_value, -0.1, f'{first_negative_value:.2f} s', color='green', fontsize=9, horizontalalignment='center')

    # Annotate the last value in the Time column on the x-axis
    plt.axvline(x=last_time_value, color='orange', linestyle=':', label=f'Last Time at {last_time_value:.2f} s')
    plt.text(last_time_value, -0.1, f'{last_time_value:.2f} s', color='orange', fontsize=9, horizontalalignment='center')

    # Annotate the position at 180 seconds on the x-axis
    plt.axvline(x=180, color='purple', linestyle='--', label='Time at 180 s')
    plt.text(180, -0.1, '180 s', color='purple', fontsize=9, horizontalalignment='center')

    # Adding grid and legend
    plt.grid()
    plt.legend()

    # Save the plot as a PNG image
    plt.savefig(f'Experiment_{i+1}_Vection_Response_Plot.png', dpi=300)

    # Display the plot
    plt.show()
