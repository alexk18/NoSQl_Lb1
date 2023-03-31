using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LR1Backend.API.Authorization;
using LR1Backend.API.Models;
using Microsoft.AspNetCore.Mvc;
using NoSQl_Lb1_Kosinskiy_Kostikova.Models;
using NoSQl_Lb1_Kosinskiy_Kostikova.Repositories;

namespace NoSQl_Lb1_Kosinskiy_Kostikova.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserRepository userRepository;
        private readonly JwtHandler _jwtHandler;
        public UserController(UserRepository userRepository, JwtHandler jwtHandler)
        {
            this.userRepository = userRepository;
            _jwtHandler = jwtHandler;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserApiModel model)
        {
            var existing = userRepository.GetByUsername(model.Username);

            if (existing != null)
                return BadRequest(new
                {
                    Error = "User already exist"
                });

            var dbUser = userRepository.Insert(new User()
            {
                UserName = model.Username,
                Password = model.Password
            });
            return Ok(new UserResponseModel
            {
                Id = dbUser.Id,
                Username = dbUser.UserName
            });
        }

        [HttpGet]
        [Route("getAll")]
        public IActionResult GetAll()
        {
            var users = userRepository.GetAll().Select(x => new UserResponseModel
                {
                    Id = x.Id,
                    Username = x.UserName
                });
            return Ok(users);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] UserApiModel model)
        {
            var user =  userRepository.GetByUsernameAndPassword(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { Error = "User not exists" });

            var signingCredentials = _jwtHandler.GetSigningCredentials();
            var claims = _jwtHandler.GetClaims(user);
            var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return Ok(new JsonResult(new
            {
                JWT = token
            }));
        }

        [HttpPut]
        [Route("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordApiModel model)
        {
            if (model.NewPassword == model.OldPassword)
                return BadRequest("New password must be different from old one");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userToUpdate = userRepository.GetById(Guid.Parse(userId));

            if (userToUpdate == null)
                return BadRequest(new { Error = "User not exists" });

            userToUpdate.Password = model.NewPassword;
            await userRepository.Edit(userToUpdate);

            return NoContent();
        }

        private ClaimsIdentity GetIdentity(string login, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
            };
            ClaimsIdentity claimsIdentity = new
                ClaimsIdentity(claims, "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
