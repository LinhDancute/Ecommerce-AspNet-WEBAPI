using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly MyDBContext _context;
        private readonly AppSettings _appSettings;

        public CustomerController(MyDBContext context, IOptionsMonitor<AppSettings> optionsMonitor) {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Validate(LoginModel model)
        {
            var customer = _context.Customers.SingleOrDefault(c => c.Email == model.Email && model.Password == c.Password);
            if (customer == null) //incorrect
            {
                return Ok(new APIResponse
                {
                    Success = false,
                    Message = "Invalid username or password"
                });
            }

            //cap token
            var token = await GenerateToken(customer);

            return Ok(new APIResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = token
            }); 
        }
        private async Task<TokenModel> GenerateToken(CustomerData customer)
        {
            var JWTTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, customer.Name),
                    new Claim(JwtRegisteredClaimNames.Email, customer.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Address", customer.Address),
                    new Claim("ID", customer.CustomerID.ToString()),

                    //roles

                }),
                Expires = DateTime.UtcNow.AddSeconds(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha512Signature)
                
            };

            var token = JWTTokenHandler.CreateToken(tokenDescription);
            var accessToken = JWTTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            //save database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtID = token.Id,
                CustomerID = customer.CustomerID,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1)
            };

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

             await _context.AddAsync(refreshTokenEntity);
             await _context.SaveChangesAsync(); 
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);  
                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("RenewToken")]
        public  async Task<IActionResult> RenewToken (TokenModel model)
        {
            var JWTTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var TokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false // don't check for expired token
            };
            try
            {
                //check 1: AccessToken valid format
                var tokenInVerification = JWTTokenHandler.ValidateToken(model.AccessToken, 
                    TokenValidateParam, out var validatedToken);
            
                //check 2: Check alg
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha512, 
                        StringComparison.InvariantCultureIgnoreCase);
                    if(!result)
                    {
                        return Ok(new APIResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.
                    FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);

                if(expireDate > DateTime.UtcNow)
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Access  token has not yet expired"
                    });
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == model.RefreshToken);
                
                if(storedToken == null)
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/revoked?
                if(storedToken.IsUsed)
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Refresh token has been used"
                    });
                }

                if (storedToken.IsRevoked)
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    });
                }

                //check 6: AccessToken id == JwtID in RefreshToken
                var jti = tokenInVerification.Claims.
                    FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                if(storedToken.JwtID != jti)
                {
                    return Ok(new APIResponse
                    {
                        Success = false,
                        Message = "Token does not match"
                    });
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;

                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                //Create new token
                var customer = await _context.Customers.SingleOrDefaultAsync(c => c.CustomerID == storedToken.CustomerID);
                var token = await GenerateToken(customer);

                return Ok(new APIResponse
                {
                    Success = true,
                    Message = "Renew token success",
                    Data = token
                });

                return Ok(new APIResponse
                {
                    Success = true,
                });
            } catch (Exception ex)
            {
                return BadRequest(new APIResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }
        }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
