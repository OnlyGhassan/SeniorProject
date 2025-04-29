using System.Text;
using SenioProject.Mappers;
using SenioProject.Models.DTO.BothSideDto;
using SenioProject.Models.DTO.FromVRSideDto;
using SenioProject.Models.DTO.ServerSideDto.ToServerSide;
using SenioProject.Repositories;
using Newtonsoft.Json;
using SenioProject.Models.DTO.ServerSideDto.FromServerSide;


namespace SenioProject.Services
{
    public class StartInterviewService
    {
        private readonly StartInterviewRepository _startInterviewRepository;
        private static readonly object _lock = new object();



        private readonly ILogger<StartInterviewService> _logger;
        public StartInterviewService(StartInterviewRepository startInterviewRepository, ILogger<StartInterviewService> logger)
        {
            _startInterviewRepository = startInterviewRepository;
            _logger = logger;
        }

        static SuperListDto superListDto;
        static int QuestionNumberIncrementer ;

        public async Task<SuperListDto> startInterview(StartInterviewDto startInterviewDto)
        {

            QuestionNumberIncrementer = 1;
            superListDto = new SuperListDto();

            int HRQuestionNumber = 0;
            int CVQuestionNumber = 0;
            int PublicModelQuestionNumber = 9;
            int PrivateModelQuestionNumber = 1;
            int DatabaseQuestionNumber = 9;

            superListDto = new SuperListDto
            {
                FullName = await _startInterviewRepository.GetFullNameByID(startInterviewDto.IntervieweeId),
                Specialty = startInterviewDto.Specialty,
                IntervieweeId = startInterviewDto.IntervieweeId,
                SuperList = new List<SuperDto>() // Initialize the list
            };

            await _startInterviewRepository.SaveChangesAsync();

            await GenerateHRQuestion(HRQuestionNumber);

            await GenerateCVQuestionAsync(startInterviewDto.IntervieweeId, CVQuestionNumber);

            var hold = "";

            if (startInterviewDto.QustionType == "PublicModel")
            {
                var generatequestionDto = startInterviewDto.ToGenerateQuestionDto(PublicModelQuestionNumber);
                hold =await GeneratePublicModelQuestionAsync(generatequestionDto);
            }
            else if (startInterviewDto.QustionType == "PrivateModel")
            {
                var generatequestionDto = startInterviewDto.ToGenerateQuestionDto(PrivateModelQuestionNumber);
                hold =await GeneratePrivateModelQuestion(generatequestionDto);
            }
            else if (startInterviewDto.QustionType == "Database")
            {
                var generatequestionDto = startInterviewDto.ToGenerateQuestionDto(DatabaseQuestionNumber);
                    hold = await GenerateDatabaseQuestion(generatequestionDto);
            }
            else if (startInterviewDto.QustionType == "Hybrid")
            {
                
                var generatequestionDto2 = startInterviewDto.ToGenerateQuestionDto(DatabaseQuestionNumber / 3);
                    hold = await GenerateDatabaseQuestion(generatequestionDto2);

                var generatequestionDto1 = startInterviewDto.ToGenerateQuestionDto(PublicModelQuestionNumber / 3);
                    hold = await GeneratePublicModelQuestionAsync(generatequestionDto1);

                var generatequestionDto3 = startInterviewDto.ToGenerateQuestionDto(PrivateModelQuestionNumber / 3);
                    hold = await GeneratePrivateModelQuestion(generatequestionDto3);

                
                
            }


            await _startInterviewRepository.SaveChangesAsync();
            

            return superListDto;
        }

        private async Task GenerateHRQuestion(int HRQuestionNumber)
        {


            for (int i = 0; i < HRQuestionNumber; i++)
            {

                // This assumes _startInterviewRepository.GetHRQuestionById exists
                var question = await _startInterviewRepository.GetHRQuestionById();

                if (question != null)
                {
                    superListDto.SuperList.Add(new SuperDto
                    {
                        QuestionNumber = QuestionNumberIncrementer++,
                        Question = question.Question,
                        AppropriateAnswer = string.Empty,
                        QustionType = "HR"
                    });
                }
            }
            await _startInterviewRepository.SaveChangesAsync();
        }



        private async Task GenerateCVQuestionAsync(string intervieweeId, int CVQuestionNumber)
        {
            // Get the CV string from repository
            string intervieweeCV = await  _startInterviewRepository.GetCVByID(intervieweeId);

            // Construct the DTO
            CVDto cVDto = new CVDto
            {
                IntervieweeCvText = intervieweeCV,
                NumberOfCVQuestions = CVQuestionNumber
            };

            // Create HttpClient and send the request
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

                var json = JsonConvert.SerializeObject(cVDto);
                Console.WriteLine("\n\n\n\n\n\n\n"+json+"\n\n\n\n\n\n\n");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("/generate-questions-Based-On-CV", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\n\n\n\n\n\n\n"+responseContent+"\n\n\n\n\n\n\n");
                CVQuestionListFromAIDto result = JsonConvert.DeserializeObject<CVQuestionListFromAIDto>(responseContent);
                Console.WriteLine($"Deserialized result: {JsonConvert.SerializeObject(result)}");

                // Check if the result or the list is null
                if (result == null || result.CVQuestionListFromAI == null || result.CVQuestionListFromAI.Count == 0)
                {
                    Console.WriteLine("No questions returned from CV question generation service.");
                    throw new InvalidOperationException("No questions found in the response from the AI service.");
                }

                // Loop through and add questions to the super list
                for (int i = 0; i < CVQuestionNumber; i++)
                {
                    // Check if the question is valid (not null or empty)
                    if (string.IsNullOrEmpty(result.CVQuestionListFromAI[i]))
                    {
                        break;
                    }

                    // Add question to SuperList
                    superListDto.SuperList.Add(new SuperDto
                    {
                        QuestionNumber = QuestionNumberIncrementer++,
                        Question = result.CVQuestionListFromAI[i],
                        AppropriateAnswer = string.Empty,
                        QustionType = "CV"
                    });
                }


            }
            await _startInterviewRepository.SaveChangesAsync();
        }

