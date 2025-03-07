namespace AKhderApi.DTOs.AuthDtos
{
    public class ResetPasswordDto
    {
        public string TempToken { get; set; }
        public string NewPassword { get; set; }
    }
}
