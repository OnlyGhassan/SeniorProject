using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

public partial class BeginnerQuestion
{
    [Key]
    [Column("BeginnerQuestions_ID")]
    public int BeginnerQuestionsId { get; set; }

    [Column("BeginnerQuestions_SPECIALTY")]
    [StringLength(100)]
    public string? BeginnerQuestionsSpecialty { get; set; }

    [Column("BeginnerQuestions_QUESTION")]
    [StringLength(1000)]
    public string BeginnerQuestionsQuestion { get; set; } = null!;

    [Column("BeginnerQuestions_ANSWER")]
    [StringLength(1000)]
    public string BeginnerQuestionsAnswer { get; set; } = null!;

    [ForeignKey("BeginnerQuestionsId")]
    [InverseProperty("BeginnerQuestions")]
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
