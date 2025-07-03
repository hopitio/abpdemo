using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AbpAngular.Suppliers;

/// <summary>
/// Email validation attribute that only validates when the field has a value
/// </summary>
public class OptionalEmailAddressAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true; // Allow empty values
        }
        
        var emailAttribute = new EmailAddressAttribute();
        return emailAttribute.IsValid(value);
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field is not a valid e-mail address.";
    }
}

/// <summary>
/// Phone validation attribute that only validates when the field has a value
/// </summary>
public class OptionalPhoneAttribute : ValidationAttribute
{
    private static readonly Regex PhoneRegex = new Regex(@"^[\+]?[0-9\s\-\(\)]{7,20}$", RegexOptions.Compiled);
    
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true; // Allow empty values
        }
        
        var phoneString = value.ToString()!;
        return PhoneRegex.IsMatch(phoneString);
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field is not a valid phone number.";
    }
}

/// <summary>
/// URL validation attribute that only validates when the field has a value
/// </summary>
public class OptionalUrlAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true; // Allow empty values
        }
        
        var urlString = value.ToString()!;
        return Uri.TryCreate(urlString, UriKind.Absolute, out var uri) && 
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeFtp);
    }
    
    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field is not a valid fully-qualified http, https, or ftp URL.";
    }
}
