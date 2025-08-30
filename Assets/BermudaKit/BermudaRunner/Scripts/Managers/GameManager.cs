using Ali.Helper;
using Bermuda.Runner;
using System.Collections;
using DG.Tweening;
using Lean.Touch;
//using ElephantSDK;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : LocalSingleton<GameManager>
{
    [Space][Header("REFERENCES")]
    [SerializeField] private Button _tapToPlayButton;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private LeanTouch _leanTouch;
    [SerializeField] private bool _deletePrefs;

    [Space] [Header("OTHER REFERENCES")]
    private BermudaLevel _currentLevel;

    [Space] [Header("TIME SCALE SETTINGS")] 
    [SerializeField] private float _slowMotionTimeScaleValue = 0.2f;
    public bool IsGamePlayFinished { get; set; } = false;
    public bool IsGamePlayStarted { get; set; } = false;
    public delegate void GameManagerEvent();
    public static event GameManagerEvent OnGameplayStarted;
    public static event GameManagerEvent OnGameplayEnded;

    private void Start()
    {
        if(_deletePrefs)
        {
            PlayerPrefs.DeleteAll();
        }
        Application.targetFrameRate = 60;
        Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale = _slowMotionTimeScaleValue;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale = 1f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Time.timeScale = 0f;
        }
    }

    private void Init()
    {
        HCLevelManager.Instance.Init();
        GenerateLevel();
        ColorOperation();
        CurrencyManager.Instance.Init(); // UIManager Initten üstte olmalı.
        UIManager.Instance.Init(); // UpgradeManager Initten üstte olmalı.
        GrindBar.Instance.Init();
        UpgradeManager.Instance.Init();
        //TimeGunController.Instance.Init();
        BermudaRunnerCharacter.Instance.Init();
        TextPopupUI.Instance.Init();
        HexagonEnding.Instance.Init();
        ScoreTable.Instance.Init();
        GrindText.Instance.Init();
        //_currentLevel.Init();
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
    
    private void OnApplicationQuit()
    {
        DOTween.KillAll();
    }
    private void GenerateLevel()
    {
        if(_currentLevel)
        {
            Destroy(_currentLevel.gameObject);
        }
        HCLevelManager.Instance.GenerateCurrentLevel();
        _currentLevel = HCLevelManager.Instance.GetCurrentLevel().GetComponent<BermudaLevel>();
    }
    
    public void StartGameplay() // GAMEPLAY START
    {
        IsGamePlayStarted = true;
        IsGamePlayFinished = false;
        HapticManager.Haptic(0);
        CameraController.Instance.SetPov(CameraPov.Gameplay);
        RunnerPlayer.Instance.StartToTun();
        OnGameplayStarted?.Invoke();
        //Elephant.LevelStarted(HCLevelManager.Instance.GetGlobalLevelIndex() + 1);
    }

    public void FinishGamePlay(bool success)
    {
        if(IsGamePlayFinished)
        {
            return;
        }
        IsGamePlayFinished = true;

        if (success) // WIN
        {
            //Elephant.LevelCompleted(HCLevelManager.Instance.GetGlobalLevelIndex() + 1); // level completed
            HCLevelManager.Instance.LevelUp();
            StartCoroutine(WinProcess());
            HapticManager.Haptic(0);
        }
        else // LOSE
        {
            //Elephant.LevelFailed(HCLevelManager.Instance.GetGlobalLevelIndex() + 1);
            UIManager.Instance.ShowLosePanel(true);
            HapticManager.Haptic(1);
        }
        
        OnGameplayEnded?.Invoke();
    }

    private IEnumerator WinProcess()
    {
        PreWinPanel.Instance.Show(2f);
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowWinPanel(true);
    }
    
    private static void ColorOperation()
    {
        var levelIndex = HCLevelManager.Instance.GetGlobalLevelIndex();
        var colorIndex = ((levelIndex + 1) / 10) % 5 + 1; // 0-9 için 1, 10-19 için 2, 20-29 için 3...

        EnvironmentManager.ChangeColors(colorIndex);
    }

    public BermudaLevel GetCurrentLevel() => _currentLevel;
    public GameConfig GameConfig() => _gameConfig;
    public LeanTouch GetLeanTouch() => _leanTouch;
}
