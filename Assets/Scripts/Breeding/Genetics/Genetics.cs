using UnityEngine;

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

    public static GeneData Combine(GeneData parentA, GeneData parentB, System.Random rng)
    {
        Color color = InheritColor(parentA.color, parentB.color, rng);
        float scale = PickFromParent(parentA.scale, parentB.scale, rng);
        return new GeneData(color, scale);
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