namespace Financial.DTO.Auth
{
    public record CustomUserClaims(string Id = null!, string FirstName = null!, string MiddleName = null!, string LastName = null!, string Email = null!, string Role = null!, string Jwt = null!, string Expiry = null!, string DivisionId = null!);
}
