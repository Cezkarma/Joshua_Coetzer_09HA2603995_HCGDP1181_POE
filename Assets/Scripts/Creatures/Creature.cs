using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [SerializeField] private string creatureName;
    [SerializeField] private string species;
    [SerializeField]
    private List<Stat> stats;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = GetStat("Speed").GetTrait().Value;
    }

    void LateUpdate()
    {
        TickStats();
    }

    private void TickStats()
    {
        foreach (Stat stat in stats)
        {
            if (stat is ITick tick)
            {
                if (stat.GetTrait().Value >= 0)
                {
                    tick.Tick();
                    
                }
            }
        }
    }

    public string GetName()
    {
        return creatureName;
    }

    public void SetName(string newName)
    {
        creatureName = newName;
    }

    public Stat GetStat(string statName)
    {
        return stats.Find(stat => stat.GetTrait().Name == statName);
    }

    public void SubtractStat(string statName, float value)
    {
        Stat stat = GetStat(statName);
        if (stat != null)
        {
            stat.GetTrait().Value -= value;
        }

        if (stat.GetTrait().Value < 0)
        {
            stat.GetTrait().Value = 0;
        }
    }

    public void AddStat(string statName, float value)
    {
        Stat stat = GetStat(statName);
        if (stat != null)
        {
            stat.GetTrait().Value += value;
        }

        if (stat.GetTrait().Value > stat.GetTrait().MaxValue)
        {
            stat.GetTrait().Value = stat.GetTrait().MaxValue;
        }
    }

    public List<Stat> GetStats()
    {
        return stats;
    }

    public string GetSpecies()
    {
        return species;
    }

    // Inherits every trait from both parents. Traits this creature doesn't have yet
    // (e.g. Charge from a Frant parent) are copied over first, so nothing is lost.
    public bool InheritFrom(Creature parentA, Creature parentB, MutationTable table, System.Random rng)
    {
        AddMissingStats(parentA);
        AddMissingStats(parentB);

        bool anyMutated = false;
        foreach (Stat stat in stats)
        {
            string traitName = stat.GetTrait().Name;
            Stat statA = parentA.GetStat(traitName);
            Stat statB = parentB.GetStat(traitName);

            if (statA == null && statB == null)
            {
                continue;
            }

            anyMutated |= stat.Inherit(statA, statB, table, rng);
        }

        return anyMutated;
    }

    private void AddMissingStats(Creature parent)
    {
        foreach (Stat parentStat in parent.GetStats())
        {
            if (GetStat(parentStat.GetTrait().Name) != null)
            {
                continue;
            }

            // Add the same Stat type and copy its serialized settings (trait, tick rate, prefabs...).
            Stat copy = (Stat)gameObject.AddComponent(parentStat.GetType());
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(parentStat), copy);
            stats.Add(copy);
        }
    }
}
