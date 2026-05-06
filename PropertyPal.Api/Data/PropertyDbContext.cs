using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropertyPal.Api.Models;
using System;
using System.Collections.Generic;


namespace PropertyPal.Api.Data;

public partial class PropertyDbContext : IdentityDbContext<ApplicationUser>
{
   

    public PropertyDbContext(DbContextOptions<PropertyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Lease> Leases { get; set; }

    public virtual DbSet<MaintenanceAttachment> MaintenanceAttachments { get; set; }

    public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

    public virtual DbSet<MaintenanceUpdate> MaintenanceUpdates { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<StaffSkill> StaffSkills { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<UnitImage> UnitImages { get; set; }

   // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
     //   => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PropertyDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //added to create identity tables 

        // Relationships to AspNetUsers (using the same column names as scaffolded)
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Lease>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(l => l.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(m => m.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(m => m.AssignedStaffId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MaintenanceUpdate>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(mu => mu.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StaffSkill>(entity =>
        {
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(ss => ss.StaffId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__Amenitie__842AF50B54EE520E");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Applicat__C93A4C99E9E00BEB");

            entity.HasIndex(e => e.TenantId, "IX_Applications_TenantId");

            entity.HasIndex(e => e.UnitId, "IX_Applications_UnitId");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Unit).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Applications_Units");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDD627FE7AC4");

            entity.ToTable("Feedback");

            entity.HasIndex(e => e.RequestId, "IX_Feedback_RequestId");

            entity.HasIndex(e => e.TenantId, "IX_Feedback_TenantId");

            entity.HasOne(d => d.Request).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_Feedback_MaintenanceRequests");
        });

        modelBuilder.Entity<Lease>(entity =>
        {
            entity.HasKey(e => e.LeaseId).HasName("PK__Leases__21FA58C10C15B77C");

            entity.HasIndex(e => e.TenantId, "IX_Leases_TenantId");

            entity.HasIndex(e => e.UnitId, "IX_Leases_UnitId");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Unit).WithMany(p => p.Leases)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_Leases_Units");
        });

        modelBuilder.Entity<MaintenanceAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__Maintena__442C64BE1EA89ECB");

            entity.HasIndex(e => e.RequestId, "IX_MaintenanceAttachments_RequestId");

            entity.HasOne(d => d.Request).WithMany(p => p.MaintenanceAttachments)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_MaintenanceAttachments_MaintenanceRequests");
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Maintena__33A8517AEF339621");

            entity.HasIndex(e => e.AssignedStaffId, "IX_MaintenanceRequests_AssignedStaffId");

            entity.HasIndex(e => new { e.Status, e.Priority }, "IX_MaintenanceRequests_Status_Priority");

            entity.HasIndex(e => e.TenantId, "IX_MaintenanceRequests_TenantId");

            entity.HasIndex(e => e.UnitId, "IX_MaintenanceRequests_UnitId");

            entity.HasIndex(e => e.TicketNumber, "UQ__Maintena__CBED06DA532A45EB").IsUnique();

            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.TicketNumber).HasMaxLength(50);

            entity.HasOne(d => d.Unit).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_MaintenanceRequests_Units");
        });

        modelBuilder.Entity<MaintenanceUpdate>(entity =>
        {
            entity.HasKey(e => e.UpdateId).HasName("PK__Maintena__7A0CF3C5C52ABA28");

            entity.HasIndex(e => e.RequestId, "IX_MaintenanceUpdates_RequestId");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);

            entity.HasOne(d => d.Request).WithMany(p => p.MaintenanceUpdates)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_MaintenanceUpdates_MaintenanceRequests");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1266919A12");

            entity.HasIndex(e => e.RequestId, "IX_Notifications_RequestId");

            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Request).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notifications_MaintenanceRequests");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A384E409EE6");

            entity.HasIndex(e => e.LeaseId, "IX_Payments_LeaseId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.PaidDate).HasColumnType("datetime");

            entity.HasOne(d => d.Lease).WithMany(p => p.Payments)
                .HasForeignKey(d => d.LeaseId)
                .HasConstraintName("FK_Payments_Leases");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PK__Properti__70C9A735FC8164CB");

            entity.HasIndex(e => e.ManagerId, "IX_Properties_ManagerId");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skills__DFA091877740A35A");
        });

        modelBuilder.Entity<StaffSkill>(entity =>
        {
            entity.HasKey(e => new { e.StaffId, e.SkillId }).IsClustered(false);

            entity.HasIndex(e => e.SkillId, "IX_StaffSkills_SkillId");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__Units__44F5ECB5C790A4F7");

            entity.HasIndex(e => e.PropertyId, "IX_Units_PropertyId");

            entity.Property(e => e.RentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Size).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Property).WithMany(p => p.Units)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK_Units_Properties");

            entity.HasMany(d => d.Amenities).WithMany(p => p.Units)
                .UsingEntity<Dictionary<string, object>>(
                    "UnitAmenity",
                    r => r.HasOne<Amenity>().WithMany()
                        .HasForeignKey("AmenityId")
                        .HasConstraintName("FK_UnitAmenities_Amenities"),
                    l => l.HasOne<Unit>().WithMany()
                        .HasForeignKey("UnitId")
                        .HasConstraintName("FK_UnitAmenities_Units"),
                    j =>
                    {
                        j.HasKey("UnitId", "AmenityId");
                        j.ToTable("UnitAmenities");
                    });
        });

        modelBuilder.Entity<UnitImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__UnitImag__7516F70C68E38D06");

            entity.HasOne(d => d.Unit).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.UnitId)
                .HasConstraintName("FK_UnitImages_Units");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
