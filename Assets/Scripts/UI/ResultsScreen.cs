using TMPro;
using UnityEngine;

public class ResultsScreen : MonoBehaviour
{
    public static ResultsScreen Instance { get; private set; }

    [SerializeField] private GameObject resultsScreen;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] public int fishThershhold = 10;
    public void ShowResults(bool survived)
    {
        resultsScreen.SetActive(true);
        winText.gameObject.SetActive(survived);
        loseText.gameObject.SetActive(!survived);
    }
}
