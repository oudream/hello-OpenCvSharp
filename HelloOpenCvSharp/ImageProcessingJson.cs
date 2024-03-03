using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloOpenCvSharp
{
    public static class ImageProcessingJson
    {
        public static void SaveImageAnnotation(List<Annotation> annotations, Mat image, string imagePath)
        {
            // 你的数据对象
            var imageData = new ImageAnnotation
            {
                Version = "5.2.1",
                Flags = new { }, // 确保这里的对象与你的实际需求匹配
                Shapes = new List<ImageShape>(),
                ImagePath = imagePath,
                ImageHeight = image.Height,
                ImageWidth = image.Width,
            };

            // annotations 添加到 Shapes中
            foreach (var annotation in annotations)
            {
                imageData.Shapes.Add(new ImageShape
                {
                    Label = annotation.Label,
                    Points = new List<List<double>> { new List<double> { annotation.X, annotation.Y }, new List<double> { annotation.X + annotation.Width, annotation.Y + annotation.Height } },
                    GroupId = null,
                    Description = "",
                    ShapeType = "rectangle",
                    Flags = new { },
                });
            }

            // 解析出 imagePath 后缀名，将其转换为Base64字符串
            string ext = Path.GetExtension(imagePath);
            string base64String = CvHelper.ConvertToBase64(image, ext);
            imageData.ImageData = base64String;

            // 保存的文件名为 imagePath ，后缀名为 .json
            string jsonFileName = Path.ChangeExtension(imagePath, ".json");

            // 将对象序列化为JSON字符串
            string jsonString = JsonConvert.SerializeObject(imageData, Formatting.Indented);

            // 保存到文件
            File.WriteAllText(jsonFileName, jsonString);
        }

        public static (List<Annotation>, Mat) LoadImageAnnotation(string jsonFilePath)
        {
            // 从文件读取JSON字符串
            string jsonString = File.ReadAllText(jsonFilePath);

            // 反序列化JSON字符串到ImageAnnotation对象
            ImageAnnotation imageAnnotation = JsonConvert.DeserializeObject<ImageAnnotation>(jsonString);

            // 根据ImageAnnotation对象中的信息重建Annotation列表
            List<Annotation> annotations = new List<Annotation>();
            foreach (var shape in imageAnnotation.Shapes)
            {
                if (shape.Points.Count >= 2) // 确保有足够的点来定义矩形
                {
                    annotations.Add(new Annotation(
                        shape.Points[0][0], // X
                        shape.Points[0][1], // Y
                        shape.Points[1][0] - shape.Points[0][0], // Width
                        shape.Points[1][1] - shape.Points[0][1] // Height
                    ));
                }
            }

            // 将Base64字符串转换回Mat图像
            Mat image = CvHelper.ConvertBase64ToMat(imageAnnotation.ImageData);

            return (annotations, image);
        }

    }

    public class ImageAnnotation
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("flags")]
        public object Flags { get; set; } // 确定具体类型或保持为object，根据你的实际情况

        [JsonProperty("shapes")]
        public List<ImageShape> Shapes { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("imageData")]
        public string ImageData { get; set; }

        [JsonProperty("imageHeight")]
        public int ImageHeight { get; set; }

        [JsonProperty("imageWidth")]
        public int ImageWidth { get; set; }
    }

    public class ImageShape
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("points")]
        public List<List<double>> Points { get; set; }

        [JsonProperty("group_id")]
        public object GroupId { get; set; } // 确定具体类型或保持为object，根据你的实际情况

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("shape_type")]
        public string ShapeType { get; set; }

        [JsonProperty("flags")]
        public object Flags { get; set; } // 确定具体类型或保持为object，根据你的实际情况
    }

}
