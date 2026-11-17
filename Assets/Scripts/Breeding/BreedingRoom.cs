using UnityEngine;
using System.Collections.Generic;

public class BreedingRoom : Room
{
    private enum BreedingStatus { NotEnough, Ready, TooMany, Cooldown }
    private BreedingStatus currentStatus;

    private const int REQUIRED_CREATURES = 2;

    [SerializeField] private float breedingDuration = 10f;
    [SerializeField] private float cooldownDuration = 15f;
    private float breedingTimer;
    private float cooldownTimer;

    [SerializeField] private List<Creature> creaturePrefabs;

    private System.Random rng = new System.Random();

    [SerializeField] private MutationTable mutationTable;

    private void Update()
    {
        BreedingStatus newStatus;

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            newStatus = BreedingStatus.Cooldown;
        }
        else
        {
            newStatus = GetStatusFromCount(GetRoomCount());
        }

        if (newStatus == BreedingStatus.Ready)
        {
            breedingTimer += Time.deltaTime;

            if (breedingTimer >= breedingDuration)
            {
                Breed();
                breedingTimer = 0;
                cooldownTimer = cooldownDuration;
                newStatus = BreedingStatus.Cooldown;
            }
        }
        else
        {
            breedingTimer = 0;
        }

        if (newStatus != currentStatus)
        {
            currentStatus = newStatus;
            Debug.Log(GetStatusMessage());
        }
    }

    private void Breed()
    {
        if (!IsAlive(children[0]) || !IsAlive(children[1]))
        {
            Debug.Log("A parent is too unalive to breed :(");
            return;
        }

        Creature parentA = children[0].GetComponent<Creature>();
        Creature parentB = children[1].GetComponent<Creature>();

        Creature speciesParent = Random.value < 0.5f ? parentA : parentB;
        Creature prefab = creaturePrefabs.Find(p => p.GetSpecies() == speciesParent.GetSpecies());

        if (prefab == null)
        {
            Debug.LogWarning($"No prefab found for species {speciesParent.GetSpecies()}");
            return;
        }

        Creature baby = Instantiate(prefab, movePoint.position, Quaternion.identity, transform);
        baby.SetName($"Baby {speciesParent.GetSpecies()}");
        bool mutated = baby.InheritFrom(parentA, parentB, mutationTable, rng);

        string message = $"A baby {speciesParent.GetSpecies()} was born!";
        if (mutated)
        {
            message += " It mutated!";
        }

        Debug.Log(message);
        UIManager.Instance.DisplayWarningMessage(message);
    }

    private bool IsAlive(Transform creature)
    {
        if (creature == null)
        {
            return false;
        }

        Stat health = creature.GetComponent<Creature>().GetStat("Health");
        return health == null || health.GetTrait().Value > 0;
    }

    private BreedingStatus GetStatusFromCount(int count)
    {
        if (count < REQUIRED_CREATURES)
        {
            return BreedingStatus.NotEnough;
        }

        if (count == REQUIRED_CREATURES)
        {
            return BreedingStatus.Ready;
        }

        return BreedingStatus.TooMany;
    }

    private string GetStatusMessage()
    {
        switch (currentStatus)
        {
            case BreedingStatus.Ready:
                return $"Breeding... {Mathf.CeilToInt(breedingDuration - breedingTimer)}s left";
            case BreedingStatus.Cooldown:
                return $"Resting... {Mathf.CeilToInt(cooldownTimer)}s left";
            case BreedingStatus.TooMany:
                return $"Too many creatures, only {REQUIRED_CREATURES} can breed";
            default:
                return $"Needs {REQUIRED_CREATURES} creatures to breed";
        }
    }

    public override string ToString()
    {
        return GetStatusMessage();
    }
}
