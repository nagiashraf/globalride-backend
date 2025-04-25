namespace GlobalRide.Domain.Common;

/// <summary>
/// Represents a unit of work for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all changes made in the unit of work to the underlying data store. It should be called
    /// after completing a set of related operations to ensure data consistency.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains the
    /// number of state entries written to the underlying data store.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
