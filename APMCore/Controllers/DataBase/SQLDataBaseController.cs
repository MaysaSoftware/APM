using Microsoft.AspNetCore.Mvc;

namespace APMCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SQLDataBaseController : ControllerBase
    {

        [HttpGet(Name = "GetSQLDataBase")]
        public string Get()
        {
            return "Hellow world";
        }
    }
}