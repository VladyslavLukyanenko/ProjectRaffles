using System;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public abstract class Entity : IEntity, IEquatable<Entity>
  {
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
      Id = id;
    }

    public Guid Id { get; protected set; }

    public bool Equals(Entity other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return Id.Equals(other.Id);
    }

    public override bool Equals(object obj)
    {
      return ReferenceEquals(this, obj) || obj is Entity other && Equals(other);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
      return !Equals(left, right);
    }
  }
}
