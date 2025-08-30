using Ali.Helper.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DestructibleHexagon : MonoBehaviour
{
    [SerializeField] private int _startHp = 100;
    [Space]
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private ParticleSystem _destructParticle;
    [SerializeField] private Collider _collider;

    private int _hp = 100;
    private Cash _cash;
    private bool _isDestructed = false;
    
    public void ResetHexagon()
    {
        if (_cash != null)
        {
            PoolManager.Instance.DespawnCash(_cash.gameObject);
            _cash = null;
        }
        _cash = PoolManager.Instance.SpawnCash().GetComponent<Cash>();
        if (_cash)
        {
            _cash.GetComponent<Collider>().enabled = false;
            _cash.transform.SetParent(_modelTransform);
            _cash.transform.SetLocalPositionAndRotation(new Vector3(0f,0,1.318663f), Quaternion.Euler(60f,-90f,-90f));
            _cash.transform.localScale = Vector3.one;
            _cash.IsCollected = false;
            _cash.SetIsMoving(false);
        }

        _isDestructed = false;
        _hp = _startHp;
        _hpText.text = _hp.ToString();
        _hpText.gameObject.SetActive(true);
        _modelTransform.gameObject.SetActive(true);
        _collider.enabled = true;
    }

    public void Hit(int damage = 1)
    {
        _hp = Mathf.Max(_hp - damage, 0);
        _hpText.text = _hp.ToString();
        if (_hp == 0 && !_isDestructed)
        {
            _isDestructed = true;
            _destructParticle?.Play();
            _cash.transform.SetParent(transform);
            _modelTransform.gameObject.SetActive(false);
            _hpText.gameObject.SetActive(false);
            _collider.enabled = false;

            CashJumpProcess();
            HapticManager.Haptic(0);
        }
        else
        {
            _modelTransform.DOKill(true);
            _modelTransform.DOPunchScale(Vector3.one * 0.07f, 0.15f, 6);
        }
        AudioPool.Instance.PlayClipByName("hexHit", false, 0.1f);
    }

    public void SetHp(int hp)
    {
        _startHp = hp;
        _hp = hp;
        _hpText.text = _hp.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Projectile>())
        {
            var projectile = other.GetComponent<Projectile>();
        
            if (projectile.IsTriggered) return;
            var hitPoint = projectile.transform.position;
            var hitParticle = PoolManager.Instance.SpawnHitParticle().GetComponent<HitParticleController>();
        
            hitParticle.PlayHitParticle(hitPoint);
            
            projectile.IsTriggered = true;
            projectile.transform.DOKill();
            PoolManager.Instance.DespawnProjectile(projectile.gameObject);
            var damage = Mathf.FloorToInt(UpgradeManager.Instance.Damage);
            Hit(damage);
            HapticManager.Haptic(0);
        }
        else if (other.CompareTag("Player") && !_isDestructed)
        {
            GameManager.Instance.FinishGamePlay(true);
        }
    }

    private void CashJumpProcess()
    {
        _cash.transform.SetParent(null);
        _cash.GetComponent<Collider>().enabled = true;
        Vector3 targetPos = new Vector3(transform.position.x, 0.02f, transform.position.z);
        
        _cash.transform.DOScale(Vector3.one * 0.6f, 0.5f);
        _cash.transform.DOJump(targetPos, 1f, 1, 0.5f).OnComplete(() =>
        {
            _cash.transform.DOJump(_cash.transform.position, 0.25f, 2, 0.5f).OnComplete(() =>
            {
                _cash.SetIsMoving(true);
            });
        });
    }

    public bool GetIsDestructed() => _isDestructed;
}