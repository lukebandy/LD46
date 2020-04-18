using System.Collections;
using System.Collections.Generic;
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
    public Seasons season;
    public enum Seasons { Spring, Autumn, Summer, Winter }
    public float growTime;
}