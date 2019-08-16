using System;
using System.Threading.Tasks;

namespace LibraryBuddy.Core
{
    public static class AmazonQueryService
    {
        public static Task<AmazonResultModel> QueryTitleAsync(TitleResultModel title)
        {
            return Task.FromResult(new AmazonResultModel());
        }
    }

    public class AmazonResultModel
    {

    }
}
