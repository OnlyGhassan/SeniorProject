using Microsoft.EntityFrameworkCore;
using SenioProject.Models;
using SenioProject.Models.DTO.ServerSideDto.FromServerSide;

namespace SenioProject.Repositories
{
    public class StartInterviewRepository
    {
        private readonly SeniorProjectDbContext _context;

        public StartInterviewRepository(SeniorProjectDbContext context)
        {
            _context = context;
        }

        // public async Task<String?> CreateIntervieweeAsync(Interview newIntrview)
        // {
        //     // var testObeject = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == newIntrviewee.IntervieweeId);
        //     // if(testObeject != null)
        //     // return null;

        //     await _context.Interviews.AddAsync(newIntrview);
        //     await _context.SaveChangesAsync();
        //     return String.Empty;
        // }

        public async Task<String?> GetCVByID(String id)
        {
            var testObeject = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == id);

            return testObeject.IntervieweeCvText;
        }

        public async Task<QuestionFromAIOrDatabaseDto> GetHRQuestionById()
        {
            // var hrQuestion = await _context.Hrquestions.FirstOrDefaultAsync(q => q.HrquestionId == id);
            // if (hrQuestion == null) return null;

            // return hrQuestion.HrquestionQuestion;

            var result =  await _context.Hrquestions
               .OrderBy(q => Guid.NewGuid()) // This will randomize the results
               .FirstOrDefaultAsync();

               return new QuestionFromAIOrDatabaseDto
            {
                Question = result.HrquestionQuestion
            };
        }

        // public async Task<String?> GetBeginnerSpecialtyQuestionById(int id, String Specialty)
        // {
        //     var specialtyQuestion = await _context.BeginnerQuestions.FirstOrDefaultAsync
        //     (q => q.BeginnerQuestionsId == id && q.BeginnerQuestionsSpecialty == Specialty);

        //     if (specialtyQuestion == null) return null;

        //     return  specialtyQuestion.BeginnerQuestionsQuestion;
        // }

        public async Task<QuestionFromAIOrDatabaseDto> GetBeginnerSpecialtyQuestionById(string specialty)
        {
            var result = await _context.BeginnerQuestions
              .Where(q => q.BeginnerQuestionsSpecialty == specialty)
              .OrderBy(q => Guid.NewGuid()) // This will randomize the results
              .FirstOrDefaultAsync();

            if (result == null) return null;

            return new QuestionFromAIOrDatabaseDto
            {
                Question = result.BeginnerQuestionsQuestion,
                AppropriateAnswer = result.BeginnerQuestionsAnswer
            };
        }


        public async Task<QuestionFromAIOrDatabaseDto> GetIntermediateSpecialtyQuestionById(String specialty)
        {
            var result = await _context.IntermediateQuestions
              .Where(q => q.IntermediateQuestionSpecialty == specialty)
              .OrderBy(q => Guid.NewGuid()) // This will randomize the results
              .FirstOrDefaultAsync();

            if (result == null) return null;

            return new QuestionFromAIOrDatabaseDto
            {
                Question = result.IntermediateQuestionQuestion,
                AppropriateAnswer = result.IntermediateQuestionAnswer
            };
        }

        public async Task<QuestionFromAIOrDatabaseDto> GetAdvancedSpecialtyQuestionById(String specialty)
        {
            var result = await _context.AdvancedQuestions
               .Where(q => q.AdvancedQuestionSpecialty == specialty)
               .OrderBy(q => Guid.NewGuid()) // This will randomize the results
               .FirstOrDefaultAsync();

            if (result == null) return null;

            return new QuestionFromAIOrDatabaseDto
            {
                Question = result.AdvancedQuestionQuestion,
                AppropriateAnswer = result.AdvancedQuestionAnswer
            };
        }

        public async Task<String?> GetFullNameByID(String id)
        {
            var testObeject = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == id);

            return testObeject.IntervieweeFirstName + " " + testObeject.IntervieweeMiddleName + " " + testObeject.IntervieweeLastName;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }



    }
}