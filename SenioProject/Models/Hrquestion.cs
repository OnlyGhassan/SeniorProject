using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

[Table("HRQuestion")]
public partial class Hrquestion
{
    [Key]
    [Column("HRQuestion_ID")]
    public int HrquestionId { get; set; }

    [Column("HRQuestion_QUESTION")]
    [StringLength(1000)]
    public string HrquestionQuestion { get; set; } = null!;

    [ForeignKey("HrquestionId")]
    [InverseProperty("Hrquestions")]
    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}
