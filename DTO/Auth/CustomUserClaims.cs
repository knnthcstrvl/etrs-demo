namespace Financial.DTO.Auth
{
    public record CustomUserClaims(string Id = null!, string Name = null!, string Username = null!, string Role = null!, string Jwt = null!, string Expiry = null!, string DivisionId = null!);
}
