using Microsoft.AspNetCore.Mvc;
using SenioProject.Mappers;
using SenioProject.Models.DTO.BothSideDto;
using SenioProject.Models.DTO.FromVRSideDto;
using SenioProject.Repositories;
using SenioProject.Services;


namespace SenioProject.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private  IntervieweeDataRepository _IntervieweeDataRepository;
        private StartInterviewService _StartInterviewService;
        private EndInterviewService _EndInterviewService;
        public HomeController(IntervieweeDataRepository intervieweeDataRepository,
         StartInterviewService startInterviewService, EndInterviewService endInterviewService)
        {
            _IntervieweeDataRepository = intervieweeDataRepository;
            _StartInterviewService = startInterviewService;
            _EndInterviewService = endInterviewService;
        }    


        [HttpPost("CreateInterviewee")]
        public async Task<IActionResult> CreateInterviewee([FromBody] CreateIntervieweeDto createIntervieweeDto){

            if(createIntervieweeDto == null)
            return BadRequest();

            var interviewee = createIntervieweeDto.FromCreateIntervieweeDtoToInterviewee();
            var statusAction = await _IntervieweeDataRepository.CreateIntervieweeAsync(interviewee);

            if(statusAction == null)
            return BadRequest("Can not create, Interviewee already exists");

            return Ok("Interviewee has been added succesfully");
        } 

        [HttpPatch("UpdateCV")]
        public async Task<IActionResult> UpdateCV([FromBody] UpdateCVDto updateCVDto){

            var IntervieweeObjectToUpdateCV = await _IntervieweeDataRepository.UpdateIntervieweeCVAsync(updateCVDto);

            if(IntervieweeObjectToUpdateCV == null)
                 return NotFound("Interviewee does not exists");

                 return Ok("CV has been updated succesfully");
        }

        [HttpDelete("DeleteInterviewee")]
        public async Task<IActionResult> DeleteCV([FromBody] DeleteIntervieweeDto deleteIntervieweeDto){

            var IntervieweeObjectToUpdateCV = await _IntervieweeDataRepository.DeleteIntervieweeCVAsync(deleteIntervieweeDto);

            if(IntervieweeObjectToUpdateCV == null)
                 return NotFound("Interviewee does not exists");

                 return Ok("Interviewee has been deleted succesfully");
        }

        //POST api/interview/start
        [HttpPost("startInterview")]
        public async Task<IActionResult> StartInterview([FromBody] StartInterviewDto request)
        {
           // Validate the input
            if (request == null)
            {
                return BadRequest("Invalid interview start data.");
            }

            var result = await _StartInterviewService.startInterview(request);

            // Simulate business logic:
            // For example, generate a set of questions with answers based on the candidate's specialty,
            // question difficulty and source.

            // var questions = new List<QuestionDto>
            // {
            //     new QuestionDto { QuestionId = 1, Question = "What is polymorphism?", AppropriateAnswer = "Polymorphism is..." },
            //     new QuestionDto { QuestionId = 2, Question = "Explain SOLID principles.", AppropriateAnswer = "SOLID stands for ..." }
            //     // Add more questions as needed.
            // };
            
            //var questions = new List<QuestionFromAIDto>();
            // Return the list of questions to the candidate
            return Ok(result);
        }

        // POST api/interview/end
        [HttpPost("endInterview")]
        public async Task<IActionResult> EndInterview([FromBody] SuperListDto request)
        {
            // Validate candidate answers
            if (request == null )
            {
                return BadRequest("Invalid candidate answers.");
            }

            // Simulate business logic:
            // Process the candidate's answers to generate an evaluation report. This could involve matching the answers
            // against the expected ones, scoring, and providing feedback.
            // var evaluationReport = new EvaluationReportDto
            // {
            //     Summary = "The candidate demonstrated a good understanding of fundamental concepts but should improve on SOLID principles.",
            //     Score = 85.0,  // Example score
            //     Recommendations = "Review SOLID principles and design patterns."
            // };

            var result = await _EndInterviewService.EndInterview(request);

            // Return the evaluation report to the candidate
            return Ok(result);
        }
    }
