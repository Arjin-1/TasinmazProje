using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Entities.Concrete;
using System.Threading.Tasks;
using Tasinmaz.Dtos;
using System.Collections.Generic;

namespace Tasinmaz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    [Authorize(Roles = "Admin")] 
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]

        public async Task<IActionResult> GetFilteredLogs([FromQuery] LogFilterDTO filter)
        {
         
            var pagedLogs = await _logService.GetFilteredLogsAsync(filter);

            if (pagedLogs.TotalCount == 0)
            {
                
                return NotFound("No log records match the criteria.");
            }

            
            return Ok(pagedLogs);
        }


        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportLogsToExcel([FromQuery] LogFilterDTO filter)
        {
            var logs = await _logService.GetLogsForExportAsync(filter);

            if (!logs.Any())
            {
                return Ok(new { message = "Export edilecek log kaydı bulunamadı." });
            }

            var fileBytes = _logService.ExportToExcel(logs);

            var fileName = $"Logs_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }


        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportLogsToPdf([FromQuery] LogFilterDTO filter)
        {
            var logs = await _logService.GetLogsForExportAsync(filter);

            if (!logs.Any())
            {
                return Ok(new { message = "Export edilecek log kaydı bulunamadı." });
            }

            var fileBytes = _logService.ExportToPdf(logs);

            var fileName = $"Logs_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";

            return File(
                fileBytes,
                "application/pdf",
                fileName
            );
        }

    }
}