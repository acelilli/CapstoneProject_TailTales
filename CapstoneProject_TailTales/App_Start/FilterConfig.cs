using System.Web;
using System.Web.Mvc;

namespace CapstoneProject_TailTales
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
