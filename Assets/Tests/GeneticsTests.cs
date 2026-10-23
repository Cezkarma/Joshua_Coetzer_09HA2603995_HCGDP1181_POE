using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GeneticsTests
{
    private const int RUNS = 100;

    private MutationTable table;

    [TearDown]
    public void TearDown()
    {
        if (table != null)
        {
            Object.DestroyImmediate(table);
        }
    }

    [Test]
    public void Combine_ColourContainsBothParents()
    {
        System.Random rng = new System.Random(1);
        GeneData red = new GeneData(Color.red, 1f);
        GeneData blue = new GeneData(Color.blue, 1f);

        for (int i = 0; i < RUNS; i++)
        {
            Color child = Genetics.Combine(red, blue, null, rng, out _).color;
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
            float scale = Genetics.Combine(small, large, null, rng, out _).scale;
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

    [Test]
    public void Combine_AlwaysMutates_WhenChanceIsOne()
    {
        table = MutationTable.Create(1f, new List<Color> { Color.green }, 0.5f, 0.6f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        GeneData child = Genetics.Combine(new GeneData(Color.red, 1f), new GeneData(Color.blue, 1f), table, rng, out bool mutated);

        Assert.IsTrue(mutated);
        Assert.AreEqual(Color.green, child.color);
        Assert.That(child.scale, Is.InRange(0.5f, 0.6f));
    }

    [Test]
    public void Combine_NeverMutates_WhenChanceIsZero()
    {
        table = MutationTable.Create(0f, new List<Color> { Color.green }, 0.5f, 0.6f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            Genetics.Combine(new GeneData(Color.red, 1f), new GeneData(Color.blue, 1f), table, rng, out bool mutated);
            Assert.IsFalse(mutated);
        }
    }

    [Test]
    public void MutationColour_IsNeverAParentColour()
    {
        table = MutationTable.Create(1f, new List<Color> { Color.red, Color.green }, 1f, 1f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            Color child = Genetics.Combine(new GeneData(Color.red, 1f), new GeneData(Color.blue, 1f), table, rng, out _).color;
            Assert.AreEqual(Color.green, child);
        }
    }

    [Test]
    public void InheritStat_Mutation_IsStrongerThanBothParents()
    {
        table = MutationTable.Create(1f, new List<Color>(), 1f, 1f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        float value = Genetics.InheritStat(5f, 10f, table, rng, out bool mutated);

        Assert.IsTrue(mutated);
        Assert.That(value, Is.GreaterThan(10f));
    }
}
