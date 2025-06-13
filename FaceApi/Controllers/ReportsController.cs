using ClosedXML.Excel;
using FaceApi.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FaceApi.Enums;

namespace FaceApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IPresenceService _presenceService;

        public ReportsController(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        /// <summary>
        /// Exporta registros de presença para Excel.
        /// Filtros opcionais: período, usuário (professor), escola.
        /// Exemplo de chamada: 
        /// /api/report/export-excel?start=2024-06-01&end=2024-06-30&userId=1&schoolId=2
        /// </summary>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] DateTime? start,
            [FromQuery] DateTime? end,
            [FromQuery] int? userId,
            [FromQuery] int? schoolId,
            [FromQuery] UserType? userType)
        {
            var records = await _presenceService.FilterAsync(start, end, userId, schoolId, userType);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Presenças");

            // Cabeçalho
            ws.Cell(1, 1).Value = "Date/Time";
            ws.Cell(1, 2).Value = "User";
            ws.Cell(1, 3).Value = "School";
            ws.Cell(1, 4).Value = "Type";

            var uaeTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time");

            // Dados
            for (int i = 0; i < records.Count; i++)
            {
                var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(records[i].DateTime, uaeTimeZone);
                ws.Cell(i + 2, 1).Value = localDateTime.ToString("dd/MM/yyyy HH:mm", new CultureInfo("pt-BR"));
                ws.Cell(i + 2, 2).Value = records[i].User.Name;
                ws.Cell(i + 2, 3).Value = records[i].School.Name;
                ws.Cell(i + 2, 4).Value = records[i].User.UserType.ToString();
            }

            ws.Columns().AdjustToContents();

            // Gerar arquivo Excel na memória
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Ajustar data do nome do arquivo para o horário local dos EAU
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, uaeTimeZone);
            string filename = $"Presencas_{localNow:yyyyMMdd_HHmm}.xlsx";

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename);
        }
    }
}