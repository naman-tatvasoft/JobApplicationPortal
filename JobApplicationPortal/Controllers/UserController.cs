using JobApplicationPortal.Service.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationPortal.Controllers;

[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;

    public UserController(IUserService userService){
        _userService = userService;
    }
 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    [HttpGet("get/employers")]
    public IActionResult GetEmployers(){
        var result = _userService.GetEmployers();

        if(result.StatusCode == 200){
            return Ok(result.Data);
        }else{
            return StatusCode(result.StatusCode, new { Message = result.Message });
        }
    }

    
    [HttpGet("get/candidates")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCandidates(){
        var result = _userService.GetCandidates();

        if(result.StatusCode == 200){
            return Ok(result.Data);
        }else{
            return StatusCode(result.StatusCode, new { Message = result.Message });
        }
    }

}
