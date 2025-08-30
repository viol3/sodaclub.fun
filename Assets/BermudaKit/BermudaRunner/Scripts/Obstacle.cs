using System.Collections;
using Bermuda.Runner;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Collider[] _colliders;
    private Coroutine _disableColCoroutine;
    private bool _isTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTrigger)
        {
            //GunController.Instance.DowngradeAllGunsLevel();
            StartCoroutine(DisableColliderForAWhile());
            StartCoroutine(PushBackCharacterProcess());
        }
    }

    private IEnumerator PushBackCharacterProcess()
    {
        HapticManager.Haptic(1);
        BermudaRunnerCharacter.Instance.SetForwardSpeed(BermudaRunnerCharacter.Instance.GetInitialSpeed() * -2.5f , 0.25f);
        yield return new WaitForSeconds(0.4f);
        BermudaRunnerCharacter.Instance.SetForwardSpeed(BermudaRunnerCharacter.Instance.GetInitialSpeed() , 0.25f);
    }

    private IEnumerator DisableColliderForAWhile()
    {
        _isTrigger = true;   
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }
        
        yield return new WaitForSeconds(1f);
        
        foreach (var col in _colliders)
        {
            col.enabled = false;
        }

        _isTrigger = false;
    }
}
