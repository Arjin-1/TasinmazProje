using Tasinmaz.Business.Abstract;
using Tasinmaz.DataAccess;
using Tasinmaz.Entities.Concrete;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Tasinmaz.Dtos;
using System.Collections.Generic;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


public class LogService : ILogService
{
    private readonly AppDbContext _context;

    public LogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddLogAsync(int userId, string operationType, string description, string status, string userIp)
    {
       
        var logEntry = new Log
        {
            UserId = userId,
            OperationType = operationType,
            Description = description,
            Status = status,
            Timestamp = DateTime.UtcNow,
            UserIp = userIp
        };

        _context.Log.Add(logEntry);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Log> ApplyFilters(IQueryable<Log> query, LogFilterDTO filter)
    {
        if (filter.UserId.HasValue && filter.UserId.Value > 0)
        {
            query = query.Where(l => l.UserId == filter.UserId.Value);
        }
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            var statusFilterPattern = $"%{filter.Status}%";
            query = query.Where(l => EF.Functions.ILike(l.Status, statusFilterPattern));
        }
        if (!string.IsNullOrWhiteSpace(filter.OperationType))
        {
            var opTypeFilterPattern = $"%{filter.OperationType}%";
            query = query.Where(l => EF.Functions.ILike(l.OperationType, opTypeFilterPattern));
        }

        if (!string.IsNullOrWhiteSpace(filter.Description))
        {
            var descFilterPattern = $"%{filter.Description}%";
            query = query.Where(l => EF.Functions.ILike(l.Description, descFilterPattern));
        }

        if (!string.IsNullOrWhiteSpace(filter.UserIp))
        {
            var ipFilterPattern = $"%{filter.UserIp}%";
            query = query.Where(l => EF.Functions.ILike(l.UserIp, ipFilterPattern));
        }

        if (filter.StartTimestamp.HasValue)
        {
            query = query.Where(l => l.Timestamp >= filter.StartTimestamp.Value);
        }
        if (filter.EndTimestamp.HasValue)
        {
            query = query.Where(l => l.Timestamp < filter.EndTimestamp.Value.AddDays(1));
        }

        return query;
    }


    public async Task<PagedResult<Log>> GetFilteredLogsAsync(LogFilterDTO filter)
    {
        
        var query = _context.Log.AsQueryable();

       
        query = ApplyFilters(query, filter);

      
        var totalCount = await query.CountAsync();

        
        var logs = await query
            .OrderByDescending(l => l.Timestamp) 
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Log>
        {
            Items = logs,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<List<Log>> GetLogsForExportAsync(LogFilterDTO filter)
    {
      
        var query = _context.Log.AsQueryable();
        query = ApplyFilters(query, filter);

       
        return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
    }



    public byte[] ExportToExcel(List<Log> logs)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Log Kayıtları");

        // HEADER
        ws.Cell(1, 1).Value = "ID";
        ws.Cell(1, 2).Value = "UserId";
        ws.Cell(1, 3).Value = "İşlem Türü";
        ws.Cell(1, 4).Value = "Açıklama";
        ws.Cell(1, 5).Value = "Durum";
        ws.Cell(1, 6).Value = "IP Adresi";
        ws.Cell(1, 7).Value = "Tarih";

        int row = 2;
        foreach (var log in logs)
        {
            ws.Cell(row, 1).Value = log.Id;
            ws.Cell(row, 2).Value = log.UserId;
            ws.Cell(row, 3).Value = log.OperationType;
            ws.Cell(row, 4).Value = log.Description;
            ws.Cell(row, 5).Value = log.Status;
            ws.Cell(row, 6).Value = log.UserIp;
            ws.Cell(row, 7).Value = log.Timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }


    public byte[] ExportToPdf(List<Log> logs)
    {
        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

            
                page.Header()
                    .Text("LOG KAYITLARI")
                    .FontSize(16)
                    .SemiBold()
                    .AlignCenter();

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(40);  
                        c.ConstantColumn(60);  
                        c.RelativeColumn(2);    
                        c.RelativeColumn(4);     
                        c.RelativeColumn(2);    
                        c.RelativeColumn(2);   
                        c.RelativeColumn(2);   
                    });

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("ID");
                        h.Cell().Element(HeaderCell).Text("User");
                        h.Cell().Element(HeaderCell).Text("İşlem");
                        h.Cell().Element(HeaderCell).Text("Açıklama");
                        h.Cell().Element(HeaderCell).Text("Durum");
                        h.Cell().Element(HeaderCell).Text("IP");
                        h.Cell().Element(HeaderCell).Text("Tarih");

                        static IContainer HeaderCell(IContainer c) =>
                            c.Background(Colors.Grey.Lighten3)
                             .Padding(4)
                             .Border(1)
                             .DefaultTextStyle(x => x.SemiBold());
                    });

    
                    foreach (var log in logs)
                    {
                        table.Cell().Element(Cell).Text(log.Id.ToString());
                        table.Cell().Element(Cell).Text(log.UserId.ToString());
                        table.Cell().Element(Cell).Text(log.OperationType);
                        table.Cell().Element(Cell).Text(log.Description);
                        table.Cell().Element(Cell).Text(log.Status);
                        table.Cell().Element(Cell).Text(log.UserIp);
                        table.Cell().Element(Cell)
                            .Text(log.Timestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm"));
                    }

                    static IContainer Cell(IContainer c) =>
                        c.Padding(4)
                         .BorderBottom(1)
                         .BorderColor(Colors.Grey.Lighten2);
                });


                page.Footer()
                    .AlignRight()
                    .Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });

        return doc.GeneratePdf();
    }

}