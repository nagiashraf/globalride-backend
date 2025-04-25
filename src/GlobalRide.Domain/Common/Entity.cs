namespace GlobalRide.Domain.Common;

/// <summary>
/// Represents a base class for entities.
/// </summary>
/// <typeparam name="TId">The type of the entity's unique identifier. Must be a non-nullable type.</typeparam>
public abstract class Entity<TId>
    where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TId}"/> class with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier for the entity.</param>
    protected Entity(TId id) => Id = id;

    /// <summary>
    /// Gets the identifier for the entity.
    /// </summary>
    public TId Id { get; private set; } = default!;

    /// <summary>
    /// Determines whether the specified object is equal to the current object based on reference and identifier equality.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (Id.Equals(default(TId)) == default || other.Id.Equals(default(TId)))
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }
}
