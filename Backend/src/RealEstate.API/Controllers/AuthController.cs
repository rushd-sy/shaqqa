using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Auth;

namespace RealEstate.API.Controllers;

[ApiController]
public class AuthController(ITokenProvider authService) : ControllerBase
{
    private readonly ITokenProvider _authService = authService;


    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult> Revoke([FromBody] RefreshTokenRequest request)
    {
        await _authService.RevokeTokenAsync(request.RefreshToken);
        return NoContent();
    }
}