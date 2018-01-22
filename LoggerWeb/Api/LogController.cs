using System.Web.Http;
using Logger.Models;

namespace LoggerWeb.Api
{
    //[RoutePrefix("api/log")]
    public class LogController : ApiController
    {
        private static string path = @"C:\Users\Cornelia\source\repos\Logger\LoggerWeb\TextFile1.txt";

        //private string[] _listOfLines;
        private readonly Logger.Logger _logger;
        private int _offset;

        public LogController()
        {
            _logger = new Logger.Logger(path, 10);
            _offset = 0;
        }

        //[Route("read")]
        [HttpGet]
        public LogData Read(int offset)
        {
            var logData = _logger.ReadLines(offset);
            
            return logData;            
        }                
    }
}