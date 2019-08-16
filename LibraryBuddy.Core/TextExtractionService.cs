using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class TextExtractionService
    {
        private const int numberOfCharsInOperationId = 36;

        public static async Task<ExtractedTextModel> ExtractAsync(string fileName)
        {
            var cacheFileName = fileName + ".json";
            IList<TextRecognitionResult> result;
            if (File.Exists(cacheFileName))
            {
                result = JsonConvert.DeserializeObject<IList<TextRecognitionResult>>(File.ReadAllText(cacheFileName));
            }
            else
            {
                result = await MakeOcrRequestAsync(fileName);
                File.WriteAllText(cacheFileName, JsonConvert.SerializeObject(result));
            }
            return new ExtractedTextModel { OcrResults = result };
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using
        /// the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with printed text.</param>
        static async Task<IList<TextRecognitionResult>> MakeOcrRequestAsync(string imageFilePath)
        {
            const string subscriptionKey = "";
            var computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new DelegatingHandler[] { });
            // You must use the same region as you used to get your subscription
            // keys. For example, if you got your subscription keys from westus,
            // replace "westcentralus" with "westus".
            //
            // Free trial subscription keys are generated in the westcentralus
            // region. If you use a free trial subscription key, you shouldn't
            // need to change the region.
            // Specify the Azure region
            computerVision.Endpoint = "https://westcentralus.api.cognitive.microsoft.com";
            BatchReadFileInStreamHeaders headers;
            //RecognizeTextInStreamHeaders headers;
            using (var image = File.OpenRead(imageFilePath))
            {
                headers = await computerVision.BatchReadFileInStreamAsync(image);
                //headers = await computerVision.RecognizeTextInStreamAsync(image, TextRecognitionMode.Printed);
            }

            var operationId = headers.OperationLocation.Substring(headers.OperationLocation.Length - numberOfCharsInOperationId);

            var result = await computerVision.GetReadOperationResultAsync(operationId);

            // Wait for the operation to complete
            var i = 0;
            var maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                Console.WriteLine("Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);
                result = await computerVision.GetReadOperationResultAsync(operationId);
            }

            // Display the results
            Console.WriteLine();
            var recResults = result.RecognitionResults;
            foreach (TextRecognitionResult recResult in recResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }

            return recResults;
        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }

    public class ExtractedTextModel
    {
        public IList<TextRecognitionResult> OcrResults { get; internal set; }
    }
}
