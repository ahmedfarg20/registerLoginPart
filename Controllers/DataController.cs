using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace projectUsers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public ActionResult <List<string>> GetALl()
    {
        //return Ok(new List<string> { "Ahmed", "salma", "sami" });
        return new List<string> { "Ahmed", "salma", "sami" };
    }
    [Authorize(Policy = "EgyptOnly")]
    [HttpGet]
    [Route("Egyptian")]
    public ActionResult<List<string>> GetForEG()
    {
        return new List<string> { "Ahmed EG", "Muhammed EG", "Salma EG" };
    }


    [Authorize(Policy = "AdminOnly")]
    [Route("Admin")]
    [HttpGet]
    public ActionResult<List<string>> GetForAdmin()
    {
        return new List<string> { "Ahmed Admin", "Muhammed Admin", "Salma Admin" };
    }
}