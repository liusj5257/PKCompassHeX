using System;
using FluentAssertions;
using Xunit;

namespace PKHeX.Core.Tests;

public class MysteryGiftIVTests
{
    [Fact]
    public void RandomGiftIVsAreSetTo31AndSpecifiedIVsArePreserved()
    {
        var gift = new WC9();
        gift.IVs = [255, 15, 252, 22, 254, 0];
        var pokemon = new PK9();
        pokemon.SetIVs([1, 15, 3, 22, 5, 0]);

        gift.SetRandomIVsToMax(pokemon);

        Span<int> actual = stackalloc int[6];
        pokemon.GetIVs(actual);
        actual.ToArray().Should().Equal(31, 15, 31, 22, 31, 0);
    }

    [Fact]
    public void GiftWithNoRandomIVsIsUnchanged()
    {
        var gift = new WC9();
        gift.IVs = [31, 15, 0, 22, 30, 1];
        var pokemon = new PK9();
        pokemon.SetIVs([1, 2, 3, 4, 5, 6]);

        gift.SetRandomIVsToMax(pokemon);

        Span<int> actual = stackalloc int[6];
        pokemon.GetIVs(actual);
        actual.ToArray().Should().Equal(1, 2, 3, 4, 5, 6);
    }
}
