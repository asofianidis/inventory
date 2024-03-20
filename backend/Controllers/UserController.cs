using backend.DTO;
using backend.Utils;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[Controller]/[Action]")]
    public class UserController : Controller
    {
        private readonly string? _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("default");
        }

        [HttpPost]
        public async Task<IActionResult> UserRegistration(UserRegistrationDTO user)
        {
            try {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand orgCmd = new(@"SELECT org_id FROM organizations WHERE org_code=@Org_code", conn);
                orgCmd.Parameters.AddWithValue("@Org_code", user.org_code);
                var orgResult = await orgCmd.ExecuteScalarAsync();


                if (orgResult == null) {
                    throw new Exception("Failed to find Org");
                }

                var rand = new Random();
                var salt = HashString.HashWithSHA256(DateTime.Now.ToString() + rand.Next(100000, 1000000).ToString());

                var pw = HashString.HashWithSHA256(salt + user.password);

                await using NpgsqlCommand insertUserCmd = new(@"INSERT INTO users (full_name, username, org_id, created, email, salt, pw) VALUES (@Full_name, @Username, @Org_id, NOW(), @Email, @Salt, @Pw) RETURNING user_id;", conn);
                insertUserCmd.Parameters.AddWithValue("@Full_name", user.full_name);
                insertUserCmd.Parameters.AddWithValue("@Username", user.username);
                insertUserCmd.Parameters.AddWithValue("@Org_id", orgResult);
                insertUserCmd.Parameters.AddWithValue("@Email", user.email);
                insertUserCmd.Parameters.AddWithValue("@Salt", salt);
                insertUserCmd.Parameters.AddWithValue("@Pw", pw);
                var result = await insertUserCmd.ExecuteScalarAsync();

                if (result == null) {
                    throw new Exception("Failed to create User");
                }

                await using NpgsqlCommand tokenCmd = new(@"INSERT INTO users_tokens (user_id, token, created, expires) VALUES (@User_id, @Token, NOW(), NOW() + INTERVAL '30 days') RETURNING token", conn);
                tokenCmd.Parameters.AddWithValue("@User_id", result);
                tokenCmd.Parameters.AddWithValue("@Token", HashString.HashWithSHA256(DateTime.Now.ToString() + rand.Next(100000, 1000000).ToString()));
                var tokenResult = await tokenCmd.ExecuteScalarAsync();

                return Ok(tokenResult);
            }catch(Exception e){
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }
        }

        
    }
}
