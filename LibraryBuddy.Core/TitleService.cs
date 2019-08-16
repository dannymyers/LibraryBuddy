using System;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class TitleService
    {
        public static Task<TitleResultModel> ExtractAsync(ExtractedTextModel extractedTextModel)
        {
            return Task.FromResult(new TitleResultModel());
        }
    }

    public class TitleResultModel
    {

    }
}
