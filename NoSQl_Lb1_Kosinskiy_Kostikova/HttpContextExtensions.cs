using System.Security.Claims;

namespace NoSQl_Lb1_Kosinskiy_Kostikova
{
    public static class HttpContextExtensions
    {
        public static string GetNameFromToken(this HttpContext context)
        {
            Claim userClaim = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return userClaim?.Value;
        }
    }
}
