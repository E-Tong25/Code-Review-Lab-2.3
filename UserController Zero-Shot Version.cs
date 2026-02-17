[HttpGet("/user")]
public IActionResult GetUserProfile()
{
    string userId = Request.Query["id"];
    
    // Validate input
    if (string.IsNullOrWhiteSpace(userId))
        return BadRequest("User ID is required");
    
    try
    {
        using (var connection = new SqliteConnection("Data Source=users.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            
            // Use parameterized queries to prevent SQL injection
            command.CommandText = "SELECT * FROM users WHERE user_id = @UserId";
            command.Parameters.AddWithValue("@UserId", userId);
            
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var userData = ProcessData(reader);
                    var response = FormatResponse(userData);
                    return Ok(response);
                }
            }
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, "Database error occurred");
    }
    
    return NotFound();
}


//Based on the Zero-Shot Prompt Version, Claude Haiku 4.5 found that some critical issues and code quality issues exist in the code:
// Critical Issues:
//1. SQL Injection vulnerability: The code uses string interpolation to construct SQL commands, which is unsafe. It should use parameterized queries with SqliteParameter to prevent SQL injection attacks.
//2. Missing Input validation: The userId query parameter is used directly without validation. It
//Code Quality Issues:
//1. Poor variable naming: The variables 'temp' and 'res' are not descriptive
//2. Misssing error handling: The code does not handle potential exceptions that may occur during database operations. Implementing try-catch blocks to handle exceptions gracefully and provide meaningful error messages to the client is recommended.