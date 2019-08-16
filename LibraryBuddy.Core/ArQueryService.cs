using System;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class ArQueryService
    {
        public static Task<ArResultModel> QueryTitleAsync(TitleResultModel title)
        {
            return Task.FromResult(new ArResultModel());
        }
    }

    public class ArResultModel
    {

    }
}
