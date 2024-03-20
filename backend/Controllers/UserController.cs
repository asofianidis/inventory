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
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand orgCmd = new(@"SELECT org_id FROM organizations WHERE org_code=@Org_code", conn);
                orgCmd.Parameters.AddWithValue("@Org_code", user.org_code);
                var orgResult = await orgCmd.ExecuteScalarAsync();


                if (orgResult == null)
                {
                    throw new Exception("Failed to find Org");
                }

                var rand = new Random();
                var salt = HashString.HashWithSHA256(DateTime.Now.ToString() + rand.Next(100000, 1000000).ToString()).ToLower();

                var pw = HashString.HashWithSHA256(salt + user.password).ToLower();

                await using NpgsqlCommand insertUserCmd = new(@"INSERT INTO users (full_name, username, org_id, created, email, salt, pw) VALUES (@Full_name, @Username, @Org_id, NOW(), @Email, @Salt, @Pw) RETURNING user_id;", conn);
                insertUserCmd.Parameters.AddWithValue("@Full_name", user.full_name);
                insertUserCmd.Parameters.AddWithValue("@Username", user.username);
                insertUserCmd.Parameters.AddWithValue("@Org_id", orgResult);
                insertUserCmd.Parameters.AddWithValue("@Email", user.email);
                insertUserCmd.Parameters.AddWithValue("@Salt", salt);
                insertUserCmd.Parameters.AddWithValue("@Pw", pw);
                var result = await insertUserCmd.ExecuteScalarAsync();

                if (result == null)
                {
                    throw new Exception("Failed to create User");
                }

                await using NpgsqlCommand tokenCmd = new(@"INSERT INTO users_tokens (user_id, token, created, expires) VALUES (@User_id, @Token, NOW(), NOW() + INTERVAL '30 days') RETURNING token", conn);
                tokenCmd.Parameters.AddWithValue("@User_id", result);
                tokenCmd.Parameters.AddWithValue("@Token", HashString.HashWithSHA256(DateTime.Now.ToString() + rand.Next(100000, 1000000).ToString()));
                var tokenResult = await tokenCmd.ExecuteScalarAsync();

                await conn.CloseAsync();
                NpgsqlConnection.ClearAllPools();

                return Ok(tokenResult);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginDTO loginInfo)
        {
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand userCmd = new(@"SELECT user_id FROM users WHERE username=@Username AND LOWER(pw)=ENCODE(SHA256((salt || @Pw::text)::bytea), 'hex')", conn);
                userCmd.Parameters.AddWithValue(@"Username", loginInfo.username);
                userCmd.Parameters.AddWithValue("@Pw", loginInfo.password);
                var result = await userCmd.ExecuteScalarAsync();

                if (result == null) {
                    throw new Exception("Couldn't find user id");
                }

                Random rand = new();

                var token = HashString.HashWithSHA256(DateTime.Now.ToString() + rand.Next(100000, 1000000).ToString()).ToLower();

                await using NpgsqlCommand tokenCmd = new(@"DELETE FROM users_tokens WHERE NOW() > expires OR user_id=@User_id;INSERT INTO users_tokens (user_id, token, created, expires) VALUES (@User_id, @Token, NOW(), NOW() + interval '30 days') RETURNING token", conn);
                tokenCmd.Parameters.AddWithValue("@User_id", result);
                tokenCmd.Parameters.AddWithValue("@Token", token);
                var tokenResult = await tokenCmd.ExecuteScalarAsync();

                if (tokenResult == null)
                {
                    throw new Exception("Failed token creation");
                }


                return Ok(tokenResult);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }
        }

    }
}
