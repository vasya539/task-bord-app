namespace WebApi.Data.DTOs.AccountDtos
{ 
    public class PasswordRestoreDto:ChangePasswordDto
    {
        public string RestoreToken { get; set; }
    }
}
