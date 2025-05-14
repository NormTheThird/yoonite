using System;
using System.Runtime.Serialization;
using Yoonite.Common.Models;

namespace Yoonite.Common.RequestAndResponses
{
    [DataContract]
    public class RegisterAccountRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string FirstName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string LastName { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Password { get; set; } = string.Empty;
    }

    [DataContract]
    public class RegisterAccountResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public Guid NewAccountId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class ValidateAccountRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string Password { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public bool RememberMe { get; set; } = false;
    }

    [DataContract]
    public class ValidateAccountResponse : BaseResponse
    {
        [DataMember(IsRequired = true)] public SecurityModel SecurityModel { get; set; } = new SecurityModel();
    }

    [DataContract]
    public class ChangePasswordRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string OldPassword { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string NewPassword { get; set; } = string.Empty;
    }

    [DataContract]
    public class ChangePasswordResponse : BaseResponse { }

    [DataContract]
    public class PasswordResetRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string Email { get; set; } = string.Empty;
        [DataMember(IsRequired = true)] public string BaseUrl { get; set; } = string.Empty;
    }

    [DataContract]
    public class PasswordResetResponse : BaseResponse
    {

        [DataMember(IsRequired = true)] public Guid ResetId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class ValidatePasswordResetRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid ResetId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public string NewPassword { get; set; } = string.Empty;
    }

    [DataContract]
    public class ValidatePasswordResetResponse : BaseResponse { }

    [DataContract]
    public class GetSecurityModelRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
    }

    [DataContract]
    public class GetSecurityModelResponse : BaseResponse
    {
        [DataMember(IsRequired = true)]
        public SecurityModel SecurityModel { get; set; } = new SecurityModel();
    }

    [DataContract]
    public class UpdateLastLoginRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; }
    }

    [DataContract]
    public class UpdateLastLoginResponse : BaseResponse { }

    [DataContract]
    public class SaveNewPasswordRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public Guid AccountId { get; set; } = Guid.Empty;
        [DataMember(IsRequired = true)] public string Password { get; set; } = string.Empty;
    }

    [DataContract]
    public class SaveNewPasswordResponse : BaseResponse { }

    [DataContract]
    public class GetRefreshTokenRequest : BaseRequest
    {
        [DataMember(IsRequired = true)] public string RefreshToken { get; set; } = string.Empty;
    }
}