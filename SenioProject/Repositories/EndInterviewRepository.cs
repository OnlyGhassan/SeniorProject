using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SenioProject.Models;

namespace SenioProject.Repositories
{
    public class EndInterviewRepository
    {
        private readonly SeniorProjectDbContext _context;

        public EndInterviewRepository(SeniorProjectDbContext context)
        {
            _context = context;
        }

        public async Task CreateIntervieweeAsync(Interview newIntrview)
        {
            // var testObeject = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == newIntrviewee.IntervieweeId);
            // if(testObeject != null)
            // return null;

            await _context.Interviews.AddAsync(newIntrview);
            await _context.SaveChangesAsync();
            
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}