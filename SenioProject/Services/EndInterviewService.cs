using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SenioProject.Mappers;
using SenioProject.Models.DTO.BothSideDto;
using SenioProject.Repositories;

namespace SenioProject.Services
{
    public class EndInterviewService
    {
        private readonly EndInterviewRepository _endInterviewRepository;

        public EndInterviewService(EndInterviewRepository endInterviewRepository)
        {
            _endInterviewRepository = endInterviewRepository;
            
        }

        public async Task<EvaluationReportDto> EndInterview(SuperListDto superListDto)
        {

            EvaluationReportDto report = await GenerateEvaluationReport(superListDto);
            
            var interview = report.ToInterview(superListDto.Specialty, superListDto.IntervieweeId);
            await _endInterviewRepository.CreateIntervieweeAsync(interview);


            await _endInterviewRepository.SaveChangesAsync();

            return report;

        }

        private async Task<EvaluationReportDto> GenerateEvaluationReport(SuperListDto superListDto)
        {
 
            EvaluationReportDto report = new ();

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

                var json = JsonConvert.SerializeObject(superListDto);
                Console.WriteLine("\n\n\n\n\n\n\n1 - "+json+"\n\n\n\n\n\n\n");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/report", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n\n\n\n\n\n\n2 - "+responseContent+"\n\n\n\n\n\n\n");
                EvaluationReportDto result = JsonConvert.DeserializeObject<EvaluationReportDto>(responseContent);
                Console.WriteLine($"Deserialized result: {JsonConvert.SerializeObject(result)}");

                // Check if the result or the list is null
                if (result == null || result.EvaluationReport == null )
                {
                    Console.WriteLine("No questions returned from AI service.");
                    throw new InvalidOperationException("No questions found in the response from the AI service.");
                }

                report = result;
            }
            

            return report;


        }

         
    }
}