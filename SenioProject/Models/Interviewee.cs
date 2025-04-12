using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

[Table("Interviewee")]
public partial class Interviewee
{
    [Key]
    [Column("Interviewee_ID")]
    [StringLength(255)]
    public string IntervieweeId { get; set; } = null!;

    [Column("Interviewee_FIRST_NAME")]
    [StringLength(50)]
    public string IntervieweeFirstName { get; set; } = null!;

    [Column("Interviewee_MIDDLE_NAME")]
    [StringLength(50)]
    public string? IntervieweeMiddleName { get; set; }

    [Column("Interviewee_LAST_NAME")]
    [StringLength(50)]
    public string IntervieweeLastName { get; set; } = null!;

    [Column("Interviewee_CV_TEXT")]
    public string? IntervieweeCvText { get; set; }

    [InverseProperty("Interviewee")]
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
