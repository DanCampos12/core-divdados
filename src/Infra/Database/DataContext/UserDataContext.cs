using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Infra.SQL.DataContext.Extensions;
using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using System;

namespace Core.Divdados.Infra.SQL.DataContext;

public class UserDataContext : DbContext   
{
    #region DbSets

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

    #endregion

    public string ConnectionString { get; set; }

    public UserDataContext(DbContextOptions<UserDataContext> options, string connectionString) : base(options)
    {
        ConnectionString = connectionString;
        Database.SetCommandTimeout(TimeSpan.FromMinutes(1));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyDecimalTypeDefault();
        modelBuilder.ApplyDateTypeDefault();
        modelBuilder.Ignore<Notification>();
    }
}
