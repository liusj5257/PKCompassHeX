using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PKHeX.Core;

/// <summary>
/// Helper class that stores references to all the event data templates.
/// </summary>
public static class EncounterEvent
{
    #region Pickle Data
    /// <summary>Event Database for Generation 4, loaded externally.</summary>
    public static readonly PCD[] MGDB_G4 = [];

    /// <summary>Event Database for Generation 5, loaded externally.</summary>
    public static readonly PGF[] MGDB_G5 = [];

    /// <summary>Event Database for Generation 6, loaded externally.</summary>
    public static readonly WC6[] MGDB_G6 = [];

    /// <summary>Event Database for Generation 7, loaded externally.</summary>
    public static readonly WC7[] MGDB_G7 = [];

    /// <summary>Event Database for Generation 7 <see cref="EntityContext.Gen7b"/>, loaded externally.</summary>
    public static readonly WB7[] MGDB_G7GG = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8"/>, loaded externally.</summary>
    public static readonly WC8[] MGDB_G8 = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8a"/>, loaded externally.</summary>
    public static readonly WA8[] MGDB_G8A = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8b"/>, loaded externally.</summary>
    public static readonly WB8[] MGDB_G8B = [];

    /// <summary>Event Database for Generation 9 <see cref="EntityContext.Gen9"/>, loaded externally.</summary>
    public static readonly WC9[] MGDB_G9 = [];

    /// <summary>Event Database for Generation 9 <see cref="EntityContext.Gen9a"/>, loaded externally.</summary>
    public static readonly WA9[] MGDB_G9A = [];
    #endregion

    #region Locally Loaded Data
    /// <summary>Event Database for Generation 4</summary>
    public static PCD[] EGDB_G4 { get; private set; } = [];

    /// <summary>Event Database for Generation 5</summary>
    public static PGF[] EGDB_G5 { get; private set; } = [];

    /// <summary>Event Database for Generation 6</summary>
    public static WC6[] EGDB_G6 { get; private set; } = [];

    /// <summary>Event Database for Generation 7</summary>
    public static WC7[] EGDB_G7 { get; private set; } = [];

    /// <summary>Event Database for Generation 7 <see cref="EntityContext.Gen7b"/></summary>
    public static WB7[] EGDB_G7GG { get; private set; } = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8"/></summary>
    public static WC8[] EGDB_G8 { get; private set; } = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8a"/></summary>
    public static WA8[] EGDB_G8A { get; private set; } = [];

    /// <summary>Event Database for Generation 8 <see cref="EntityContext.Gen8b"/></summary>
    public static WB8[] EGDB_G8B { get; private set; } = [];

    /// <summary>Event Database for Generation 9 <see cref="EntityContext.Gen9"/></summary>
    public static WC9[] EGDB_G9 { get; private set; } = [];

    /// <summary>Event Database for Generation 9 <see cref="EntityContext.Gen9a"/></summary>
    public static WA9[] EGDB_G9A { get; private set; } = [];
    #endregion

    /// <summary>
    /// Reloads the locally stored event templates.
    /// </summary>
    /// <param name="paths">External folder(s) to source individual mystery gift template files from.</param>
    public static void RefreshMGDB(params ReadOnlySpan<string> paths)
    {
        // If no paths are provided, clear the arrays. See the bottom of this method.
        HashSet<PCD>? g4 = null; List<PCD>? lg4 = null;
        HashSet<PGF>? g5 = null; List<PGF>? lg5 = null;
        HashSet<WC6>? g6 = null; List<WC6>? lg6 = null;
        HashSet<WC7>? g7 = null; List<WC7>? lg7 = null;
        HashSet<WB7>? b7 = null; List<WB7>? lb7 = null;
        HashSet<WC8>? g8 = null; List<WC8>? lg8 = null;
        HashSet<WB8>? b8 = null; List<WB8>? lb8 = null;
        HashSet<WA8>? a8 = null; List<WA8>? la8 = null;
        HashSet<WC9>? g9 = null; List<WC9>? lg9 = null;
        HashSet<WA9>? a9 = null; List<WA9>? la9 = null;

        // Load external files
        // For each file, load the gift object into the appropriate list.
        foreach (var path in paths)
        {
            if (!Directory.Exists(path))
                continue;
            var gifts = MysteryUtil.GetGiftsFromFolder(path);
            foreach (var gift in gifts)
            {
                var added = gift switch
                {
                    PCD pcd => AddOrExpand(ref g4, ref lg4, pcd),
                    PGF pgf => AddOrExpand(ref g5, ref lg5, pgf),
                    WC6 wc6 => AddOrExpand(ref g6, ref lg6, wc6),
                    WC7 wc7 => AddOrExpand(ref g7, ref lg7, wc7),
                    WB7 wb7 => AddOrExpand(ref b7, ref lb7, wb7),
                    WC8 wc8 => AddOrExpand(ref g8, ref lg8, wc8),
                    WB8 wb8 => AddOrExpand(ref b8, ref lb8, wb8),
                    WA8 wa8 => AddOrExpand(ref a8, ref la8, wa8),
                    WC9 wc9 => AddOrExpand(ref g9, ref lg9, wc9),
                    WA9 wa9 => AddOrExpand(ref a9, ref la9, wa9),
                    _ => false,
                };
                if (!added)
                    Trace.WriteLine($"Failed to add gift in {Path.GetDirectoryName(path)}: {gift.FileName}");

                static bool AddOrExpand<T>([NotNullWhen(true)] ref HashSet<T>? arr, ref List<T>? extra, T obj)
                {
                    if (arr is null)
                    {
                        // Most users won't be adding more than 1-2 gifts
                        // Save memory by initializing the HashSet and List minimally.
                        arr = new HashSet<T>(1);
                        extra = new List<T>(1);
                    }
                    if (arr.Add(obj))
                        extra!.Add(obj);
                    return true;
                }
            }
        }

        EGDB_G4 = SetArray(lg4);
        EGDB_G5 = SetArray(lg5);
        EGDB_G6 = SetArray(lg6);
        EGDB_G7 = SetArray(lg7);
        EGDB_G7GG = SetArray(lb7);
        EGDB_G8 = SetArray(lg8);
        EGDB_G8A = SetArray(la8);
        EGDB_G8B = SetArray(lb8);
        EGDB_G9 = SetArray(lg9);
        EGDB_G9A = SetArray(la9);
        return;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static T[] SetArray<T>(List<T>? update) => update is null ? [] : update.ToArray();
    }

    /// <summary>
    /// Gets all event gifts.
    /// </summary>
    /// <param name="sorted">If true, the result will be sorted by ascending species ID. Otherwise, will be ordered by generation ascending.</param>
    public static IEnumerable<MysteryGift> GetAllEvents(bool sorted = true)
    {
        var regular = new IReadOnlyList<MysteryGift>[]
        {
            MGDB_G4,       EGDB_G4,
            MGDB_G5,       EGDB_G5,
            MGDB_G6,       EGDB_G6,
            MGDB_G7,       EGDB_G7,
            MGDB_G7GG,     EGDB_G7GG,
            MGDB_G8,       EGDB_G8,
            MGDB_G8A,      EGDB_G8A,
            MGDB_G8B,      EGDB_G8B,
            MGDB_G9,       EGDB_G9,
            MGDB_G9A,      EGDB_G9A,
        }.SelectMany(z => z);
        var result = regular.Where(mg => mg is { IsItem: false, IsEntity: true, Species: not 0 });
        if (sorted)
            result = result.OrderBy(mg => mg.Species);
        return result;
    }
}
