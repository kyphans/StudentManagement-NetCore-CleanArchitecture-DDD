using AutoMapper;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Mappings;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value))
            .ForMember(dest => dest.Prerequisites, opt => opt.MapFrom(src => src.Prerequisites.ToList()));

        CreateMap<Course, CourseSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code.Value))
            .ForMember(dest => dest.CanEnroll, opt => opt.MapFrom(src => src.CurrentEnrollmentCount < src.MaxEnrollment && src.IsActive));

        CreateMap<Course, CourseWithEnrollmentsDto>()
            .IncludeBase<Course, CourseDto>()
            .ForMember(dest => dest.Enrollments, opt => opt.Ignore());

        CreateMap<Course, CourseWithPrerequisitesDto>()
            .IncludeBase<Course, CourseDto>()
            .ForMember(dest => dest.PrerequisiteCourses, opt => opt.Ignore());

        CreateMap<CreateCourseDto, Course>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Code, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentEnrollmentCount, opt => opt.Ignore())
            .ForMember(dest => dest.Prerequisites, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}