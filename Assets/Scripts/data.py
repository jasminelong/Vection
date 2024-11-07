import pandas as pd
import matplotlib.pyplot as plt

# Load the CSV file into a DataFrame
file_path = 'D:/unity/Vection/Assets/ExperimentData/20241105_180723_continuous_cameraSpeed4_fps60_I_trialNumber1.csv'  # 请替换为你的实际文件路径
df = pd.read_csv(file_path)
print(df);
# Extract 'Time' and 'Vection Response' columns
time = df.iloc[:, 1]/1000  # 第二列作为横轴
vection_response = df.iloc[:, 2]  # 第三列作为纵轴

# Calculate the total time when the 'Vection Response' is equal to 1
# Calculate the time differences between consecutive frames
time_diff = time.diff().fillna(0)
# Get the time differences only when Vection Response is 1
time_intervals = time_diff[vection_response == 1]

# Calculate the total duration
total_duration_1 = time_intervals.sum()

# Find the first occurrence of Vection Response equal to 1
first_occurrence_index = vection_response[vection_response == 1].index[0]
first_occurrence_time = time[first_occurrence_index]

# Plotting the line chart
plt.figure(figsize=(10, 6))
#plt.scatter(time, vection_response, color='b', label='Vection Response')
plt.plot(time, vection_response, marker='', linestyle='-', color='b', label='Vection Response')
plt.fill_between(time, vection_response,alpha=0.3, color='b');
# Adding labels and title
plt.xlabel('Time (s)')
plt.ylabel('Vection Response')
plt.title('Vection Response vs Time')
plt.legend()
# Set y-axis to display only 0 and 1
plt.yticks([0, 1])

# Annotate the total time when Vection Response is 1 on the plot
plt.text(0.95, 0.8, f'Total Time (Response=1): {total_duration_1:.2f} s',
         horizontalalignment='right',
         verticalalignment='top',
         transform=plt.gca().transAxes,
         fontsize=10,
         bbox=dict(facecolor='white', alpha=0.5))

# Annotate the first occurrence of Vection Response equal to 1 on the plot
plt.axvline(x=first_occurrence_time, color='r', linestyle='--', label=f'First Response=1 at {first_occurrence_time:.2f} s')
plt.legend()

# Display the plot
plt.grid()
plt.show()
