using UnityEngine;
using System.Collections.Generic;

public struct GeneData
{
    public Color color;
    public float scale;

    public GeneData(Color color, float scale)
    {
        this.color = color;
        this.scale = scale;
    }
}

public static class Genetics
{
    private const float MIN_BLEND = 0.25f;
    private const float MAX_BLEND = 0.75f;

    public static GeneData Combine(GeneData parentA, GeneData parentB, MutationTable table, System.Random rng, out bool mutated)
    {
        mutated = false;
        Color color = InheritColor(parentA.color, parentB.color, rng);
        float scale = PickFromParent(parentA.scale, parentB.scale, rng);

        if (ShouldMutate(table, rng) && TryGetMutationColor(table, parentA.color, parentB.color, rng, out Color mutationColor))
        {
            color = mutationColor;
            mutated = true;
        }

        if (ShouldMutate(table, rng))
        {
            scale = Mathf.Lerp(table.MinScale, table.MaxScale, (float)rng.NextDouble());
            mutated = true;
        }

        return new GeneData(color, scale);
    }

    public static float InheritStat(float a, float b, MutationTable table, System.Random rng, out bool mutated)
    {
        if (ShouldMutate(table, rng))
        {
            mutated = true;
            float multiplier = Mathf.Lerp(table.MinStatMultiplier, table.MaxStatMultiplier, (float)rng.NextDouble());
            return Mathf.Round(Mathf.Max(a, b) * multiplier);
        }

        mutated = false;
        return PickFromParent(a, b, rng);
    }

    private static bool ShouldMutate(MutationTable table, System.Random rng)
    {
        return table != null && rng.NextDouble() < table.MutationChance;
    }

    private static bool TryGetMutationColor(MutationTable table, Color a, Color b, System.Random rng, out Color result)
    {
        List<Color> options = table.MutationColors.FindAll(c => c != a && c != b);

        if (options.Count == 0)
        {
            result = default;
            return false;
        }

        result = options[rng.Next(options.Count)];
        return true;
    }

    public static float PickFromParent(float a, float b, System.Random rng)
    {
        return rng.NextDouble() < 0.5 ? a : b;
    }

    private static Color InheritColor(Color a, Color b, System.Random rng)
    {
        float blend = Mathf.Lerp(MIN_BLEND, MAX_BLEND, (float)rng.NextDouble());
        return Color.Lerp(a, b, blend);
    }
}