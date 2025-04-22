using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SenioProject.Models;
using SenioProject.Models.DTO.BothSideDto;
using SenioProject.Models.DTO.FromVRSideDto;
using SenioProject.Models.DTO.ServerSideDto.ToServerSide;

namespace SenioProject.Mappers
{
    public static class GeneralMappers
    {
        public static Interviewee FromCreateIntervieweeDtoToInterviewee(this CreateIntervieweeDto createIntervieweeDto){
            return new Interviewee
            {
                IntervieweeId = createIntervieweeDto.IntervieweeId,
                IntervieweeFirstName = createIntervieweeDto.IntervieweeFirstName,
                IntervieweeMiddleName = createIntervieweeDto.IntervieweeMiddleName,
                IntervieweeLastName = createIntervieweeDto.IntervieweeLastName,
                IntervieweeCvText = createIntervieweeDto.IntervieweeCvText
            };
        }

         public static Interview FromStartIntervieweeDtoToInterview(this StartInterviewDto startInterviewDto){
            return new Interview
            {
                // InterviewId is auto incremented in the database!
                InterviewReportText = String.Empty,
                InterviewSpecialty = startInterviewDto.Specialty,
                IntervieweeId = startInterviewDto.IntervieweeId
            };
        }


        public static GenerateQuestionDto ToGenerateQuestionDto(this StartInterviewDto startInterviewDto,int publicModelQuestionNumber){
            return new GenerateQuestionDto
            {
                // InterviewId is auto incremented in the database!
                Specialty = startInterviewDto.Specialty,
                QuestionDifficulty = startInterviewDto.QuestionDifficulty,
                NumberOfQuestions = publicModelQuestionNumber
            };
        }

        public static CVDto ToCVDto(this Interviewee interviewee){
            return new CVDto
            {
                // InterviewId is auto incremented in the database!
                IntervieweeCvText = interviewee.IntervieweeCvText
            };
        }

        public static Interview ToInterview(this EvaluationReportDto evaluationReportDto, String specialty, String intervieweeId){
            return new Interview
            {
                // InterviewId is auto incremented in the database!
                InterviewReportText = evaluationReportDto.EvaluationReport,
                InterviewSpecialty = specialty,
                IntervieweeId = intervieweeId
            };
        }



    }

   
}