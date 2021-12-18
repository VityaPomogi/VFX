using UnityEngine;
using TMPro;

public class ActionPointGauge : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalEnergyLabel;
    [SerializeField] private TextMeshProUGUI remainingEnergyLabel;

    private int totalActionPoints = 0;
    private int remainingActionPoints = 0;

    public void SetUp( int totalActionPoints, int remainingActionPoints )
    {
        this.totalActionPoints = totalActionPoints;
        this.remainingActionPoints = remainingActionPoints;

        UpdateLabels();
    }

    public void UpdateLabels()
    {
        totalEnergyLabel.text = totalActionPoints.ToString();
        remainingEnergyLabel.text = remainingActionPoints.ToString();
    }

    public void AddActionPoints( int amount )
    {
        remainingActionPoints = Mathf.Clamp( remainingActionPoints + amount, 0, totalActionPoints );
        UpdateLabels();
    }

    public void MinusActionPoints( int amount )
    {
        remainingActionPoints = Mathf.Clamp( remainingActionPoints - amount, 0, totalActionPoints );
        UpdateLabels();
    }
}
