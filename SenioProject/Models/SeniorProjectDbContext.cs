using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SenioProject.Models;

public partial class SeniorProjectDbContext : DbContext
{
    public SeniorProjectDbContext()
    {
    }

    public SeniorProjectDbContext(DbContextOptions<SeniorProjectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdvancedQuestion> AdvancedQuestions { get; set; }

    public virtual DbSet<BeginnerQuestion> BeginnerQuestions { get; set; }

    public virtual DbSet<Hrquestion> Hrquestions { get; set; }

    public virtual DbSet<IntermediateQuestion> IntermediateQuestions { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<Interviewee> Interviewees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Ghassan");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdvancedQuestion>(entity =>
        {
            entity.HasKey(e => e.AdvancedQuestionId).HasName("PK__Advanced__26BD6FB0E8AF6899");
        });

        modelBuilder.Entity<BeginnerQuestion>(entity =>
        {
            entity.HasKey(e => e.BeginnerQuestionsId).HasName("PK__Beginner__37027F3BE2886C5B");
        });

        modelBuilder.Entity<Hrquestion>(entity =>
        {
            entity.HasKey(e => e.HrquestionId).HasName("PK__HRQuesti__C23949340364312B");
        });

        modelBuilder.Entity<IntermediateQuestion>(entity =>
        {
            entity.HasKey(e => e.IntermediateQuestionId).HasName("PK__Intermed__E61F26B4D61D9335");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__536D7219EDA3A3B7");

            entity.HasOne(d => d.Interviewee).WithMany(p => p.Interviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Interview_Interviewee");

            entity.HasMany(d => d.AdvancedQuestions).WithMany(p => p.Interviews)
                .UsingEntity<Dictionary<string, object>>(
                    "InterviewAdvancedQuestion",
                    r => r.HasOne<AdvancedQuestion>().WithMany()
                        .HasForeignKey("AdvancedQuestionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IAQ_AdvancedQuestion"),
                    l => l.HasOne<Interview>().WithMany()
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IAQ_Interview"),
                    j =>
                    {
                        j.HasKey("InterviewId", "AdvancedQuestionId");
                        j.ToTable("Interview_AdvancedQuestion");
                        j.IndexerProperty<int>("InterviewId").HasColumnName("Interview_ID");
                        j.IndexerProperty<int>("AdvancedQuestionId").HasColumnName("AdvancedQuestion_ID");
                    });

            entity.HasMany(d => d.BeginnerQuestions).WithMany(p => p.Interviews)
                .UsingEntity<Dictionary<string, object>>(
                    "InterviewBeginnerQuestion",
                    r => r.HasOne<BeginnerQuestion>().WithMany()
                        .HasForeignKey("BeginnerQuestionsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IEQ_BeginnerQuestions"),
                    l => l.HasOne<Interview>().WithMany()
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IEQ_Interview"),
                    j =>
                    {
                        j.HasKey("InterviewId", "BeginnerQuestionsId");
                        j.ToTable("Interview_BeginnerQuestions");
                        j.IndexerProperty<int>("InterviewId").HasColumnName("Interview_ID");
                        j.IndexerProperty<int>("BeginnerQuestionsId").HasColumnName("BeginnerQuestions_ID");
                    });

            entity.HasMany(d => d.Hrquestions).WithMany(p => p.Interviews)
                .UsingEntity<Dictionary<string, object>>(
                    "InterviewHrquestion",
                    r => r.HasOne<Hrquestion>().WithMany()
                        .HasForeignKey("HrquestionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IHQ_HRQuestion"),
                    l => l.HasOne<Interview>().WithMany()
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IHQ_Interview"),
                    j =>
                    {
                        j.HasKey("InterviewId", "HrquestionId");
                        j.ToTable("Interview_HRQuestion");
                        j.IndexerProperty<int>("InterviewId").HasColumnName("Interview_ID");
                        j.IndexerProperty<int>("HrquestionId").HasColumnName("HRQuestion_ID");
                    });

            entity.HasMany(d => d.IntermediateQuestions).WithMany(p => p.Interviews)
                .UsingEntity<Dictionary<string, object>>(
                    "InterviewIntermediateQuestion",
                    r => r.HasOne<IntermediateQuestion>().WithMany()
                        .HasForeignKey("IntermediateQuestionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IIQ_IntermediateQuestion"),
                    l => l.HasOne<Interview>().WithMany()
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_IIQ_Interview"),
                    j =>
                    {
                        j.HasKey("InterviewId", "IntermediateQuestionId");
                        j.ToTable("Interview_IntermediateQuestion");
                        j.IndexerProperty<int>("InterviewId").HasColumnName("Interview_ID");
                        j.IndexerProperty<int>("IntermediateQuestionId").HasColumnName("IntermediateQuestion_ID");
                    });
        });

        modelBuilder.Entity<Interviewee>(entity =>
        {
            entity.HasKey(e => e.IntervieweeId).HasName("PK__Intervie__95F25838C820F6D7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
