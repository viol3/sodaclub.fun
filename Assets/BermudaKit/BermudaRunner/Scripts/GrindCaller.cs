using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindCaller : MonoBehaviour
{
    private bool _isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            if (_isTriggered) return;
            
            _isTriggered = true;
            
            var scoreTable = ScoreTable.Instance;
            var highest = scoreTable.GetScore();
            scoreTable.NewScore++;
            if(scoreTable.NewScore > highest)
                PlayerPrefs.SetInt(ScoreTable.Instance.GetScorePref(), scoreTable.NewScore - 1);
            
            GrindText.Instance.Grind();
        }
    }
}
