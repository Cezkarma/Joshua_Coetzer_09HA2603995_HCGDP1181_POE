using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MutationTable", menuName = "Breeding/Mutation Table")]
public class MutationTable : ScriptableObject
{
    [SerializeField, Range(0f, 1f)] private float mutationChance = 0.05f;
    [SerializeField] private List<Color> mutationColors = new List<Color>();
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 1.4f;
    [SerializeField] private float minStatMultiplier = 1.2f;
    [SerializeField] private float maxStatMultiplier = 1.5f;

    public float MutationChance { get { return mutationChance; } }
    public List<Color> MutationColors { get { return mutationColors; } }
    public float MinScale { get { return minScale; } }
    public float MaxScale { get { return maxScale; } }
    public float MinStatMultiplier { get { return minStatMultiplier; } }
    public float MaxStatMultiplier { get { return maxStatMultiplier; } }

    public static MutationTable Create(float mutationChance, List<Color> mutationColors, float minScale, float maxScale, float minStatMultiplier, float maxStatMultiplier)
    {
        MutationTable table = CreateInstance<MutationTable>();
        table.mutationChance = mutationChance;
        table.mutationColors = mutationColors;
        table.minScale = minScale;
        table.maxScale = maxScale;
        table.minStatMultiplier = minStatMultiplier;
        table.maxStatMultiplier = maxStatMultiplier;
        return table;
    }
}