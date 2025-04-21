using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.ServerSideDto.ToServerSide
{
    public class GenerateQuestionDto
    {
        public String Specialty { get; set; }
        public String QuestionDifficulty { get; set; }
        public int NumberOfQuestions { get; set; }
    }
}