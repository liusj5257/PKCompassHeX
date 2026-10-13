using System.Linq;
using FluentAssertions;
using PKHeX.Core.AutoMod;
using Xunit;

namespace PKHeX.Core.Tests.AutoMod;

public class BaseSpeciesOnlyTests
{
    [Fact]
    public void GenerateLivingDexUsesLevelOneEggsAndLowestNonEggEncounter()
    {
        var trainer = new SimpleTrainerInfo(GameVersion.SL);
        var personal = new FilteredPersonalTable(PersonalTable.SV, (ushort)Species.Pichu, (ushort)Species.Pikachu, (ushort)Species.Koraidon);
        var config = new LivingDexConfig { BaseSpeciesOnly = true };
        var previousUseTrainerData = APILegality.UseTrainerData;
        APILegality.UseTrainerData = false;

        try
        {
            var pokemon = trainer.GenerateLivingDex(personal, config).ToArray();

            pokemon.Select(z => z.Species).Should().Contain((ushort)Species.Pichu)
                .And.Contain((ushort)Species.Koraidon)
                .And.NotContain((ushort)Species.Pikachu);

            var egg = pokemon.Single(z => z.Species == (ushort)Species.Pichu);
            var eggEncounter = new LegalityAnalysis(egg).EncounterMatch;
            eggEncounter.Should().BeAssignableTo<IEncounterEgg>();
            egg.CurrentLevel.Should().Be(1);
            egg.IVs.Should().OnlyContain(z => z == 31);

            var nonEgg = pokemon.Single(z => z.Species == (ushort)Species.Koraidon);
            var nonEggEncounter = new LegalityAnalysis(nonEgg).EncounterMatch;
            nonEggEncounter.Should().NotBeAssignableTo<IEncounterEgg>();
            nonEgg.CurrentLevel.Should().Be(nonEggEncounter.LevelMin);
        }
        finally
        {
            APILegality.UseTrainerData = previousUseTrainerData;
        }
    }

    private sealed class FilteredPersonalTable(IPersonalTable inner, params ushort[] species) : IPersonalTable
    {
        public ushort MaxSpeciesID => inner.MaxSpeciesID;
        public int Count => inner.Count;
        public PersonalInfo this[int index] => inner[index];
        public PersonalInfo this[ushort speciesId, byte form] => inner[speciesId, form];
        public int GetFormIndex(ushort speciesId, byte form) => inner.GetFormIndex(speciesId, form);
        public PersonalInfo GetFormEntry(ushort speciesId, byte form) => inner.GetFormEntry(speciesId, form);
        public bool IsSpeciesInGame(ushort speciesId) => species.Contains(speciesId) && inner.IsSpeciesInGame(speciesId);
        public bool IsPresentInGame(ushort speciesId, byte form) => species.Contains(speciesId) && inner.IsPresentInGame(speciesId, form);
    }
}
