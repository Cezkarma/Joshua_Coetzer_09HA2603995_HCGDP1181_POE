using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GeneticsTests
{
    private const int RUNS = 100;
    private const float RED = 0f;
    private const float GREEN = 120f;
    private const float BLUE = 240f;

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
    public void InheritHue_BlendsBetweenParents()
    {
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            float hue = Genetics.InheritHue(RED, GREEN, null, rng, out _);
            Assert.That(hue, Is.InRange(30f, 90f));
        }
    }

    [Test]
    public void BlendHue_WrapsAroundColourWheel()
    {
        float hue = Genetics.BlendHue(350f, 10f, 0.5f);
        Assert.That(Mathf.Abs(Mathf.DeltaAngle(hue, 0f)), Is.LessThan(0.01f));
    }

    [Test]
    public void InheritScale_ComesFromAParent()
    {
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            float scale = Genetics.InheritScale(0.8f, 1.2f, null, rng, out _);
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
    public void AllTraits_AlwaysMutate_WhenChanceIsOne()
    {
        table = MutationTable.Create(1f, new List<Color> { Color.green }, 0.5f, 0.6f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        float hue = Genetics.InheritHue(RED, BLUE, table, rng, out bool hueMutated);
        float scale = Genetics.InheritScale(1f, 1f, table, rng, out bool scaleMutated);
        Genetics.InheritStat(5f, 5f, table, rng, out bool statMutated);

        Assert.IsTrue(hueMutated && scaleMutated && statMutated);
        Assert.AreEqual(GREEN, hue, 0.01f);
        Assert.That(scale, Is.InRange(0.5f, 0.6f));
    }

    [Test]
    public void AllTraits_NeverMutate_WhenChanceIsZero()
    {
        table = MutationTable.Create(0f, new List<Color> { Color.green }, 0.5f, 0.6f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            Genetics.InheritHue(RED, BLUE, table, rng, out bool hueMutated);
            Genetics.InheritScale(1f, 1f, table, rng, out bool scaleMutated);
            Genetics.InheritStat(5f, 5f, table, rng, out bool statMutated);
            Assert.IsFalse(hueMutated || scaleMutated || statMutated);
        }
    }

    [Test]
    public void MutationHue_IsNeverAParentHue()
    {
        table = MutationTable.Create(1f, new List<Color> { Color.red, Color.green }, 1f, 1f, 1.2f, 1.5f);
        System.Random rng = new System.Random(1);

        for (int i = 0; i < RUNS; i++)
        {
            float hue = Genetics.InheritHue(RED, BLUE, table, rng, out _);
            Assert.AreEqual(GREEN, hue, 0.01f);
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
