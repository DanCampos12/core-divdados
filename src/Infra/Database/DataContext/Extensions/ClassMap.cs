using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Divdados.Infra.SQL.DataContext.Extensions;

public abstract class ClassMap<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : class
{
    private readonly string _tableName;

    protected ClassMap(string tableName) => _tableName = tableName;

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        MappingKeys(builder);
        MappingProperties(builder);
        builder.ToTable(_tableName);
    }

    protected abstract void MappingProperties(EntityTypeBuilder<TEntity> builder);

    protected abstract void MappingKeys(EntityTypeBuilder<TEntity> builder);
}
