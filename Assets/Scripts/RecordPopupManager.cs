using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RecordLabelInfo
{
    public string Name;
    public TMPro.TMP_Text Recordlabel;
}

public class RecordPopupManager : MonoBehaviour
{
    [SerializeField] private List<RecordLabelInfo> recordLabelInfos;
    [SerializeField] private TMPro.TMP_Text HighScoreLabel;

    private void Start()
    {
        HighScoreLabel.text = PlayerPrefs.GetInt("BestScore", 0).ToString();

        foreach(RecordLabelInfo info in recordLabelInfos)
        {
            info.Recordlabel.text = PlayerPrefs.GetInt(info.Name, 0).ToString();
        }
    }

}
