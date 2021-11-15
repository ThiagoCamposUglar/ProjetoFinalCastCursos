using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
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
        public async Task<ActionResult<EmployeeDto>> Register(RegisterDto registerDto)
        {
            if(await EmployeeExists(registerDto.UserName)) return BadRequest("Nome de usuário já existe");

            using var hmac = new HMACSHA512();
            var employee = new Employee
            {
                EmployeeName = registerDto.EmployeeName,
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                RoleId = registerDto.RoleId
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                UserName = employee.UserName,
                Token = _tokenService.CreateToken(employee),
                RoleId = employee.RoleId,
                EmployeeName = employee.EmployeeName
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<EmployeeDto>> Login(LoginDto loginDto)
        {
            var employee = await _context.Employees.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);

            if(employee == null) return Unauthorized("Usuário inválido");

            using var hmac = new HMACSHA512(employee.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if(computedHash[i] != employee.PasswordHash[i]) return Unauthorized("Senha inválida");
            }

            return new EmployeeDto
            {
                Id = employee.Id,
                UserName = employee.UserName,
                EmployeeName = employee.EmployeeName,
                RoleId = employee.RoleId,
                Role = _context.Roles.Find(employee.RoleId),
                Token = _tokenService.CreateToken(employee)
            };
        }

        private async Task<bool> EmployeeExists(string userName)
        {
            return await _context.Employees.AnyAsync(x => x.UserName == userName.ToLower());
        }
    }
}