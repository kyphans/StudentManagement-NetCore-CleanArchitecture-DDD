using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Interfaces;
using StudentManagement.Domain.Entities;
using StudentManagement.Domain.Repositories;
using StudentManagement.Domain.ValueObjects;

namespace StudentManagement.Application.Commands.Authentication;

/// <summary>
/// Handler cho LoginCommand
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponseDto<AuthenticationResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<ApiResponseDto<AuthenticationResponseDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Tìm user theo username VÀ load RefreshTokens ngay (CHỈ 1 QUERY)
            var username = Username.Create(request.Username);
            var user = await _unitOfWork.Users.GetByUsernameWithRefreshTokensAsync(username, cancellationToken);

            if (user == null)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Username hoặc password không đúng" });
            }

            // 2. Kiểm tra user có active không
            if (!user.IsActive)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Tài khoản đã bị khóa" });
            }

            // 3. Verify password
            var isPasswordValid = _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash.Value);

            if (!isPasswordValid)
            {
                return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                    new[] { "Username hoặc password không đúng" });
            }

            // 4. Generate access token
            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            // 5. Generate refresh token
            var refreshTokenString = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiryDays = _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays");

            var refreshToken = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenString,
                expiryDays: refreshTokenExpiryDays,
                createdByIp: request.IpAddress ?? "unknown"
            );

            // 6. Xóa các refresh tokens đã expired
            user.RemoveExpiredRefreshTokens();

            // 7. Thêm refresh token mới vào user
            user.AddRefreshToken(refreshToken);

            // 8. Update last login
            user.UpdateLastLogin();

            // 9. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Map sang DTO
            var userDto = _mapper.Map<UserDto>(user);
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

            var response = new AuthenticationResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
                User = userDto
            };

            return ApiResponseDto<AuthenticationResponseDto>.SuccessResult(
                response,
                "Đăng nhập thành công");
        }
        catch (ArgumentException ex)
        {
            return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                new[] { ex.Message });
        }
        catch (Exception ex)
        {
            return ApiResponseDto<AuthenticationResponseDto>.ErrorResult(
                new[] { "Có lỗi xảy ra khi đăng nhập", ex.Message });
        }
    }
}