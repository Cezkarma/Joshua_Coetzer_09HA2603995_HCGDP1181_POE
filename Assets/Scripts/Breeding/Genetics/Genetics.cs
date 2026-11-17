using UnityEngine;
using System.Collections.Generic;

// Pure inheritance maths used by the Stat classes. Kept free of MonoBehaviours so it can be unit tested.
public static class Genetics
{
    private const float MIN_BLEND = 0.25f;
    private const float MAX_BLEND = 0.75f;
    private const float HUE_TOLERANCE = 30f;

    // Any numeric trait: pick one parent's value, or mutate stronger than both.
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

    // Colour trait: blend both parents' hues, or mutate to a hue neither parent has.
    public static float InheritHue(float a, float b, MutationTable table, System.Random rng, out bool mutated)
    {
        if (ShouldMutate(table, rng) && TryGetMutationHue(table, a, b, rng, out float mutationHue))
        {
            mutated = true;
            return mutationHue;
        }

        mutated = false;
        float blend = Mathf.Lerp(MIN_BLEND, MAX_BLEND, (float)rng.NextDouble());
        return BlendHue(a, b, blend);
    }

    // Size trait: pick one parent's size, or mutate to a random size in the table's range.
    public static float InheritScale(float a, float b, MutationTable table, System.Random rng, out bool mutated)
    {
        if (ShouldMutate(table, rng))
        {
            mutated = true;
            return Mathf.Lerp(table.MinScale, table.MaxScale, (float)rng.NextDouble());
        }

        mutated = false;
        return PickFromParent(a, b, rng);
    }

    public static float PickFromParent(float a, float b, System.Random rng)
    {
        return rng.NextDouble() < 0.5 ? a : b;
    }

    // Blends around the colour wheel the short way, so red (350) + red (10) stays red instead of becoming cyan.
    public static float BlendHue(float a, float b, float t)
    {
        return Mathf.Repeat(a + Mathf.DeltaAngle(a, b) * t, 360f);
    }

    public static float ColorToHue(Color color)
    {
        Color.RGBToHSV(color, out float h, out _, out _);
        return h * 360f;
    }

    private static bool ShouldMutate(MutationTable table, System.Random rng)
    {
        return table != null && rng.NextDouble() < table.MutationChance;
    }

    private static bool TryGetMutationHue(MutationTable table, float a, float b, System.Random rng, out float result)
    {
        List<float> options = new List<float>();
        foreach (Color color in table.MutationColors)
        {
            float hue = ColorToHue(color);
            if (Mathf.Abs(Mathf.DeltaAngle(hue, a)) > HUE_TOLERANCE && Mathf.Abs(Mathf.DeltaAngle(hue, b)) > HUE_TOLERANCE)
            {
                options.Add(hue);
            }
        }

        if (options.Count == 0)
        {
            result = 0;
            return false;
        }

        result = options[rng.Next(options.Count)];
        return true;
    }
}
