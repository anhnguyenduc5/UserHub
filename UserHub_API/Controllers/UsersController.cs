using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project_API.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Project_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly PRN231_1Context _context;
        private readonly RoleManager<AspNetRole> _roleManager;

        private IQueryable<AspNetUser>? listUserQuery;

        public UsersController(UserManager<AspNetUser> userManager, SignInManager<AspNetUser> signInManager, IConfiguration configuration, PRN231_1Context context, RoleManager<AspNetRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid password" });
            }
            var roleId = await _context.AspNetUserRoles
    .Where(ur => ur.UserId == user.Id)
    .Select(ur => ur.RoleId)
    .FirstOrDefaultAsync();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, roleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //token.ValidTo = DateTime.Now.AddHours(1);
            Console.WriteLine("Thời gian hiện tại (UTC): " + DateTime.UtcNow);
            Console.WriteLine("Thời gian hết hạn của token (UTC): " + token.ValidTo);
            return Ok(new
            {
                Token = tokenHandler.WriteToken(token),
                Expiration = token.ValidTo
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null)
                {
                    return BadRequest(new { Message = "Username already exists" });
                }

                var existingEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingEmail != null)
                {
                    return BadRequest(new { Message = "Email already exists" });
                }
                var user = new AspNetUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber, Status = (byte) 1, Avatar = model.Avatar};

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userRole = new AspNetUserRole
                    {
                        UserId = user.Id,
                        RoleId = 2
                    };

                    _context.AspNetUserRoles.Add(userRole);
                    await _context.SaveChangesAsync();

                    var userDetails = new UserDetail
                    {
                        UserId = user.Id,
                        FullName = model.FullName,
                        Address = model.Address,
                        DateOfBirth = model.DateOfBirth
                    };
                    _context.UserDetails.Add(userDetails);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "User registered successfully" });
                }

                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            var userInfo = await _userManager.Users
        .Include(u => u.UserDetails)
        .Include(u => u.Roles)
        .FirstOrDefaultAsync(u => u.Id == userId);
            if (userInfo == null)
            {
                return NotFound("Can not find user with Id: " + userId);
            }
            var userProfile = new
            {
                userInfo.Id,
                userInfo.UserName,
                userInfo.Email,
                userInfo.PhoneNumber,
                userInfo.Avatar,
                Role = userInfo.Roles.FirstOrDefault()?.Name,
                UserDetails = userInfo.UserDetails.Select(ud => new
                {
                    ud.FullName,
                    ud.Address,
                    ud.DateOfBirth,
                }).ToList()
            };

            return Ok(userProfile);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserModel model)
        {
            if (userId != model.UserId)
            {
                return BadRequest("User ID mismatch");
            }
            var user = await _userManager.Users
                .Include(u => u.UserDetails)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            if (model.Avatar != null && model.Avatar != "")
            {
                user.Avatar = model.Avatar;
            }
            var userDetail = user.UserDetails.FirstOrDefault();
            if (userDetail != null)
            {
                userDetail.FullName = model.FullName;
                userDetail.Address = model.Address;
                userDetail.DateOfBirth = model.DateOfBirth;
            }
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.UserDetails)
                .Include(u => u.Roles)
                .Where(u => u.Status != 0)
                .ToListAsync();

            var userList = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                Role = user.Roles.FirstOrDefault()?.Name,
                UserDetails = user.UserDetails.Select(ud => new
                {
                    ud.FullName,
                    ud.Address,
                    ud.DateOfBirth
                }).ToList()
            }).ToList();

            return Ok(userList);
        }

        [HttpPost("ChangeRole/{id}")]
        public async Task<IActionResult> PromoteToAdmin(int id, [FromQuery] string action)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Lấy roleId của Admin (có thể thay đổi theo cách bạn xác định vai trò Admin)
            var adminRoleId = 0; // Giả sử 2 là ID của vai trò Admin
            if (action == "promote")
            {
                adminRoleId = 1;
            }
            else if (action == "degrade")
            {
                adminRoleId = 2;
            }
            else
            {
                return BadRequest("Invalid action.");
            }
            // Xóa tất cả các vai trò hiện tại của người dùng
            var existingRoles = await _context.AspNetUserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            _context.AspNetUserRoles.RemoveRange(existingRoles);

            // Thêm vai trò Admin
            var userRole = new AspNetUserRole
            {
                UserId = user.Id,
                RoleId = adminRoleId
            };

            _context.AspNetUserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok("User promoted to Admin successfully");
        }
        [HttpPut("Delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //var user = await _userManager.FindByIdAsync(id.ToString());
            //if (user == null)
            //{
            //    return NotFound("User not found");
            //}

            //var result = await _userManager.DeleteAsync(user);
            //if (result.Succeeded)
            //{
            //    return NoContent(); // HTTP 204 No Content
            //}

            //return BadRequest(result.Errors);
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            var status = 0;

            user.Status = (byte) status;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(result.Errors);
        }
        [HttpPut("DeleteSelected")]
        public async Task<IActionResult> DeleteSelected([FromBody] List<int> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return NotFound($"User with ID {userId} not found");
                }

                var status = 0;
                user.Status = (byte)status;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            return NoContent();
        }
        [HttpGet("/DeletedUser")]
        [EnableQuery]
        public async Task<IActionResult> GetAllDeletedUsers()
        {
            var users = await _userManager.Users
                .Include(u => u.UserDetails)
                .Include(u => u.Roles)
                .Where(u => u.Status == (byte)0)
                .ToListAsync();

            var userList = users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.PhoneNumber,
                Role = user.Roles.FirstOrDefault()?.Name,
                UserDetails = user.UserDetails.Select(ud => new
                {
                    ud.FullName,
                    ud.Address,
                    ud.DateOfBirth
                }).ToList()
            }).ToList();

            return Ok(userList);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Lấy userId từ token hoặc session (nếu có)
            var userId = model.UserId; // Giả sử bạn lưu userId trong claim

            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            return BadRequest(result.Errors);
        }
        
    }
}
