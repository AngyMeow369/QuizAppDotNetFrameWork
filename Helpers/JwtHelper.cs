using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

public static class JwtHelper
{
    public static int GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == "UserId");
            return int.Parse(userIdClaim.Value);
        }
        catch
        {
            return -1;
        }
    }

    public static string GetUsernameFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
        }
        catch
        {
            return null;
        }
    }

    public static string GetRoleFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
        }
        catch
        {
            return null;
        }
    }
}