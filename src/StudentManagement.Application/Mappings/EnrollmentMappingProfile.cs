using AutoMapper;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Mappings;

public class EnrollmentMappingProfile : Profile
{
    public EnrollmentMappingProfile()
    {
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId.Value))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Grade, opt => opt.MapFrom(src => src.Grade));

        CreateMap<Enrollment, EnrollmentSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId.Value))
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.FinalGrade, opt => opt.MapFrom(src => src.Grade != null ? src.Grade.LetterGrade : null))
            .ForMember(dest => dest.GradePoints, opt => opt.MapFrom(src => src.Grade != null ? src.Grade.GradePoints : (decimal?)null))
            .ForMember(dest => dest.StudentName, opt => opt.Ignore())
            .ForMember(dest => dest.CourseCode, opt => opt.Ignore())
            .ForMember(dest => dest.CourseName, opt => opt.Ignore());

        CreateMap<Enrollment, EnrollmentWithDetailsDto>()
            .IncludeBase<Enrollment, EnrollmentDto>()
            .ForMember(dest => dest.Student, opt => opt.Ignore())
            .ForMember(dest => dest.Course, opt => opt.Ignore());

        CreateMap<CreateEnrollmentDto, Enrollment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StudentId, opt => opt.Ignore())
            .ForMember(dest => dest.CourseId, opt => opt.Ignore())
            .ForMember(dest => dest.EnrollmentDate, opt => opt.Ignore())
            .ForMember(dest => dest.CompletionDate, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Grade, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}