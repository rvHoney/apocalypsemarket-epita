using UnityEngine;
using TMPro;

public class AmoDisplay : MonoBehaviour
{
    #region Singleton
    public static AmoDisplay Instance;
    #endregion
    [SerializeField] private TextMeshProUGUI amoCountText;
    [SerializeField] private TextMeshProUGUI maxAmoText;

    void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void UpdateAmoDisplay(int currentCount, int maxCount)
    {
        amoCountText.text = currentCount.ToString();
        maxAmoText.text = "/" + maxCount.ToString();
    }
}
