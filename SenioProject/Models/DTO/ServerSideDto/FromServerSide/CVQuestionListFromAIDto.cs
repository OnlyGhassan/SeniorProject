using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SenioProject.Models.DTO.ServerSideDto.FromServerSide
{
    public class CVQuestionListFromAIDto
    {
        [JsonProperty("Question")]
        public List<String> CVQuestionListFromAI { get; set; } 
    }
}