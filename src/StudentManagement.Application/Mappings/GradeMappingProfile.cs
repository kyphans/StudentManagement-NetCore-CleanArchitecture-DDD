using AutoMapper;
using StudentManagement.Application.DTOs;
using StudentManagement.Domain.Entities;

namespace StudentManagement.Application.Mappings;

public class GradeMappingProfile : Profile
{
    public GradeMappingProfile()
    {
        CreateMap<Grade, GradeDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<Grade, GradeSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<CreateGradeFromLetterDto, Grade>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GradedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsPassing, opt => opt.Ignore())
            .ForMember(dest => dest.IsHonorGrade, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<CreateGradeFromNumericDto, Grade>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.LetterGrade, opt => opt.Ignore())
            .ForMember(dest => dest.GradePoints, opt => opt.Ignore())
            .ForMember(dest => dest.GradedDate, opt => opt.Ignore())
            .ForMember(dest => dest.IsPassing, opt => opt.Ignore())
            .ForMember(dest => dest.IsHonorGrade, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<UpdateGradeDto, Grade>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.GradedDate, opt => opt.Ignore())
            .ForMember(dest => dest.GradedBy, opt => opt.Ignore())
            .ForMember(dest => dest.IsPassing, opt => opt.Ignore())
            .ForMember(dest => dest.IsHonorGrade, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}