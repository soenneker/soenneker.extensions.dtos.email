[![](https://img.shields.io/nuget/v/soenneker.extensions.dtos.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.dtos.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.dtos.email/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.dtos.email/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.dtos.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.dtos.email/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Dtos.Email
### A collection of helpful EmailDto extension methods

## 📧  Features

- Converts a well-defined `EmailDto` into a [MimeKit](https://github.com/jstedfast/MimeKit) `MimeMessage`
- Supports both `html` and `plain` formats
- Adds `To`, `Cc`, `Bcc`, and `Reply-To` addresses
- Automatically attaches files via `EmailAttachmentDto`
- Sets headers for `High` and `Low` priority emails
- Logs malformed recipients using `ILogger`

---

## 🧪 Validation

The extension validates:
- Required fields: `To`, `Subject`, and `Body`
- Non-null, non-whitespace addresses
- Optionally logs issues rather than throwing for individual recipient fields

---

## Installation

```
dotnet add package Soenneker.Extensions.Dtos.Email
```

---

## 🔧 Usage

```csharp
var mimeMessage = emailDto.ToMimeMessage(logger);
```

---

## 📁 Example `EmailDto`

```csharp
var dto = new EmailDto
{
    To = new List<string> { "to@example.com" },
    Cc = new List<string> { "cc@example.com" },
    Bcc = new List<string> { "bcc@example.com" },
    ReplyTo = "reply@example.com",
    Name = "Sender Name",
    Address = "sender@example.com",
    Subject = "Test Subject",
    Body = "<p>This is a test email.</p>",
    Format = EmailFormat.Html,
    Priority = EmailPriority.High,
    Attachments = new List<EmailAttachmentDto>
    {
        new EmailAttachmentDto
        {
            FileName = "test.txt",
            MimeType = "text/plain",
            Data = Encoding.UTF8.GetBytes("Sample attachment content")
        }
    }
};
```