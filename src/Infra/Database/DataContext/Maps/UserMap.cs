using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Infra.SQL.DataContext.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Core.Divdados.Infra.SQL.DataContext.Maps;

public class UserMap : ClassMap<User>
{
    public UserMap() : base("Users") { }

    protected override void MappingKeys(EntityTypeBuilder<User> builder) =>
        builder.HasKey(e => e.Id);

    protected override void MappingProperties(EntityTypeBuilder<User> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(50).HasColumnType("varchar");
        builder.Property(e => e.BirthDate).HasColumnType("datetime");
        builder.Property(e => e.Sex).HasMaxLength(1).HasColumnType("char");
        builder.Property(e => e.Email).HasMaxLength(100).HasColumnType("varchar");
        builder.Property(e => e.Password).HasMaxLength(100).HasColumnType("varchar");
    }
}
