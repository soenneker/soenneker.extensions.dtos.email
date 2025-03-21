using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MimeKit;
using Soenneker.Dtos.Email;
using Soenneker.Dtos.Email.Attachment;
using Soenneker.Extensions.Enumerable;
using Soenneker.Extensions.Enumerable.String;
using Soenneker.Extensions.String;

namespace Soenneker.Extensions.Dtos.Email;

/// <summary>
/// A collection of helpful EmailDto extension methods
/// </summary>
public static class EmailDtoExtension
{
    /// <summary>
    /// Converts an <see cref="EmailDto"/> to a <see cref="MimeMessage"/> using the provided logger for diagnostics.
    /// </summary>
    /// <param name="emailDto">The email DTO containing message details like recipients, subject, body, and attachments.</param>
    /// <param name="logger">An <see cref="ILogger"/> used for logging malformed or empty recipient fields.</param>
    /// <returns>A fully constructed <see cref="MimeMessage"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emailDto"/> is null.</exception>
    /// <exception cref="Exception">Thrown if subject, body, or recipients are missing from the DTO.</exception>
    public static MimeMessage ToMimeMessage(this EmailDto emailDto, ILogger logger)
    {
        if (emailDto == null)
            throw new ArgumentNullException(nameof(emailDto));

        if (emailDto.Subject.IsNullOrEmpty())
            throw new Exception("Missing subject for email");

        if (emailDto.Body.IsNullOrEmpty())
            throw new Exception("Missing body for email");

        if (!emailDto.To.Any())
            throw new Exception("Missing recipients for email");

        var message = new MimeMessage();

        foreach (string recipient in emailDto.To)
        {
            if (recipient.IsNullOrWhiteSpace())
            {
                logger.LogError("Recipient was null or whitespace. Recipients: {recipients}", emailDto.To.ToCommaSeparatedString());
                continue;
            }

            message.To.Add(new MailboxAddress("", recipient));
        }

        if (emailDto.Cc.Populated())
        {
            foreach (string cc in emailDto.Cc!)
            {
                if (cc.IsNullOrWhiteSpace())
                {
                    logger.LogError("Cc email was null or whitespace. CC: {recipients}", emailDto.Cc.ToCommaSeparatedString());
                    continue;
                }

                message.Cc.Add(new MailboxAddress("", cc));
            }
        }

        if (emailDto.Bcc.Populated())
        {
            foreach (string bcc in emailDto.Bcc!)
            {
                if (bcc.IsNullOrWhiteSpace())
                {
                    logger.LogError("Bcc email was null or whitespace. BCC: {recipients}", emailDto.Bcc.ToCommaSeparatedString());
                    continue;
                }

                message.Bcc.Add(new MailboxAddress("", bcc));
            }
        }

        if (!emailDto.ReplyTo.IsNullOrWhiteSpace())
        {
            message.ReplyTo.Add(new MailboxAddress("", emailDto.ReplyTo));
        }

        message.From.Add(new MailboxAddress(emailDto.Name, emailDto.Address));
        message.Subject = emailDto.Subject;

        // Handle attachments and body formatting
        if (emailDto.Attachments.Populated())
        {
            var multipart = new Multipart("mixed");

            TextPart bodyPart = emailDto.Format == Enums.Email.Format.EmailFormat.Plaintext
                ? new TextPart("plain") { Text = emailDto.Body }
                : new TextPart("html") { Text = emailDto.Body };

            multipart.Add(bodyPart);

            foreach (EmailAttachmentDto attachment in emailDto.Attachments!)
            {
                var mimePart = new MimePart(attachment.MimeType)
                {
                    Content = new MimeContent(new MemoryStream(attachment.Data)),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = attachment.FileName
                };

                multipart.Add(mimePart);
            }

            message.Body = multipart;
        }
        else
        {
            message.Body = emailDto.Format == Enums.Email.Format.EmailFormat.Plaintext
                ? new TextPart("plain") { Text = emailDto.Body }
                : new TextPart("html") { Text = emailDto.Body };
        }

        // Set priority headers
        switch (emailDto.Priority)
        {
            case nameof(Enums.Email.Priority.EmailPriority.High):
                message.Headers.Add("X-Priority", "1");
                message.Headers.Add("Priority", "urgent");
                message.Headers.Add("Importance", "high");
                break;

            case nameof(Enums.Email.Priority.EmailPriority.Low):
                message.Headers.Add("X-Priority", "5");
                message.Headers.Add("Priority", "non-urgent");
                message.Headers.Add("Importance", "low");
                break;
        }

        return message;
    }
}
