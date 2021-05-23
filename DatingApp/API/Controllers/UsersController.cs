using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatingApp.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id) 
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> Add(string username)
        {
            if(_context.Users.Any(x=>x.UserName.Equals(username))){
                return BadRequest(new {Error="User already exists"});
            }
            var user = new AppUser(){UserName=username};

            _context.Users.Add(user);
            if(await _context.SaveChangesAsync()>0)
            return user;
            else return BadRequest(new {Error="Some Error Occured"});
        }
    }
}