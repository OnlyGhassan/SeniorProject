using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

[Table("AdvancedQuestion")]
public partial class AdvancedQuestion
{
    [Key]
    [Column("AdvancedQuestion_ID")]
    public int AdvancedQuestionId { get; set; }

    [Column("AdvancedQuestion_SPECIALTY")]
    [StringLength(100)]
    public string? AdvancedQuestionSpecialty { get; set; }

    [Column("AdvancedQuestion_QUESTION")]
    [StringLength(1000)]
    public string AdvancedQuestionQuestion { get; set; } = null!;

    [Column("AdvancedQuestion_ANSWER")]
    [StringLength(1000)]
    public string AdvancedQuestionAnswer { get; set; } = null!;

    [ForeignKey("AdvancedQuestionId")]
    [InverseProperty("AdvancedQuestions")]
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
