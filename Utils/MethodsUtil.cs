using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace provasemestral.Utils
{
    public class MethodsUtil
    {
        public string GenerateToken(string data)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcabcabc"); //Esta chave sera gravada em uma variavel de ambiente
            var tokenDecriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDecriptor);

            return tokenHandler.WriteToken(token); //Converte para string
        }

        
            public async Task<bool> ValidateToken(HttpContext context)
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token não fornecido");
                    return false;
                }

                //Obter token
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                //Validar o token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("abcabcabcabcabcabcabcabcabcabcabcabc");
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken validatedToken;

                try
                {
                    tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                }
                catch (Exception e)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token inválido");
                    return false;
                }

                // Token válido
                await context.Response.WriteAsync("Token autorizado");
                return true;
            
        }
    }
}
