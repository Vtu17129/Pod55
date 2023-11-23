using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LoggingMicroservice.Data;
using LoggingMicroservice.Models;

namespace LoggingMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<LogsController> _logger;

        public LogsController(DataContext context, ILogger<LogsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet()]
        public ActionResult<IEnumerable<Log>> Get() 
        {
             if (_context.Logs == null)
            {
                return NotFound();
            }

            var logs = _context.Logs.AsQueryable();
            return Ok(logs);
        }

    [HttpPost]
public IActionResult PostLog(Log log)
{
    // Check if context is available
    if (_context.Logs == null)
    {
        return NotFound();
    }

   try{
    
    var missingFields = new List<string>();
    // Check each attribute of the log object
    if (string.IsNullOrEmpty(log.LogLevel)) missingFields.Add(nameof(log.LogLevel));
    if (string.IsNullOrEmpty(log.HostName)) missingFields.Add(nameof(log.HostName));
    if (log.AssociateId == null) missingFields.Add(nameof(log.AssociateId));
    if (string.IsNullOrEmpty(log.Technology)) missingFields.Add(nameof(log.Technology));
    if (string.IsNullOrEmpty(log.ModuleName)) missingFields.Add(nameof(log.ModuleName));
    if (string.IsNullOrEmpty(log.FeatureName)) missingFields.Add(nameof(log.FeatureName));
    if (string.IsNullOrEmpty(log.ClassName)) missingFields.Add(nameof(log.ClassName));
    if (string.IsNullOrEmpty(log.ErrorCode)) missingFields.Add(nameof(log.ErrorCode));
    if (string.IsNullOrEmpty(log.ErrMessage)) missingFields.Add(nameof(log.ErrMessage));

    // If there are missing fields, return an error message
    if (missingFields.Any())
    {
        return BadRequest($"The following fields are missing or empty: {string.Join(", ", missingFields)}");
    }

    log.CreatedAt = DateTime.Now;
    _context.Logs.Add(log);
    _context.SaveChanges();
    _logger.LogInformation("New log: {@Log}", log);


    //Setting up log string to enter into file
    string logEntry = $"Log Entry: {DateTime.Now}\n" +
                $"LogLevel: {log.LogLevel}\n" +
                $"Serverity: {log.LogSeverity}\n"+
                $"HostName: {log.HostName}\n"+
                $"Associated Id: {log.AssociateId}\n"+
                $"Technology: {log.Technology}\n"+
                $"ModuleName: {log.ModuleName}\n"+
                $"FeatureName: {log.FeatureName}\n"+
                $"ClassName: {log.ClassName}\n"+
                $"ErrorCode: {log.ErrorCode}\n"+
                $"Error Message: {log.ErrMessage}\n"+
                $"----------------------------------\n";

    string directoryPath = "logs"; 
    string filePath = Path.Combine(directoryPath, $"logs_{DateTime.Now:yyyy-MM-dd}.txt");
    Directory.CreateDirectory(directoryPath);

    // Writing the log entry to the file
    System.IO.File.AppendAllText(filePath, logEntry);
    
    return Ok("Logged Successfully");
   }
   catch(Exception ex){
       return BadRequest(ex.Message);
   }
}

        [HttpGet("{id}")]
        public IActionResult GetLog(int id)
        {
             if (_context.Logs == null)
            {
                return NotFound();
            }

            var log = _context.Logs.Find(id);
            if (log == null)
            {
                return NotFound();
            }
            return Ok(log);
        }

        
    }
}
