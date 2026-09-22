using BaseLibrary.DTOs;
using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAccount(RegisterDto model);
        Task<LoginResponse> SignIn(LoginDto model);
        // Task<LoginResponse> RefreshToken(RefreshTokenDto model); // Plus tard
        Task<User?> GetUserByEmail(string email);
        Task<GeneralResponse> ChangePassword(int userId, ChangePasswordDto model);
        Task<GeneralResponse> ChangeEmail(int userId, ChangeEmailDto model);
        Task<LoginResponse> SignInWithGoogle(GoogleLoginDto model);
        Task<GeneralResponse> VerifyEmail(string token);
    }
}
