using UnityEngine;

public class BreedingRoom : Room
{
    private enum BreedingStatus { NotEnough, Ready, TooMany, Cooldown }
    private BreedingStatus currentStatus;

    private const int REQUIRED_CREATURES = 2;

    [SerializeField] private float breedingDuration = 10f;
    [SerializeField] private float cooldownDuration = 15f;
    private float breedingTimer;
    private float cooldownTimer;

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
        string message = "Creatures bred!";
        Debug.Log(message);
        UIManager.Instance.DisplayWarningMessage(message);
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
