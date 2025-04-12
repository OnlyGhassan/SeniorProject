using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

[Table("Interview")]
public partial class Interview
{
    [Key]
    [Column("Interview_ID")]
    public int InterviewId { get; set; }

    [Column("Interviewee_ID")]
    [StringLength(255)]
    public string IntervieweeId { get; set; } = null!;

    [Column("Interview_REPORT_TEXT")]
    public string? InterviewReportText { get; set; }

    [Column("Interview_SPECIALTY")]
    [StringLength(100)]
    public string? InterviewSpecialty { get; set; }

    [ForeignKey("IntervieweeId")]
    [InverseProperty("Interviews")]
    public virtual Interviewee Interviewee { get; set; } = null!;

    [ForeignKey("InterviewId")]
    [InverseProperty("Interviews")]
    public virtual ICollection<AdvancedQuestion> AdvancedQuestions { get; set; } = new List<AdvancedQuestion>();

    [ForeignKey("InterviewId")]
    [InverseProperty("Interviews")]
    public virtual ICollection<BeginnerQuestion> BeginnerQuestions { get; set; } = new List<BeginnerQuestion>();

    [ForeignKey("InterviewId")]
    [InverseProperty("Interviews")]
    public virtual ICollection<Hrquestion> Hrquestions { get; set; } = new List<Hrquestion>();

    [ForeignKey("InterviewId")]
    [InverseProperty("Interviews")]
    public virtual ICollection<IntermediateQuestion> IntermediateQuestions { get; set; } = new List<IntermediateQuestion>();
}
