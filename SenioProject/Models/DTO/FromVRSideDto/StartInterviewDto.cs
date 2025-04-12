using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.FromVRSideDto
{
    public class StartInterviewDto
    {
    public string IntervieweeId { get; set; }
    public string Specialty { get; set; }
    public string QuestionDifficulty { get; set; }
    public string QustionType { get; set; }
    }
}