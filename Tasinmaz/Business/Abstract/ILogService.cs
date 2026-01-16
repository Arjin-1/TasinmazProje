using System.Threading.Tasks;
using Tasinmaz.Entities.Concrete;
using Tasinmaz.Dtos;
using System.Collections.Generic;

public interface ILogService
{
    Task AddLogAsync(int userId, string operationType, string description, string status, string userIp);

    Task<PagedResult<Log>> GetFilteredLogsAsync(LogFilterDTO filter);


    Task<List<Log>> GetLogsForExportAsync(LogFilterDTO filter);

    byte[] ExportToExcel(List<Log> logs);
    byte[] ExportToPdf(List<Log> logs);
}
