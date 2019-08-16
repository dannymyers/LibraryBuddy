using System;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class CommonSenseMediaQueryService
    {
        public static Task<CommonSenseMediaResultModel> QueryTitleAsync(TitleResultModel title)
        {
            return Task.FromResult(new CommonSenseMediaResultModel());
        }
    }

    public class CommonSenseMediaResultModel
    {

    }
}
