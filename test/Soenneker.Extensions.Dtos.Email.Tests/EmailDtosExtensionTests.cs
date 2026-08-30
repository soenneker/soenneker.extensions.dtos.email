using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Soenneker.Dtos.Email;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Extensions.Dtos.Email.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class EmailDtosExtensionTests : HostedUnitTest
{
    public EmailDtosExtensionTests(Host host) : base(host)
    {

    }

    [Test]
    public void Default()
    {

    }

    [Test]
    public async System.Threading.Tasks.Task ToMimeMessage_rejects_a_recipient_list_with_no_usable_addresses()
    {
        var dto = new EmailDto
        {
            To = new List<string> { " ", "" },
            Name = "Sender",
            Address = "sender@example.com",
            Subject = "Subject",
            Body = "Body"
        };

        await Assert.That(() => dto.ToMimeMessage(NullLogger.Instance)).Throws<InvalidOperationException>();
    }
}
