namespace CarServiceBG.DTOs
{
    public sealed record ChangePasswordUsingTokenDto(
        string Email,
        string NewPassword,
        string Token);


}
