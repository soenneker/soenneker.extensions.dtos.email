[![](https://img.shields.io/nuget/v/soenneker.extensions.dtos.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.dtos.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.dtos.email/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.dtos.email/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.dtos.email.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.dtos.email/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.dtos.email/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.dtos.email/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Dtos.Email

Converts `EmailDto` messages into MimeKit `MimeMessage` instances, including HTML or plain-text bodies, recipients, attachments, reply-to addresses, and priority headers.

## Installation

```bash
dotnet add package Soenneker.Extensions.Dtos.Email
```

## Usage

```csharp
using System.Text;
using Microsoft.Extensions.Logging;
using MimeKit;
using Soenneker.Dtos.Email;
using Soenneker.Dtos.Email.Attachment;
using Soenneker.Enums.Email.Format;
using Soenneker.Enums.Email.Priority;
using Soenneker.Extensions.Dtos.Email;

var email = new EmailDto
{
    Name = "Example App",
    Address = "no-reply@example.com",
    To = ["person@example.com"],
    Cc = ["team@example.com"],
    ReplyTo = "support@example.com",
    Subject = "Your export is ready",
    Body = "<p>The export is attached.</p>",
    Format = EmailFormat.Html,
    Priority = EmailPriority.Normal,
    Attachments =
    [
        new EmailAttachmentDto
        {
            FileName = "export.csv",
            MimeType = "text/csv",
            Data = Encoding.UTF8.GetBytes("id,name\n1,Example")
        }
    ]
};

MimeMessage message = email.ToMimeMessage(logger);
```

`EmailFormat.Plaintext` creates a `text/plain` body; other format values create `text/html`. Attachments produce a `multipart/mixed` body with the text part first. Attachment bytes are wrapped in streams owned by the returned MIME structure, so dispose the `MimeMessage` after it has been sent or serialized.

High priority adds `X-Priority: 1`, `Priority: urgent`, and `Importance: high`. Low priority adds the corresponding low-priority values. Normal priority adds no priority headers.

## Validation and recipient handling

The DTO must provide a non-whitespace subject, body, sender address, and at least one usable `To` address. Missing required message data throws `InvalidOperationException`; a null DTO throws `ArgumentNullException`.

Whitespace-only entries in `To`, `Cc`, or `Bcc` are skipped and logged without including recipient addresses in the log. At least one `To` entry must remain after filtering. MimeKit validates nonblank sender and recipient syntax and can throw for malformed addresses. `ReplyTo` is omitted when blank and otherwise follows the same MimeKit address validation.
