using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SenioProject.Models;
using SenioProject.Models.DTO.FromVRSideDto;

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



    }

   
}