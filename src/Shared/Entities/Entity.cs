using Flunt.Notifications;
using System;

namespace Core.Divdados.Shared.Entities;

public abstract class Entity : Notifiable, IEquatable<Entity>
{
    public Entity() => Id = Guid.NewGuid();

    public Guid Id { get; private set; }

    public override bool Equals(object obj) =>
        Equals(obj as Entity);

    public bool Equals(Entity other)
    {
        if (other is null) return false;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() =>
        Id.GetHashCode();

    public static bool operator ==(Entity item1, Entity item2)
    {
        if (ReferenceEquals(item1, item2)) return true;
        if (item1 is null || item2 is null) return false;
        return item1.Equals(item2);
    }

    public static bool operator !=(Entity item1, Entity item2) =>
        !(item1 == item2);
}