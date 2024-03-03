using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloOpenCvSharp
{
    public static class CvHelper
    {
        /// <summary>
        /// 将16位灰度图像的ushort数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(Mat image, string ext)
        {
            // 将Mat对象编码 ext 格式的字节流
            byte[] buf;
            // ext: ".tif"表示TIF格式；".png"表示PNG格式
            Cv2.ImEncode(ext, image, out buf);

            // 将字节流转换为Base64字符串
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// 将16位灰度图像的ushort数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(ushort[,] grayScaleData, int width, int height, string ext)
        {
            // 创建一个临时的Mat对象来存储图像数据
            using (Mat mat = new Mat(height, width, MatType.CV_16U, grayScaleData))
            {
                // 将Mat对象编码 ext 格式的字节流
                byte[] buf;
                // ext: ".tif"表示TIF格式；".png"表示PNG格式
                Cv2.ImEncode(ext, mat, out buf);

                // 将字节流转换为Base64字符串
                return Convert.ToBase64String(buf);
            }
        }

        /// <summary>
        /// 将16位灰度图像的byte数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据（byte数组）</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(byte[] grayScaleData, int width, int height, string ext)
        {
            // 确保输入的byte数组长度与16位图像数据的大小匹配
            if (grayScaleData == null || grayScaleData.Length == 0 || grayScaleData.Length != width * height * 2)
            {
                return "";
                //throw new ArgumentException("The size of the grayScaleData does not match the expected size based on width and height.");
            }

            // 使用byte数组创建一个Mat对象
            using (Mat mat = new Mat(height, width, MatType.CV_16U, grayScaleData))
            {
                // 将Mat对象编码 ext 格式的字节流
                byte[] buf;
                // ext: ".tif"表示TIF格式；".png"表示PNG格式
                Cv2.ImEncode(ext, mat, out buf);

                // 将字节流转换为Base64字符串
                return Convert.ToBase64String(buf);
            }
        }

        /// <summary>
        /// 将Base64编码的字符串转换为Mat图像对象。
        /// </summary>
        /// <param name="base64String">表示图像的Base64编码字符串。</param>
        /// <returns>表示解码图像的Mat对象。</returns>
        public static Mat ConvertBase64ToMat(string base64String)
        {
            // 将Base64字符串解码为字节数组。
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // 将字节数组解码为Mat对象。假设图像以标准格式（如PNG或TIF）编码。
            Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Unchanged);

            return image;
        }

    }

    public static class CvHelperTest
    {
        static void ConvertToBase64Tif()
        {
            // 示例：创建一个16位灰度图像的ushort数组
            int width = 640; // 图像宽度
            int height = 480; // 图像高度
            ushort[,] grayScaleData = new ushort[width, height];
            // 填充你的数据到grayScaleData中

            // 调用函数并获取结果
            string base64Tif = CvHelper.ConvertToBase64(grayScaleData, width, height, ".tif");

            // 打印或使用Base64字符串
            Console.WriteLine(base64Tif);
        }

        static void ConvertToBase64TifFromBytes()
        {
            // 示例：创建一个16位灰度图像的byte数组
            int width = 640; // 图像宽度
            int height = 480; // 图像高度
            byte[] grayScaleData = new byte[width * height * 2];
            // 填充你的数据到grayScaleData中

            // 调用函数并获取结果
            string base64Tif = CvHelper.ConvertToBase64(grayScaleData, width, height, ".tif");

            // 打印或使用Base64字符串
            Console.WriteLine(base64Tif);
        }
    }
}
