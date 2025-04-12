using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.BothSideDto
{
    public class SuperDto
    {
        public int QuestionNumber { get; set; }
        public String Question { get; set; }
        public String AppropriateAnswer { get; set; }
        public String? CandidateAnswer { get; set; }
        public String QustionType { get; set; }
    }
}