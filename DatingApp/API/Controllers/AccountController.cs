using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName)) return BadRequest("Username already taken!");

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasseordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if (user == null) return BadRequest("Invalid username");

            using var hmac = new HMACSHA512(user.PasseordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password!");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        public async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}