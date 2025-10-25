namespace FinTrack360.Application.Features.Auth.ChangePassword;

public record ChangePasswordDto(string OldPassword, string NewPassword, string ConfirmPassword);
