using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Tasinmaz.Business.Abstract;
using Tasinmaz.Data;
using Tasinmaz.Dtos;
using Tasinmaz.Entities.Concrete;



namespace Tasinmaz.Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IAuthRepository authRepository,
            ILogService logService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger)
        {
            _authRepository = authRepository;
            _logService = logService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string GetUserIp()
        {
            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "N/A";
        }
        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _authRepository.GetAllUsersAsync();

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            }).ToList();
        }
        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _authRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UserForUpdateDto updateDto)
        {
            var userToUpdate = await _authRepository.GetUserByIdAsync(id);
            if (userToUpdate == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            var sameEmailUser = await _authRepository.GetUserByEmailAsync(updateDto.Email);
            if (sameEmailUser != null && sameEmailUser.Id != id)
                throw new Exception("Bu email başka bir kullanıcıya aittir.");

            var oldRole = userToUpdate.Role;

            userToUpdate.FullName = updateDto.FullName;
            userToUpdate.Email = updateDto.Email;
            userToUpdate.Role = updateDto.Role;
            userToUpdate.IsActive = updateDto.IsActive;

            _authRepository.UpdateUser(userToUpdate);
            var success = await _authRepository.SaveChangesAsync();

            if (!success)
                throw new Exception("Kullanıcı güncellenirken bir hata oluştu.");

            await _logService.AddLogAsync(
                userId: id,
                operationType: "UpdateUser",
                description: $"Kullanıcı güncellendi → Email: {userToUpdate.Email}, Rol: {oldRole} → {updateDto.Role}",
                status: "Success",
                userIp: GetUserIp()
            );

            return new UserDTO
            {
                Id = userToUpdate.Id,
                FullName = userToUpdate.FullName,
                Email = userToUpdate.Email,
                Role = userToUpdate.Role,
                IsActive = userToUpdate.IsActive,
                CreatedAt = userToUpdate.CreatedAt
            };
        }

        public async Task DeleteUserAsync(int id)
        {
            var userToDelete = await _authRepository.GetUserByIdAsync(id);
            if (userToDelete == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            await _logService.AddLogAsync(
                userId: id,
                operationType: "DeleteUser",
                description: $"Kullanıcı silindi → Email: {userToDelete.Email}",
                status: "Success",
                userIp: GetUserIp()
            );

            _authRepository.DeleteUser(userToDelete);
            var success = await _authRepository.SaveChangesAsync();

            if (!success)
                throw new Exception("Kullanıcı silinirken hata oluştu.");
        }

        public async Task<UserDTO> RegisterUserAsync(UserForRegisterDto registerDto)
        {
            var exists = await _authRepository.GetUserByEmailAsync(registerDto.Email);
            if (exists != null)
                throw new Exception("Email zaten kayıtlı.");

            var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var combined = Convert.FromBase64String(salt)
                .Concat(Encoding.UTF8.GetBytes(registerDto.Password))
                .ToArray();

            using var sha = SHA256.Create();
            var hash = Convert.ToBase64String(sha.ComputeHash(combined));

            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Salt = salt,
                PasswordHash = hash,
                Role = registerDto.Role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _authRepository.AddUserAsync(user);
            await _authRepository.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<byte[]> ExportUsersToExcelAsync()
        {
            var users = await _authRepository.GetAllUsersAsync();
            return ExportUsersToExcel(users);
        }

        private byte[] ExportUsersToExcel(List<User> users)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Kullanıcılar");

            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Ad Soyad";
            ws.Cell(1, 3).Value = "Email";
            ws.Cell(1, 4).Value = "Rol";

            int row = 2;
            foreach (var u in users)
            {
                ws.Cell(row, 1).Value = u.Id;
                ws.Cell(row, 2).Value = u.FullName;
                ws.Cell(row, 3).Value = u.Email;
                ws.Cell(row, 4).Value = u.Role;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }





        private byte[] ExportUsersToPdf(List<User> users)
        {
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Header()
                        .Text("KULLANICI LİSTESİ")
                        .FontSize(16)
                        .SemiBold()
                        .AlignCenter();

                    page.Content().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(50);   
                            c.RelativeColumn(3);    
                            c.RelativeColumn(4);    
                            c.RelativeColumn(2);    
                        });

                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderStyle).Text("ID");
                            h.Cell().Element(HeaderStyle).Text("Ad Soyad");
                            h.Cell().Element(HeaderStyle).Text("Email");
                            h.Cell().Element(HeaderStyle).Text("Rol");

                            static IContainer HeaderStyle(IContainer container) =>
                                container
                                    .Background(Colors.Grey.Lighten3)
                                    .Padding(5)
                                    .Border(1)
                                    .AlignCenter()
                                    .DefaultTextStyle(x => x.SemiBold());
                        });

                        foreach (var u in users)
                        {
                            table.Cell().Element(CellStyle).Text(u.Id.ToString());
                            table.Cell().Element(CellStyle).Text(u.FullName ?? "");
                            table.Cell().Element(CellStyle).Text(u.Email ?? "");
                            table.Cell().Element(CellStyle).Text(u.Role ?? "");
                        }

                        static IContainer CellStyle(IContainer container) =>
                            container
                                .Padding(5)
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

        public async Task<byte[]> ExportUsersToPdfAsync()
        {
            var users = await _authRepository.GetAllUsersAsync();
            return ExportUsersToPdf(users);
        }





    }
}