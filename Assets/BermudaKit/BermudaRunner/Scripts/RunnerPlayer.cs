using UnityEngine;
using Bermuda.Runner;
using Ali.Helper;

public class RunnerPlayer : LocalSingleton<RunnerPlayer>
{
    [SerializeField] private BermudaRunnerCharacter _runnerCharacter;

    private void Start()
    {
        if(_runnerCharacter.GetSimpleAnimancer())
            _runnerCharacter.IdleAnimation();
    }

    public void StartToTun()
    {
        _runnerCharacter.StartToRun();
    }
}