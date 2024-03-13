using backend.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
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

        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDTO body)
        {
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand cmd = new(@"INSERT INTO organizations (name) VALUES (@Org_Name) RETURNING org_id", conn);
                cmd.Parameters.AddWithValue("@Org_Name", body.OrganizationName);
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

        [HttpDelete("{org_id}")]
        public async Task<IActionResult> DeleteOrg(int org_id) {
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand cmd = new(@"DELETE FROM organizations WHERE org_id=@Org_id", conn);
                cmd.Parameters.AddWithValue("@Org_id", org_id);
                var result = await cmd.ExecuteNonQueryAsync();

                await conn.CloseAsync();
                NpgsqlConnection.ClearAllPools();
                return Ok(result > 0);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrganization([FromBody] UpdateOrganizationDTO body)
        {
            try
            {
                await using NpgsqlConnection conn = new(_connectionString);
                await conn.OpenAsync();

                await using NpgsqlCommand cmd = new(@"UPDATE organizations SET name=@Org_Name WHERE org_id=@Org_id RETURNING org_id", conn);
                cmd.Parameters.AddWithValue("@Org_Name", body.Org_name);
                cmd.Parameters.AddWithValue("@Org_id", body.Org_id);
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
