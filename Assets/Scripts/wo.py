from moviepy.editor import ImageSequenceClip
import os
import re

def get_numeric_value(filename):
    # 提取文件名中的数字部分并将其转换为浮点数
    numeric_part = re.findall(r'\d+\.?\d*', filename)
    return float(numeric_part[0]) if numeric_part else 0

def create_video_from_images(image_folder, output_video, fps, duration=60):
    # 获取文件夹中所有JPG图片，并按文件名中的数值进行排序
    images = sorted([os.path.join(image_folder, img) for img in os.listdir(image_folder) if img.endswith(".png")], key=get_numeric_value)

    # 过滤出数值大于或等于400的图片
    images = [img for img in images if get_numeric_value(img) >= 0]

    if not images:
        print("未找到符合条件的JPG图片")
        return

    # 计算间隔步长，根据fps选择间隔使用的图片
    step = 60 // fps  # 60是保存图片时的帧率（每秒60张），我们根据需要的fps来选择步长

    # 根据步长选择间隔的图片
    selected_images = images[::step]

    # 计算总帧数
    total_frames = fps * duration

    # 如果图片数量不足，将图片重复使用，确保视频长度为3分钟
    repeated_images = (selected_images * (total_frames // len(selected_images) + 1))[:total_frames]

    # 打印出使用的文件名及其序号和总数量
    print("使用的图片文件：")
    for index, img in enumerate(repeated_images, start=1):
        print(f"第 {index}/{len(repeated_images)} 张: {os.path.basename(img)}")

    # 创建视频剪辑
    clip = ImageSequenceClip(repeated_images, fps=fps)

    # 输出为视频文件
    clip.write_videofile(output_video, codec="libx264", fps=fps)

if __name__ == "__main__":
    fps_list = [60]  # 你可以调整fps列表中的值
    type = "lm"
    image_folder = "../../../ProcessedImages/lm30"  # Replace with the path to your image folder

    for fps in fps_list:
        output_video = "../Videos/" + type + "_30" + ".mp4"  # Output video filename
        # output_video = "../Videos/" + type + "_" + str(fps) + ".mp4"  # Output video filename
        create_video_from_images(image_folder, output_video, fps)
