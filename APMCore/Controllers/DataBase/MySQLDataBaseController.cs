using Microsoft.AspNetCore.Mvc;

namespace APMCore.Controllers.DataBase
{
    [ApiController]
    [Route("[controller]")]
    public class MySQLDataBaseController : ControllerBase
    {
        [HttpGet(Name = "GetMySQLDataBase")]
        public string Get()
        {
            return "Hellow world";
        }
    }
}
