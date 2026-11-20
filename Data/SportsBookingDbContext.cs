using Microsoft.EntityFrameworkCore;
using SportsBookingApp.Models;

namespace SportsBookingApp.Data;

public class SportsBookingDbContext : DbContext
{
    public SportsBookingDbContext(DbContextOptions<SportsBookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<FacilityType> FacilityTypes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Sport> Sports { get; set; }
    public DbSet<Inquiry> Inquiries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ================= MAP EXACT TABLE NAMES =================
        modelBuilder.Entity<Member>().ToTable("Member");
        modelBuilder.Entity<Facility>().ToTable("Facility");
        modelBuilder.Entity<FacilityType>().ToTable("FacilityType");
        modelBuilder.Entity<Booking>().ToTable("Booking");
        modelBuilder.Entity<Review>().ToTable("Review");
        modelBuilder.Entity<Sport>().ToTable("Sport");
        modelBuilder.Entity<Inquiry>().ToTable("Inquiry");

        // ================= PRIMARY KEYS =================
        modelBuilder.Entity<Member>().HasKey(m => m.MemberID);
        modelBuilder.Entity<Facility>().HasKey(f => f.FacilityID);
        modelBuilder.Entity<FacilityType>().HasKey(ft => ft.FacilityTypeID);
        modelBuilder.Entity<Booking>().HasKey(b => b.BookingID);
        modelBuilder.Entity<Review>().HasKey(r => r.ReviewID);
        modelBuilder.Entity<Sport>().HasKey(s => s.SportID);
        modelBuilder.Entity<Inquiry>().HasKey(i => i.InquiryID);

        // ================= COLUMN CONFIGURATION =================
        modelBuilder.Entity<Facility>()
            .Property(f => f.HourlyRate)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");

        // ================= IGNORE COMPUTED / JOINED HELPER PROPERTIES =================
        // Facility joined/calculated helpers
        modelBuilder.Entity<Facility>().Ignore(f => f.TypeName);
        modelBuilder.Entity<Facility>().Ignore(f => f.AvgRating);
        modelBuilder.Entity<Facility>().Ignore(f => f.ReviewCount);

        // Booking joined/calculated helpers
        modelBuilder.Entity<Booking>().Ignore(b => b.MemberName);
        modelBuilder.Entity<Booking>().Ignore(b => b.MemberEmail);
        modelBuilder.Entity<Booking>().Ignore(b => b.FacilityName);
        modelBuilder.Entity<Booking>().Ignore(b => b.Location);
        modelBuilder.Entity<Booking>().Ignore(b => b.HourlyRate);
        modelBuilder.Entity<Booking>().Ignore(b => b.TypeName);
        modelBuilder.Entity<Booking>().Ignore(b => b.Reference);
        modelBuilder.Entity<Booking>().Ignore(b => b.Hours);

        // Review joined helpers
        modelBuilder.Entity<Review>().Ignore(r => r.MemberName);
        modelBuilder.Entity<Review>().Ignore(r => r.FacilityName);
    }
}
