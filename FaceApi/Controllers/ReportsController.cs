using ClosedXML.Excel;
using FaceApi.Services;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaceApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly PresenceService _presenceService;

        public ReportsController(PresenceService presenceService)
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
            [FromQuery] int? schoolId)
        {
            var records = await _presenceService.FilterAsync(start, end, userId, schoolId);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Presenças");

            // Cabeçalho
            ws.Cell(1, 1).Value = "Data/Hora";
            ws.Cell(1, 2).Value = "Professor";
            ws.Cell(1, 3).Value = "Escola";

            // Dados
            for (int i = 0; i < records.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = records[i].DateTime.ToString("dd/MM/yyyy HH:mm", new CultureInfo("pt-BR"));
                ws.Cell(i + 2, 2).Value = records[i].User.Name;
                ws.Cell(i + 2, 3).Value = records[i].School.Name;
            }

            ws.Columns().AdjustToContents();

            // Gerar arquivo Excel na memória
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            string filename = $"Presencas_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                filename);
        }
    }
}