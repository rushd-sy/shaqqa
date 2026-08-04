using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Auth;
using RealEstate.Application.Identity.DTOs;

namespace RealEstate.API.Controllers;

[ApiController]
public class AuthController(IAuthServices authService) : ControllerBase
{
    private readonly IAuthServices _authService = authService;


    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request , CancellationToken ct) 
    {

        if (string.IsNullOrWhiteSpace(request?.RefreshToken))
        {
            return BadRequest(new { error = "Refresh token is required." });
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized(new { error = "Invalid user session." });
        }

        try
        {
            await _authService.RevokeTokenAsync(request.RefreshToken, userId , ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}