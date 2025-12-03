using AutoMapper;
using MediatR;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Common.Enums;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Handler cho RegisterCommand
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponseDto<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<ApiResponseDto<UserDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Kiểm tra username đã tồn tại chưa
            var username = Username.Create(request.Username);
            var isUsernameUnique = await _unitOfWork.Users.IsUsernameUniqueAsync(username, cancellationToken: cancellationToken);
            if (!isUsernameUnique)
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Username đã tồn tại" });
            }

            // 2. Kiểm tra email đã tồn tại chưa
            var email = Email.Create(request.Email);
            var isEmailUnique = await _unitOfWork.Users.IsEmailUniqueAsync(email, cancellationToken: cancellationToken);
            if (!isEmailUnique)
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Email đã tồn tại" });
            }

            // 3. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 4. Parse role
            if (!Enum.TryParse<UserRole>(request.Role, out var userRole))
            {
                return ApiResponseDto<UserDto>.ErrorResult(
                    new[] { "Role không hợp lệ" });
            }

            // 5. Tạo user entity
            var user = User.Create(
                username: request.Username,
                email: request.Email,
                passwordHash: passwordHash,
                firstName: request.FirstName,
                lastName: request.LastName,
                role: userRole
            );

            // 6. Thêm vào database
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Map sang DTO và return
            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(
                userDto,
                "Đăng ký thành công");
        }
        catch (ArgumentException ex)
        {
            return ApiResponseDto<UserDto>.ErrorResult(
                new[] { ex.Message });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<UserDto>.ErrorResult(
                new[] { "Có lỗi xảy ra khi đăng ký user", ex.Message });
        }
    }
}