        // private List<SuperListDto> GenerateCVQuestion(string intervieweeId, int CVQuestionNumber)
        // {
        //     String IntervieweeCV =  _startInterviewRepository.GetCVByID(intervieweeId).ToString();
        //     CVDto cVDto =   new CVDto
        //     {
        //         // InterviewId is auto incremented in the database!
        //         IntervieweeCvText = IntervieweeCV,
        //         NumberOfCVQuestions = CVQuestionNumber
        //     };


        //     throw new NotImplementedException();
        // }

        private async Task<string> GenerateDatabaseQuestion(GenerateQuestionDto generateQuestionDto)
        {

            
            QuestionFromAIOrDatabaseDto question = new QuestionFromAIOrDatabaseDto();
             for (int i = 0; i < generateQuestionDto.NumberOfQuestions; i++)
            {

                // This assumes _startInterviewRepository.GetHRQuestionById exists
                if(generateQuestionDto.QuestionDifficulty == "Beginner")
                 question = await _startInterviewRepository.GetBeginnerSpecialtyQuestionById( generateQuestionDto.Specialty);
                else if(generateQuestionDto.QuestionDifficulty == "Intermediate")
                 question = await _startInterviewRepository.GetIntermediateSpecialtyQuestionById(generateQuestionDto.Specialty);
                else if(generateQuestionDto.QuestionDifficulty == "Advanced")
                 question = await _startInterviewRepository.GetAdvancedSpecialtyQuestionById(generateQuestionDto.Specialty);
                 
                

                if (question != null)
                {
                    superListDto.SuperList.Add(new SuperDto
                    {
                        QuestionNumber = QuestionNumberIncrementer++,
                        Question = question.Question,
                        AppropriateAnswer = question.AppropriateAnswer,
                        QustionType = "Database"
                    });
                }
            }
            await _startInterviewRepository.SaveChangesAsync();

            return "";
        }

        private async Task<string> GeneratePrivateModelQuestion(GenerateQuestionDto generateQuestionDto)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

                // Serialize the input object
                var json = JsonConvert.SerializeObject(generateQuestionDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // POST it
                var response = await httpClient.PostAsync("/generate-questions-Based-On-Private-Model", content);
                response.EnsureSuccessStatusCode();

                // Read response
                var responseJson = await response.Content.ReadAsStringAsync();
                QuestionListFromAIDto result = JsonConvert.DeserializeObject<QuestionListFromAIDto>(responseJson);

                for (int i = 0; i < generateQuestionDto.NumberOfQuestions; i++)
                {

                    if (result.QuestionListFromAI[i] == null ||
                    result.QuestionListFromAI.Any(x => x.Question == String.Empty) || result.QuestionListFromAI.Any(x => x.AppropriateAnswer == String.Empty))
                        break;

                    superListDto.SuperList.Add(new SuperDto
                    {
                        QuestionNumber = QuestionNumberIncrementer++,
                        Question = result.QuestionListFromAI[i].Question,
                        AppropriateAnswer = result.QuestionListFromAI[i].AppropriateAnswer,
                        QustionType = "PrivateModel"
                    });

                }


            }
            return "";
        }

        private async Task<string> GeneratePublicModelQuestionAsync(GenerateQuestionDto generateQuestionDto)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("http://127.0.0.1:8000");

                // Serialize the input object
                var json = JsonConvert.SerializeObject(generateQuestionDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // POST it
                var response = await httpClient.PostAsync("/generate-questions-Based-On-position", content);
                response.EnsureSuccessStatusCode();

                // Read response
                var responseJson = await response.Content.ReadAsStringAsync();
                QuestionListFromAIDto result = JsonConvert.DeserializeObject<QuestionListFromAIDto>(responseJson);

                if (result == null || result.QuestionListFromAI == null || result.QuestionListFromAI.Count == 0)
                {
                    Console.WriteLine("No questions returned from Specialty question generation service.");
                    throw new InvalidOperationException("No questions found in the response from the AI service.");
                }

                
                for (int i = 0; i < generateQuestionDto.NumberOfQuestions; i++)
                {

                    if (result.QuestionListFromAI[i] == null ||
                    result.QuestionListFromAI.Any(x => x.Question == String.Empty) || result.QuestionListFromAI.Any(x => x.AppropriateAnswer == String.Empty))
                        break;

                    superListDto.SuperList.Add(new SuperDto
                    {
                        QuestionNumber = QuestionNumberIncrementer++,
                        Question = result.QuestionListFromAI[i].Question,
                        AppropriateAnswer = result.QuestionListFromAI[i].AppropriateAnswer,
                        QustionType = "PublicModel"
                    });

                }


            }

            return "";
        }

        // private int GenerateRandomNumber()
        // {
        //     Random random = new Random();
        //     return random.Next(1, 350); // upper bound is exclusive, so 3001
        // }


        // private List<SuperListDto> GeneratePublicModelQuestion(GenerateQuestionDto generateQuestionDto)
        // {


        //     throw new NotImplementedException();
        // }


    }
}