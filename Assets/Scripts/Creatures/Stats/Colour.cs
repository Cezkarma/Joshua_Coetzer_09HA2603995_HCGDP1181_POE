using UnityEngine;

// Visual trait: the trait's Value is the body hue in degrees (0-360).
public class Colour : Stat
{
    private static readonly string[] HUE_NAMES = { "Red", "Orange", "Yellow", "Lime", "Green", "Teal", "Cyan", "Sky", "Blue", "Purple", "Magenta", "Pink" };

    [SerializeField] private SkinnedMeshRenderer bodyRenderer;
    [SerializeField, Range(0f, 1f)] private float saturation = 0.6f;

    private MaterialPropertyBlock propertyBlock;

    public Colour(Trait trait) : base(trait)
    {
    }

    private void Awake()
    {
        if (bodyRenderer == null)
        {
            bodyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    private void Start()
    {
        Apply();
    }

    public override bool Inherit(Stat parentA, Stat parentB, MutationTable table, System.Random rng)
    {
        float hueA = (parentA != null ? parentA : parentB).GetTrait().Value;
        float hueB = (parentB != null ? parentB : parentA).GetTrait().Value;

        trait.Value = Genetics.InheritHue(hueA, hueB, table, rng, out bool mutated);
        Apply();
        return mutated;
    }

    public void Apply()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        Color color = Color.HSVToRGB(trait.Value / 360f, saturation, 1f);
        bodyRenderer.GetPropertyBlock(propertyBlock, 0);
        propertyBlock.SetColor("_BaseColor", color);
        bodyRenderer.SetPropertyBlock(propertyBlock, 0);
    }

    public override string ToString()
    {
        int index = Mathf.RoundToInt(Mathf.Repeat(trait.Value, 360f) / 30f) % HUE_NAMES.Length;
        return trait.Name + ": " + HUE_NAMES[index];
    }
}
