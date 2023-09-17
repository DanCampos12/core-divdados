using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Core.Divdados.Infra.SQL.DataContext.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyDecimalTypeDefault(this ModelBuilder modelBuilder) =>
        modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .ToList()
            .ForEach(property =>
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    property.SetColumnType("numeric(20, 8)");
            });

    public static void ApplyDateTypeDefault(this ModelBuilder modelBuilder) =>
        modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .ToList()
            .ForEach(property =>
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetColumnType("timestamp");
            });
}
