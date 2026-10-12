namespace PKHeX.Core;

public static class PKMDataUtil
{
    public static byte[] GetDecryptedBoxData(this PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return data;
    }

    public static byte[] GetDecryptedPartyData(this PKM pk)
    {
        var data = new byte[pk.SIZE_PARTY];
        pk.WriteDecryptedDataParty(data);
        return data;
    }

    public static byte[] GetEncryptedBoxData(this PKM pk)
    {
        var data = new byte[pk.SIZE_STORED];
        pk.WriteEncryptedDataStored(data);
        return data;
    }

    public static byte[] GetEncryptedPartyData(this PKM pk)
    {
        var data = new byte[pk.SIZE_PARTY];
        pk.WriteEncryptedDataParty(data);
        return data;
    }
}
