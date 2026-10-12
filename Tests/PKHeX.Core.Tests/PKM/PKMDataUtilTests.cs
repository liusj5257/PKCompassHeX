using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests.PKM;

public class PKMDataUtilTests
{
    [Fact]
    public void DataHelpersMatchThePKMSerializationWriters()
    {
        var pk = new PK7 { Species = 25 };

        var expectedDecryptedBox = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(expectedDecryptedBox);
        pk.GetDecryptedBoxData().Should().Equal(expectedDecryptedBox);

        var expectedDecryptedParty = new byte[pk.SIZE_PARTY];
        pk.WriteDecryptedDataParty(expectedDecryptedParty);
        pk.GetDecryptedPartyData().Should().Equal(expectedDecryptedParty);

        var expectedEncryptedBox = new byte[pk.SIZE_STORED];
        pk.WriteEncryptedDataStored(expectedEncryptedBox);
        pk.GetEncryptedBoxData().Should().Equal(expectedEncryptedBox);

        var expectedEncryptedParty = new byte[pk.SIZE_PARTY];
        pk.WriteEncryptedDataParty(expectedEncryptedParty);
        pk.GetEncryptedPartyData().Should().Equal(expectedEncryptedParty);
    }
}
