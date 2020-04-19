using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Plant")]
public class PlantData : ScriptableObject {
    // Materials
    public Material materialSproutAlive;
    public Material materialSproutDead;
    public Material materialGrowingAlive;
    public Material materialGrowingDead;
    public Material materialGrownAlive;
    public Material materialGrownDead;

    // Timings
    public GameController.Seasons season;
    public float growTime;
    public float dryTime;
}