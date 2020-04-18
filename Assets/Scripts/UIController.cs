using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour {

    [SerializeField]
    private TextMeshProUGUI balance;
    [SerializeField]
    private TextMeshProUGUI season;
    [SerializeField]
    private TextMeshProUGUI waterRemaining;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        balance.text = "£" + GameController.main.farmValue.ToString();
        season.text = GameController.main.season.ToString() + " - Year " + GameController.main.year.ToString();
        waterRemaining.text = Mathf.Clamp(Mathf.RoundToInt(100 * (Player.main.hoseRemaining / Player.main.hoseCapacity)), 0, 100).ToString() + "%";
    }
}
