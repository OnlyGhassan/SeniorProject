using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenioProject.Models.DTO.BothSideDto
{
    public class SuperListDto
    {
        public String FullName { get; set; }
       public String Specialty { get; set; }
       public string IntervieweeId { get; set; }
       public List<SuperDto> SuperList { get; set; }
    }
}