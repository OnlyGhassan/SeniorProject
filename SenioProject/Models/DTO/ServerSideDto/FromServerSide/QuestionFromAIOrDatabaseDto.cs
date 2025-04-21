using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.ServerSideDto.FromServerSide
{
    public class QuestionFromAIOrDatabaseDto
    {
        public string Question { get; set; }
        public String? AppropriateAnswer { get; set; }
    }
}