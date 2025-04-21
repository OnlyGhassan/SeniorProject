using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.ServerSideDto.FromServerSide
{
    public class QuestionListFromAIDto
    {
       public List<QuestionFromAIOrDatabaseDto> QuestionListFromAI { get; set; } 
    }
}