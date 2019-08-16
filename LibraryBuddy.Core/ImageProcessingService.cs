using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class ImageProcessingService
    {
        public static async Task<ImageProcessingResult> ProcessAsync(string fileName)
        {
            var extractedTextModel = await TextExtractionService.ExtractAsync(fileName);
            var title = await TitleService.ExtractAsync(extractedTextModel);
            var arTask = ArQueryService.QueryTitleAsync(title);
            var amazonTask = AmazonQueryService.QueryTitleAsync(title);
            var commonSenseMediaTask = CommonSenseMediaQueryService.QueryTitleAsync(title);
            await Task.WhenAll(arTask, amazonTask, commonSenseMediaTask);
            return new ImageProcessingResult { ExtractedTextModel = extractedTextModel };
        }
    }

    public class ImageProcessingResult
    {
        public ExtractedTextModel ExtractedTextModel { get; internal set; }
    }
}