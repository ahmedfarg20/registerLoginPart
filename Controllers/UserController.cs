using Azure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using projectUsers.Data.Context;
using projectUsers.Data.Models;
using projectUsers.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace projectUsers.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        UsersContext _db;

        public UserController(IConfiguration configuration, UserManager<User> userManager, UsersContext db)
        {
            _configuration = configuration;
            _userManager = userManager;
            _db = db;
        }
        #region login static
        //public ActionResult Login(LoginDTO credntials) //loginDTO==credntials
        //{
        //    if (credntials.Username != "admin" || credntials.Password != "pass")
        //    {
        //        return Unauthorized();
        //    }
        //    var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.GivenName, "Ahmed Mahmoud"),
        //    new Claim("Nationality","Egyptian")
        //};
        //    var secretKey = _configuration.GetValue<string>("secretKey");
        //    var keyInBytes = Encoding.ASCII.GetBytes(secretKey);
        //    var key = new SymmetricSecurityKey(keyInBytes);
        //    var algorithmAndKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    //system.identitymodel.tokens.jwt part
        //    var jwtToken = new JwtSecurityToken(
        //        claims: claims,
        //        signingCredentials: algorithmAndKey,
        //        expires: DateTime.Now.AddMinutes(90)
        //        );
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    return Ok(tokenHandler.WriteToken(jwtToken));
        //}
        #endregion

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterDTO registerDTO)
        {
            var EmailExists = _db.Users.Any(x=>x.Email==registerDTO.Email);
            if (EmailExists)
            {
                return Conflict();
            }
            var user = new User
            { UserName = registerDTO.Username, 
                Email = registerDTO.Email, 
                Name = registerDTO.Name, 
                Age = registerDTO.Age,
                Role = registerDTO.Role
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            await _userManager.AddClaimsAsync(user, new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim("UserName", registerDTO.Username),
            new Claim("Email", registerDTO.Email),
            new Claim("Name", registerDTO.Name),
            new Claim("Age",registerDTO.Age.ToString() ),
            new Claim("role",registerDTO.Role)
        });
            return StatusCode(StatusCodes.Status201Created, "User Created");
        }
        #region register an admin
        //[HttpPost]
        //[Route("register")]
        //public async Task<ActionResult> AdminRegister(RegisterDTO registerDTO)
        //{
        //    var user = new User { UserName = registerDTO.Username, Email = registerDTO.Email, Name = registerDTO.Name };
        //    var result = await _userManager.CreateAsync(user, registerDTO.Password);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors);
        //    }
        //    await _userManager.AddClaimsAsync(user, new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier,user.Id),
        //    new Claim(ClaimTypes.GivenName, "Ahmed Mahmoud"),
        //    new Claim("Nationality","Egyptian")
        //});
        //    return StatusCode(StatusCodes.Status201Created, "User Created");
        //}
        #endregion

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(LoginDTO credntials)
        {
          //  var requiredUser = await _userManager.FindByNameAsync(credntials.Username) ;
            var requiredEmail = await _userManager.FindByEmailAsync(credntials.Email);
            //check for locking
            var LockedUser = await _userManager.IsLockedOutAsync(requiredEmail);
            var isAuth = await _userManager.CheckPasswordAsync(requiredEmail, credntials.Password);
      //      var userRole = await _userManager.GetRolesAsync(requiredUser);
            if (!isAuth)
            {
                return Unauthorized();
            }
            if (LockedUser)
            {
                return Unauthorized();
            }
            var claims =await _userManager.GetClaimsAsync(requiredEmail);
            var secretKey = _configuration.GetValue<string>("secretKey");
            var keyInBytes = Encoding.ASCII.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyInBytes);
            var algorithmAndKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expDate = DateTime.Now.AddMinutes(15);
            //system.identitymodel.tokens.jwt part
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                signingCredentials: algorithmAndKey,
                expires: expDate
               // roles : userRole
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            return Ok(new TokenDTO
            {
                Token = tokenHandler.WriteToken(jwtToken),
                Exp = expDate

            });
         //   return Ok();
        }
    }
}

