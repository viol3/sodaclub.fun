using Ali.Helper;
using TMPro;
using UnityEngine;

public class ScoreTable : LocalSingleton<ScoreTable>
{
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private string scorePref;
    [SerializeField] private int score;
    public int NewScore { get; set; } = 0;
    
    public void Init()
    {
        if (!PlayerPrefs.HasKey(scorePref))
            score = 0;
        else
            score = PlayerPrefs.GetInt(scorePref);
        
        UpdateScore(score);
    }

    public void UpdateScore(int scoreMeter)
    {
        const int offsetZ = 4;
        
        transform.localPosition = new Vector3(-3f, 1f, scoreMeter*offsetZ  - 1.5f);
        scoreText.text = scoreMeter*5 + " m";
    }

    public string  GetScorePref()
    {
        return scorePref;
    }
    
    public int GetScore()
    {
        return score;
    }
}
