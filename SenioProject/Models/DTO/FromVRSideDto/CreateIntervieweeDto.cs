using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.FromVRSideDto
{
    public class CreateIntervieweeDto
    {
        public string IntervieweeId { get; set; }
        public string IntervieweeFirstName { get; set; }
        public string? IntervieweeMiddleName { get; set; }
        public string IntervieweeLastName  { get; set; }
        public string? IntervieweeCvText { get; set; }
    }
}