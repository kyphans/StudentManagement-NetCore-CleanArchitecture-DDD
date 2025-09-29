using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Events;

public record StudentEnrolledEvent(
    StudentId StudentId,
    Guid CourseId,
    Guid EnrollmentId,
    DateTime EnrollmentDate,
    int CreditHours
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}