using Microsoft.AspNetCore.Mvc;
using Nest;
using OfficeOpenXml;
using PetrolStation.Infrastructure;
using PetrolStation.View.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.View.Controllers
{
    [Route("petrolStation")]
    public class PetrolStationsController : Controller
    {
        private readonly IElasticClient elasticClient;
        private readonly ElasticsearchEntityRepository<Entities.PetrolStation> repository;

        public PetrolStationsController(IElasticClient elasticClient, ElasticsearchEntityRepository<Entities.PetrolStation> repository)
        {
            this.elasticClient = elasticClient;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<PetrolStationResponse>> GetPetrolStations()
        {
            var elasticResponse = await elasticClient.SearchAsync<Entities.PetrolStation>();
            var documents = new List<PetrolStationResponse>();

            foreach (var item in elasticResponse.Documents)
            {
                documents.Add(new PetrolStationResponse(
                    id: item.Id,
                    name: item.Name));
            }

            return documents;
        }

        [HttpGet("{id}")]
        public async Task<PetrolStationDetailsResponse> GetPetrolStationDetails([FromRoute] Guid id)
        {
            var petrolStation = await repository.GetAsync(id);

            var response = new PetrolStationDetailsResponse(
                id: petrolStation.Id,
                name: petrolStation.Name,
                time: petrolStation.Time,
                tanks: petrolStation.Tanks?.Select(x => new TankResponse(
                    id: x.Id,
                    maximumVolume: x.MaximumVolume,
                    currentVolume: x.CurrentVolume,
                    petrolTemperature: x.PetrolTemperature,
                    tankHigh: x.TankHigh,
                    nozzleResponses: x.Nozzles?.Select(z => new NozzleResponse(
                        id: z.Id,
                        lastTransactionVolume: z.LastTransactionVolume,
                        totalStolenPetrolAmount: z.TotalStolenPetrolAmount,
                        lastStolenPetrolAmount: z.LastStolenPetrolAmount,
                        totalPetrolAmount: z.TotalPetrolAmount)).ToList())).ToList(),
                raports: petrolStation.Reports?.OrderByDescending(x => x.TimeStamp).Select(x => new PetrolStationRaportResponse(
                    tankId: x.TankId,
                    nozzleId: x.NozzleId,
                    timeStamp: x.TimeStamp,
                    report: x.Report,
                    status: x.Status.ToString())).ToList());

            return response;
        }

        [HttpGet("{id}/report")]
        public async Task<IActionResult> GenerateRaport([FromRoute] Guid id)
        {
            var comlumHeadrs = new string[]
            {
                "Tank id",
                "Nozzle id",
                "Time stamp",
                "Status",
                "Report"
            };

            var petrolStation = await repository.GetAsync(id);

            byte[] result;

            using (var package = new ExcelPackage())
            {
                // add a new worksheet to the empty workbook

                var worksheet = package.Workbook.Worksheets.Add("Report");
                using (var cells = worksheet.Cells[1, 1, 1, 5]) //(1,1) (1,5)
                {
                    cells.Style.Font.Bold = true;
                }

                //First add the headers
                for (var i = 0; i < comlumHeadrs.Count(); i++)
                {
                    worksheet.Cells[1, i + 1].Value = comlumHeadrs[i];
                }

                //Add values
                var j = 2;
                foreach (var raport in petrolStation.Reports)
                {
                    worksheet.Cells["A" + j].Value = raport.TankId;
                    worksheet.Cells["B" + j].Value = raport.NozzleId;
                    worksheet.Cells["C" + j].Value = raport.TimeStamp.ToString("dd/MM/yyyy HH:mm:ss.f");
                    worksheet.Cells["D" + j].Value = raport.Status.ToString();
                    worksheet.Cells["E" + j].Value = raport.Report;

                    j++;
                }
                result = package.GetAsByteArray();
            }



            var fileName = $"PetrolStationName-{petrolStation.Name}-PetrolStationId-{petrolStation.Id}-Time-{petrolStation.Time}-Report.xlsx";
            return File(result, "application/ms-excel", fileName);
        }
    }
}
