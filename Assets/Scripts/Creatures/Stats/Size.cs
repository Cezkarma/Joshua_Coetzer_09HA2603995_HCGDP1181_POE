using UnityEngine;

// Visual trait: the trait's Value is the creature's scale multiplier.
public class Size : Stat
{
    public Size(Trait trait) : base(trait)
    {
    }

    private void Start()
    {
        Apply();
    }

    public override bool Inherit(Stat parentA, Stat parentB, MutationTable table, System.Random rng)
    {
        float sizeA = (parentA != null ? parentA : parentB).GetTrait().Value;
        float sizeB = (parentB != null ? parentB : parentA).GetTrait().Value;

        trait.Value = Genetics.InheritScale(sizeA, sizeB, table, rng, out bool mutated);
        trait.MaxValue = trait.Value;
        Apply();
        return mutated;
    }

    public void Apply()
    {
        transform.localScale = Vector3.one * trait.Value;
    }

    public override string ToString()
    {
        return trait.Name + ": " + trait.Value.ToString("0.0") + "x";
    }
}
