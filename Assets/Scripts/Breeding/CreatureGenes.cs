using UnityEngine;

public class CreatureGenes : MonoBehaviour
{
    [SerializeField] private string species;
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float scale = 1;

    [SerializeField] private SkinnedMeshRenderer bodyRenderer;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        if (bodyRenderer == null)
        {
            bodyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    private void Start()
    {
        ApplyGenes();
    }

    public void ApplyGenes()
    {
        transform.localScale = new Vector3(scale, scale, scale);

        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        bodyRenderer.GetPropertyBlock(propertyBlock, 0);
        propertyBlock.SetColor("_BaseColor", color);
        bodyRenderer.SetPropertyBlock(propertyBlock, 0);
    }
}
