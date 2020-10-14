using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FunctionReSize
{
    public static class ReSizeImage
    {
        [FunctionName("ReSizeImage")]
        public static void Run(
            [BlobTrigger("files/{name}"), StorageAccount("AzureWebJobsStorage")]Stream image,
            [Blob("sample-images-sm/{name}", FileAccess.Write, Connection = "AzureWebJobsStorage")] Stream imageSmall,
            string name,
            ILogger log
        )
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {image.Length} Bytes");

            IImageFormat format;

            using (Image<Rgba32> input = (Image<Rgba32>)Image.Load(image, out format))
            {
                ResizeImage(input, imageSmall, ImageSize.Small, format);
            }
        }

        public static void ResizeImage(Image<Rgba32> input, Stream output, ImageSize size, IImageFormat format)
        {
            var dimensions = imageDimensionsTable[size];

            input.Mutate(x => x.Resize(dimensions.Item1, dimensions.Item2));
            input.Save(output, format);
        }

        public enum ImageSize { ExtraSmall, Small, Medium }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
            { ImageSize.ExtraSmall, (320, 200) },
            { ImageSize.Small,      (640, 400) },
            { ImageSize.Medium,     (800, 600) }
        };
    }

    public static class HelloWorld
    {
        [FunctionName("HelloWorld")]
        public static void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log
        )
        {
            log.LogInformation("Hello World");
        }
    }
}
