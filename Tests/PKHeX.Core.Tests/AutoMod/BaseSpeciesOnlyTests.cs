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
}
