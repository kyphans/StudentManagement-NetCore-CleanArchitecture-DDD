using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Events;

public record CourseCompletedEvent(
    StudentId StudentId,
    Guid CourseId,
    Guid EnrollmentId,
    DateTime CompletionDate,
    string FinalGrade,
    decimal GradePoints
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}