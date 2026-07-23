using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Identity.DTOs;
using FluentValidation;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<SendOtpDto> _sendOtpValidator;
    private readonly IValidator<RegisterWithOtpDto> _registerWithOtpValidator;

    public IdentityController(IIdentityService identityService, IValidator<SendOtpDto> sendOtpValidator, IValidator<RegisterWithOtpDto> registerWithOtpValidator)
    {
        _identityService = identityService;
        _sendOtpValidator = sendOtpValidator;
        _registerWithOtpValidator = registerWithOtpValidator;
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _sendOtpValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new
            {
                Code = e.PropertyName,
                Description = e.ErrorMessage
            });

            return BadRequest(new { IsSuccess = false, IsError = true, Errors = errors });
        }

        var result = await _identityService.SendOtpAsync(dto, cancellationToken);
        return Ok(result);
    }
    [HttpPost("register-with-otp")]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _registerWithOtpValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new
            {
                Code = e.PropertyName,
                Description = e.ErrorMessage
            });

            return BadRequest(new { IsSuccess = false, IsError = true, Errors = errors });
        }

        var result = await _identityService.RegisterWithOtpAsync(dto, cancellationToken);
        return Ok(result);
    }
}