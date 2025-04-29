using Microsoft.EntityFrameworkCore;
using SenioProject.Models;
using SenioProject.Models.DTO.FromVRSideDto;

namespace SenioProject.Repositories
{
    public class IntervieweeDataRepository
    {
        private readonly SeniorProjectDbContext _context;

        public IntervieweeDataRepository(SeniorProjectDbContext context)
        {
            _context = context;
        }

        public async Task<String?> CreateIntervieweeAsync(Interviewee newIntrviewee)
    {
        var testObeject = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == newIntrviewee.IntervieweeId);
        if(testObeject != null)
        return null;

        await _context.Interviewees.AddAsync(newIntrviewee);
        await _context.SaveChangesAsync();
        return String.Empty;
    }

    public async Task<Interviewee?> UpdateIntervieweeCVAsync(UpdateCVDto updateCV){

        var IntervieweeObjectToUpdateCV = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == updateCV.IntervieweeId);

        if(IntervieweeObjectToUpdateCV == null)
            return null;

        IntervieweeObjectToUpdateCV.IntervieweeCvText = updateCV.NewIntervieweeCvText;

        await _context.SaveChangesAsync();

        return IntervieweeObjectToUpdateCV;
    }

    public async Task<Interviewee?> DeleteIntervieweeCVAsync(DeleteIntervieweeDto deleteIntervieweeDto){
        

        while(true){

            var IntervieweeReleatedObjectTodelete = await _context.Interviews.FirstOrDefaultAsync(x => x.IntervieweeId == deleteIntervieweeDto.IntervieweeId);

            if (IntervieweeReleatedObjectTodelete == null)
            break;

            
            _context.Interviews.Remove(IntervieweeReleatedObjectTodelete);
            await _context.SaveChangesAsync();

        }


        var IntervieweeObjectTodelete = await _context.Interviewees.FirstOrDefaultAsync(x => x.IntervieweeId == deleteIntervieweeDto.IntervieweeId);

        if(IntervieweeObjectTodelete == null)
            return null;

        _context.Interviewees.Remove(IntervieweeObjectTodelete);

        await _context.SaveChangesAsync();

        return IntervieweeObjectTodelete;
    }

    //     public async Task<Product> GetByIdAsync(int id)
    // {
    //     return await _context.Products.FindAsync(id);
    // }
    }
}