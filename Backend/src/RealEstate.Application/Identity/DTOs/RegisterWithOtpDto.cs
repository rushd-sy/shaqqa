using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstate.Application.Identity.DTOs
{
    public class RegisterWithOtpDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
