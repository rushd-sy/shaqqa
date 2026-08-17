using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs;
using RealEstate.Application.Identity.DTOs;
using RealEstate.Domain.Common.Results;

namespace RealEstate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IValidator<SendOtpDto> _sendOtpValidator;
    private readonly IValidator<RegisterWithOtpDto> _registerWithOtpValidator;
    private readonly IValidator<LoginWithOtpDto> _loginWithOtpValidator;

    public IdentityController(IIdentityService identityService, IValidator<SendOtpDto> sendOtpValidator, IValidator<RegisterWithOtpDto> registerWithOtpValidator, IValidator<LoginWithOtpDto> loginWithOtpValidator)
    {
        _identityService = identityService;
        _sendOtpValidator = sendOtpValidator;
        _registerWithOtpValidator = registerWithOtpValidator;
        _loginWithOtpValidator = loginWithOtpValidator;
    }

    [HttpPost("send-otp")]
    [EnableRateLimiting("otp-policy")]
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
        return HandleResult(result);
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
        return HandleResult(result);
    }
    [HttpPost("login-otp")]
    public async Task<IActionResult> LoginWithOtp([FromBody] LoginWithOtpDto dto,CancellationToken cancellationToken)
    {
        var validationResult = await _loginWithOtpValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new
            {
                Code = e.PropertyName,
                Description = e.ErrorMessage
            });

            return BadRequest(new { IsSuccess = false, IsError = true, Errors = errors });
        }
        var result = await _identityService.LoginWithOtpAsync(dto, cancellationToken);

        return HandleResult(result);
    }

    private IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var error = result.TopError;

        return error.Type switch
        {
            ErrorKind.Validation => BadRequest(result.Errors),
            ErrorKind.NotFound => NotFound(result.Errors),
            ErrorKind.Conflict => Conflict(result.Errors),
            ErrorKind.Unauthorized => Unauthorized(result.Errors),
            ErrorKind.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.Errors),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Errors)
        };
    }
}