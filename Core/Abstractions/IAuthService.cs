using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProduct.Core.Dtos;

namespace UserProduct.Core.Abstractions
{
    public interface IAuthService
    {
        public Task<Result> Register(RegisterDto registerUserDto);
        public Task<Result<LoginResponseDto>> Login(LoginUserDto loginUserDto);
        public Task<Result> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        public Task<Result> ForgotPassword(ResetPasswordDto resetPasswordDto);
        public Task<Result> ConfirmEmail(string email, string token);
    }
}
