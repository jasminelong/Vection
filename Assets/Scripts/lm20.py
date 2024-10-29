from PIL import Image
import os
import re

def get_numeric_value(filename):
    # 提取文件名中的数字部分并将其转换为浮点数
    numeric_part = re.findall(r'\d+\.?\d*', filename)
    return float(numeric_part[0]) if numeric_part else 0

def convert_jpg_to_png(jpg_path, png_path):
    """
    将JPG文件转换为PNG格式，并添加Alpha通道
    :param jpg_path: 输入的JPG文件路径
    :param png_path: 输出的PNG文件路径
    """
    img = Image.open(jpg_path)
    img = img.convert("RGBA")  # 添加Alpha通道
    img.save(png_path)

def adjust_image_alpha(image_path, alpha_factor, output_path):
    """
    调整整张图片的透明度，并保存为PNG
    :param image_path: 输入的PNG图片路径
    :param alpha_factor: 透明度因子，0到1之间
    :param output_path: 输出的PNG图片路径
    """
    image = Image.open(image_path).convert("RGBA")
    # 获取图像数据
    data = image.getdata()

    # 调整透明度
    new_data = []
    for item in data:
        # 更改Alpha通道 (保持RGB不变)
        new_data.append((item[0], item[1], item[2], int(item[3] * alpha_factor)))

    # 更新图片数据
    image.putdata(new_data)

    # 保存修改后的图片
    image.save(output_path)

def process_images(image_folder, output_folder, fps=60):
    # 获取文件夹中所有JPG图片，并按文件名中的数值进行排序
    images = sorted([os.path.join(image_folder, img) for img in os.listdir(image_folder) if img.endswith(".jpg")], key=get_numeric_value)

    # 确保输出文件夹存在
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    frame_counter = 0  # 用于保存帧编号

    # 每隔一张图片处理，例如第1张与第3张，第3张与第5张，以此类推
    for i in range(0, len(images) - 2, 3):  # 跳过2张
        image_a_path = images[i]
        image_b_path = images[i + 2]

        # 将JPG转换为PNG
        image_a_png_path = os.path.join(output_folder, f"image_a_{frame_counter:04d}.png")
        image_b_png_path = os.path.join(output_folder, f"image_b_{frame_counter:04d}.png")

        convert_jpg_to_png(image_a_path, image_a_png_path)
        convert_jpg_to_png(image_b_path, image_b_png_path)

        # 调整透明度并保存为PNG
        # 第一帧，图片a透明度为1，图片b透明度为0
        frame_counter += 1
        adjust_image_alpha(image_a_png_path, 1.0, os.path.join(output_folder, f"frame_{frame_counter:04d}.png"))
        
        frame_counter += 1
        # 第二帧，图片a透明度为0.5，图片b透明度为0.5
        blended_image_a = os.path.join(output_folder, f"temp_a_{frame_counter:04d}.png")
        blended_image_b = os.path.join(output_folder, f"temp_b_{frame_counter:04d}.png")
        adjust_image_alpha(image_a_png_path, 0.6666, blended_image_a)
        adjust_image_alpha(image_b_png_path, 0.3333, blended_image_b)

        # 叠加两张透明度调整后的图片
        image_a = Image.open(blended_image_a)
        image_b = Image.open(blended_image_b)
        blended_image = Image.alpha_composite(image_a, image_b)
        blended_image.save(os.path.join(output_folder, f"frame_{frame_counter:04d}.png"))
 
        frame_counter += 1
        # 第二帧，图片a透明度为0.5，图片b透明度为0.5
        adjust_image_alpha(image_a_png_path, 0.3333, blended_image_a)
        adjust_image_alpha(image_b_png_path, 0.6666, blended_image_b)

        # 叠加两张透明度调整后的图片
        image_a = Image.open(blended_image_a)
        image_b = Image.open(blended_image_b)
        blended_image = Image.alpha_composite(image_a, image_b)
        blended_image.save(os.path.join(output_folder, f"frame_{frame_counter:04d}.png"))


        print(f"已保存PNG帧: frame_{frame_counter - 1:04d}")

if __name__ == "__main__":
    image_folder = "../CapturedImage3"  # 输入图片文件夹路径
    output_folder = "../../../ProcessedImages/lm20"  # 输出图片文件夹路径
    fps = 60  # 每秒帧数
    process_images(image_folder, output_folder, fps)
