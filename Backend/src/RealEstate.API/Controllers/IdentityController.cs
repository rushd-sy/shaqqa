using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity.DTOs;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public IdentityController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto, CancellationToken cancellationToken)
    {
        var result = await _identityService.SendOtpAsync(dto, cancellationToken);
        return Ok(result);
    }
    [HttpPost("register-with-otp")]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpDto dto, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterWithOtpAsync(dto, cancellationToken);
        return Ok(result);
    }
}