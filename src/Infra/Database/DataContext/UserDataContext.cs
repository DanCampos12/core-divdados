using Microsoft.EntityFrameworkCore;
using Core.Divdados.Domain.UserContext.Entities;
using System;
using Flunt.Notifications;
using Core.Divdados.Infra.SQL.DataContext.Extensions;

namespace Core.Divdados.Infra.SQL.DataContext;

public class UserDataContext : DbContext   
{
    #region DbSets

    public virtual DbSet<User> Users { get; set; }

    #endregion

    public string ConnectionString { get; set; }

    public UserDataContext(DbContextOptions<UserDataContext> options, string connectionString) : base(options)
    {
        ConnectionString = connectionString;
        Database.SetCommandTimeout(TimeSpan.FromMinutes(1));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyDecimalTypeDefault();
        modelBuilder.ApplyDateTypeDefault();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDataContext).Assembly);
        modelBuilder.Ignore<Notification>();
    }
}
