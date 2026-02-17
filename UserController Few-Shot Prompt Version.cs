[HttpGet("/user")]
public IActionResult GetUserProfile()
{
    string userId = Request.Query["id"];
    
    if (string.IsNullOrWhiteSpace(userId))
        return BadRequest("User ID is required.");
    
    try
    {
        using (var connection = new SqliteConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM users WHERE user_id = @userId";
            command.Parameters.AddWithValue("@userId", userId);
            
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var processedUserData = ProcessData(reader);
                    var formattedResponse = FormatResponse(processedUserData);
                    return Ok(formattedResponse);
                }
            }
        }
    }
    catch (SqliteException ex)
    {
        return StatusCode(500, "Database error occurred.");
    }
    
    return NotFound();
}

//Based on the Few-Shot Prompt Version, Claude Haiku 4.5 recommendations:
// Critical Issues:
//Line 12: SQL Injection vulnerability string interpolation in SQL commands is unsafe. You must use parameterized queries with SqliteParameter to prevent SQL injection attacks.
//Line 5: Missing Input validation the userId query parameter is used directly without validation. Add null/empty checks and validate the format before database operations.
//Line 6: Hardcoded database connection stringThe database connection string is hardcoded. Move this to appsettings.json and inject IConfiguration to follow configuration best practices.
// Code Quality Issues:
// Line 15: Poor variable naming 'temp' and 'res' are not descriptive. Rename to processedUserData and formattedResponse to improve readability.
// Line 9: Misssing error handling The code does not handle potential exceptions that may occur during database operations. Implement try-catch blocks to handle exceptions gracefully and provide meaningful error messages to the client.
