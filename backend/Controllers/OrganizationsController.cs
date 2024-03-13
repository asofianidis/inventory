using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[Controller]/[Action]")]
    public class OrganizationsController : Controller
    {
        private readonly string? _connectionString;

        public OrganizationsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("default");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrgs() {
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand cmd = new(@"SELECT json_agg(t) FROM (SELECT * FROM organizations) t", conn);
                var result = await cmd.ExecuteScalarAsync();

                await conn.CloseAsync();
                NpgsqlConnection.ClearAllPools();
                return Ok(result);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }
        }
    }
}
