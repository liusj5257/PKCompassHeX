using System.Linq;
using FluentAssertions;
using PKHeX.Core.AutoMod;
using Xunit;

namespace PKHeX.Core.Tests.AutoMod;

public class BaseSpeciesOnlyTests
{
    [Fact]
    public void LivingDexConfigReadsBaseSpeciesOnlyFlag()
    {
        new LivingDexConfig(8).BaseSpeciesOnly.Should().BeTrue();
        new LivingDexConfig(0).BaseSpeciesOnly.Should().BeFalse();
    }

    [Fact]
    public void BaseSpeciesOnlyKeepsBaseFormsAndSetsEggLevelToOne()
    {
        var evolutionTree = EvolutionTree.GetEvolutionTree(EntityContext.Gen9);
        ModLogic.IsBaseSpecies(evolutionTree, Species.Pichu, 0).Should().BeTrue();
        ModLogic.IsBaseSpecies(evolutionTree, Species.Pikachu, 0).Should().BeFalse();

        var egg = new PK7 { Species = (ushort)Species.Pichu, CurrentLevel = 100 };
        var eggEncounter = new EncounterEgg7((ushort)Species.Pichu, 0, GameVersion.US);
        ModLogic.SetBaseSpeciesOnlyLevel(egg, eggEncounter);
        egg.CurrentLevel.Should().Be(1);
    }

    [Fact]
    public void BaseSpeciesOnlyUsesMinimumLevelForNonEggEncounters()
    {
        var pokemon = new PK9 { Species = (ushort)Species.Koraidon, CurrentLevel = 100 };
        var encounter = new EncounterStatic9(GameVersion.SL)
        {
            Species = (ushort)Species.Koraidon,
            Level = 68,
            Location = 0,
        };

        ModLogic.SetBaseSpeciesOnlyLevel(pokemon, encounter);

        pokemon.CurrentLevel.Should().Be(encounter.LevelMin);
    }

    [Fact]
    public void BaseSpeciesOnlyGeneratesTinkatinkFromVioletEggEncounter()
    {
        var trainer = new SimpleTrainerInfo(GameVersion.VL);
        var personal = new SingleSpeciesPersonalTable(GameData.GetPersonal(trainer.Version), (ushort)Species.Tinkatink);
        var tinkatink = trainer.GenerateLivingDex(personal, new LivingDexConfig { BaseSpeciesOnly = true })
            .Should().ContainSingle(p => p.Species == (ushort)Species.Tinkatink).Which;

        tinkatink.Version.Should().Be(GameVersion.VL);
        tinkatink.CurrentLevel.Should().Be(1);

        var legality = new LegalityAnalysis(tinkatink);
        legality.Valid.Should().BeTrue(because: legality.Report());
        legality.EncounterMatch.Should().BeOfType<EncounterEgg9>();
    }

    private sealed class SingleSpeciesPersonalTable(IPersonalTable inner, ushort species) : IPersonalTable
    {
        private readonly ushort _species = species;

        public ushort MaxSpeciesID => _species;
        public int Count => inner.Count;
        public PersonalInfo this[int index] => inner[index];
        public PersonalInfo this[ushort species, byte form] => inner[species, form];

        public int GetFormIndex(ushort species, byte form) => inner.GetFormIndex(species, form);
        public PersonalInfo GetFormEntry(ushort species, byte form) => inner.GetFormEntry(species, form);
        public bool IsSpeciesInGame(ushort species) => species == _species && inner.IsSpeciesInGame(species);
        public bool IsPresentInGame(ushort species, byte form) => species == _species && inner.IsPresentInGame(species, form);
    }
}
