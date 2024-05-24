using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserProduct.Core.Abstractions;
using UserProduct.Core.Dtos;
using UserProduct.Domain.Constants;
using UserProduct.Domain.Entities;

namespace UserProduct.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<User> _userManager;
        private readonly IRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<User> userManager, IRepository repository, IJwtService jwtService,
            IEmailService emailService, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _repository = repository;
            _jwtService = jwtService;
            _emailService = emailService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Register(RegisterDto registerDto)
        {
            var emailExist = await _userManager.FindByEmailAsync(registerDto.Email);

            if (emailExist != null)
                return new Error[] { new("Registration.Error", "Email already exists") };

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return result.Errors.Select(error => new Error(error.Code, error.Description)).ToArray();

            result = await _userManager.AddToRoleAsync(user, RolesConstant.User);
            if (!result.Succeeded)
                return result.Errors.Select(error => new Error(error.Code, error.Description)).ToArray();

            var isSaved = await _unitOfWork.SaveChangesAsync() > 0;
            if (!isSaved)
                return new Error[] { new("User.NotSaved", "User not saved") };

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink =
                $"{_configuration["ConfirmationUrl"]}?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";

            const string emailSubject = "Confirm Your Email";

            var emailBody = $"Hello {user.FirstName}, please confirm your email by clicking this link: {confirmationLink}.";

            var isEmailSent = await _emailService.SendEmailAsync(registerDto.Email, emailSubject, emailBody);
            if (!isEmailSent)
                return new Error[] { new("Auth.Error", "Error occurred while sending confirmation email") };

            return Result.Success();
        }

        public async Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto)
        {
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);

            if (user is null)
                return new Error[] { new("Auth.Error", "email or password not correct") };

            var isValidUser = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);

            if (!isValidUser)
                return new Error[] { new("Auth.Error", "email or password not correct") };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            return new LoginResponseDto(token, roles.First());
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
                return new Error[] { new("Auth.Error", "No user found with the provided email") };

            var resetPasswordResult =
                await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!resetPasswordResult.Succeeded)
                return resetPasswordResult.Errors.Select(error => new Error(error.Code, error.Description)).ToArray();

            return Result.Success();
        }

        public async Task<Result> ForgotPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
                return new Error[] { new("Auth.Error", "No user found with the provided email") };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink =
                $"{_configuration["ResetPasswordUrl"]}?email={HttpUtility.UrlEncode(user.Email)}&token={HttpUtility.UrlEncode(token)}";

            const string emailSubject = "Your New Password";

            var emailBody = $"Hello {user.FirstName}, click this link to reset your password: {resetLink}.";

            var isSuccessful = await _emailService.SendEmailAsync(resetPasswordDto.Email, emailSubject, emailBody);
            if (!isSuccessful)
                return new Error[] { new("Auth.Error", "Error occured while sending reset password email") };

            return Result.Success();
        }

        public async Task<Result> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return new Error[] { new("Auth.Error", "User not found") };

            var confirmEmailResult = await _userManager.ConfirmEmailAsync(user, token);

            if (!confirmEmailResult.Succeeded)
            {
                return Result.Failure(confirmEmailResult.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
            }

            user.EmailConfirmed = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result.Failure(updateResult.Errors.Select(e => new Error(e.Code, e.Description)).ToArray());
            }

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new Error[] { new("Auth.Error", "email not correct") };

            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                return new Error[] { new("Auth.Error", "password not correct") };

            if (model.NewPassword != model.ConfirmPassword)
                return new Error[] { new("Auth.Error", "Newpassword and Confirmpassword must match") };

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return result.Errors.Select(error => new Error(error.Code, error.Description)).ToArray();

            return Result.Success();

        }
    }
}
