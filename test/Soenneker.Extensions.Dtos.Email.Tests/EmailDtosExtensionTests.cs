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
}
