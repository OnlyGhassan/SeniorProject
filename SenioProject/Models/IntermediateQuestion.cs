using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

[Table("IntermediateQuestion")]
public partial class IntermediateQuestion
{
    [Key]
    [Column("IntermediateQuestion_ID")]
    public int IntermediateQuestionId { get; set; }

    [Column("IntermediateQuestion_SPECIALTY")]
    [StringLength(100)]
    public string? IntermediateQuestionSpecialty { get; set; }

    [Column("IntermediateQuestion_QUESTION")]
    [StringLength(1000)]
    public string IntermediateQuestionQuestion { get; set; } = null!;

    [Column("IntermediateQuestion_ANSWER")]
    [StringLength(1000)]
    public string IntermediateQuestionAnswer { get; set; } = null!;

    [ForeignKey("IntermediateQuestionId")]
    [InverseProperty("IntermediateQuestions")]
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
