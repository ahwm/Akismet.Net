# Akismet.Net

Complete and full-featured Akismet client for .NET

Existing libraries don't allow for all of the possible options available in the Akismet API and the source code is not public. This library is meant to fix that.

It is also multi-targeted to support as many applications as possible.

[![NuGet Status](https://img.shields.io/nuget/vpre/AkismetApi.Net)](https://www.nuget.org/packages/AkismetApi.Net/)

Meant to be a drop-in replacement for the leading Akismet library with minimal changes so the class names are the same. However some properties have been removed because of redundancy.

## Example usage:

### .NET Framework

```csharp
var model = new ContactModel();
string ip = Request.Headers["CF-Connecting-IP"] ?? Request.UserHostAddress;
if (String.IsNullOrWhiteSpace(ip))
    ip = Request.ServerVariables["REMOTE_HOST"];
AkismetClient akismet = new AkismetClient("apikeyhere", new Uri("https://www.adamh.us"), "Application Name");
AkismetComment comment = new AkismetComment
{
    CommentAuthor = model.Name,
    CommentAuthorEmail = model.EmailAddress,
    CommentAuthorUrl = "http://www.spamwebsite.com",
    Referrer = Request.UrlReferrer.ToString(),
    UserAgent = Request.UserAgent,
    UserIp = ip,
    CommentContent = model.Message,
    CommentType = AkismentCommentType.ContactForm, // multiple defined values, or use new AkismetCommentType("new-comment-type") for a custom option
    Permalink = "https://www.adamh.us/contact",
    IsTest = "false",
    BlogCharset = "UTF-8",
    BlogLanguage = "en-US",
    CommentDate = DateTime.UtcNow.ToString("O"), // ISO-8601 format
    CommentPostModified = DateTime.UtcNow.ToString("O"), // ISO-8601 format
    UserRole = "administrator",
    RecheckReason = "edit",
    HoneypotFieldName = "honeypot",
    HoneypotFieldValue = "blah"
};

var akismetResult = akismet.Check(comment);
bool isSpam = akismetResult.SpamStatus == SpamStatus.Spam; // Options: Ham, Spam, Unspecified (in the case of an error)

// "invalid" and/or combination of X-akismet-alert-code and X-akismet-alert-msg header values
foreach (string err in akismetResult.Errors)
    Console.WriteLine(err);
    
// Other properties:
//    - ProTip (X-akismet-pro-tip header value, if present)
//    - DebugHelp (X-akismet-debug-help header value, if present)
```

### .NET Core/.NET 5+ Usage Modifications

```csharp
string ip = Request.Headers["CF-Connecting-IP"].ToString() ?? _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "";
if (String.IsNullOrWhiteSpace(ip))
    ip = _contextAccessor.HttpContext?.GetServerVariable("REMOTE_HOST") ?? "";
AkismetComment comment = new AkismetComment
{
    // ... other properties set as above
    UserAgent = Request.Headers[HeaderNames.UserAgent],
    Referrer = Request.Headers[HeaderNames.Referer]
};
```

### Notes

If `HoneypotFieldName` and `HoneypotFieldValue` are supplied then the library will add these two values to the request:

`honeypot_field_name=honeypot&honeypot=blah`
