using UnityEngine;

public class BreedingRoom : Room
{
    private enum BreedingStatus { NotEnough, Ready, TooMany }
    private BreedingStatus currentStatus;

    private const int REQUIRED_CREATURES = 2;

    private void Update()
    {
        int count = GetRoomCount();
        BreedingStatus newStatus;

        if (count < REQUIRED_CREATURES)
            newStatus = BreedingStatus.NotEnough;
        else if (count == REQUIRED_CREATURES)
            newStatus = BreedingStatus.Ready;
        else
            newStatus = BreedingStatus.TooMany;

        if (newStatus != currentStatus)
        {
            currentStatus = newStatus;
            Debug.Log(GetStatusMessage());
        }
    }

    private string GetStatusMessage()
    {
        switch (currentStatus)
        {
            case BreedingStatus.Ready:
                return "Ready to breed!";
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
