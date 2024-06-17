using System.Web.Http;

namespace VMMS
{
    [RoutePrefix("api/home")]
    public class HomeController : ApiController
    {
        [Route("LoadDataGrid")]
        [HttpGet]
        public IHttpActionResult Echo(ObjBill obj)
        {
            return Json(new { Name = obj, Message = DalBill.GetList(obj) });
        }
    }
}
