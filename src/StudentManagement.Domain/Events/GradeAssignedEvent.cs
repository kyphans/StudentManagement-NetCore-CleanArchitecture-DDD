using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Domain.Events;

public record GradeAssignedEvent(
    StudentId StudentId,
    Guid CourseId,
    Guid EnrollmentId,
    Guid GradeId,
    string LetterGrade,
    decimal GradePoints,
    string GradedBy
) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}