using NUnit.Framework;
using UnityEngine;

public class GeneticsTests
{
    private const int RUNS = 100;

    [Test]
    public void Combine_ColourContainsBothParents()
    {
        System.Random rng = new System.Random(1);
        GeneData red = new GeneData(Color.red, 1f);
        GeneData blue = new GeneData(Color.blue, 1f);

        for (int i = 0; i < RUNS; i++)
        {
            Color child = Genetics.Combine(red, blue, rng).color;
            Assert.That(child.r, Is.InRange(0.25f, 0.75f));
            Assert.That(child.b, Is.InRange(0.25f, 0.75f));
        }
    }

    [Test]
    public void Combine_ScaleComesFromAParent()
    {
        System.Random rng = new System.Random(1);
        GeneData small = new GeneData(Color.white, 0.8f);
        GeneData large = new GeneData(Color.white, 1.2f);

        for (int i = 0; i < RUNS; i++)
        {
            float scale = Genetics.Combine(small, large, rng).scale;
            Assert.That(scale == 0.8f || scale == 1.2f);
        }
    }

    [Test]
    public void PickFromParent_UsesBothParentsOverTime()
    {
        System.Random rng = new System.Random(1);
        bool pickedA = false;
        bool pickedB = false;

        for (int i = 0; i < RUNS; i++)
        {
            float value = Genetics.PickFromParent(1f, 2f, rng);
            pickedA |= value == 1f;
            pickedB |= value == 2f;
        }

        Assert.IsTrue(pickedA && pickedB);
    }
}