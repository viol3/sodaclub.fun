using Ali.Helper.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private int _startHp = 100;
    [Space]
    [SerializeField] private TextMeshPro _hpText;
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private ParticleSystem _destructParticle;
    [SerializeField] private Collider _collider;
    [SerializeField] private Ease _moveEase = Ease.Unset;

    private int _hp = 100;
    private Cash _cash;
    private bool _isDestructed = false;

    private void Start()
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
            _cash.transform.SetLocalPositionAndRotation(new Vector3(0f,3.15f,0f), Quaternion.Euler(0f,-30f,0f));
            _cash.transform.localScale = Vector3.one;
            _cash.IsCollected = false;
            _cash.SetIsMoving(false);
        }
        
        SetHp(_startHp);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Hit();
        }
    }

    public void Hit(int damage = 1)
    {
        int previousHp = _hp;
        _hp = Mathf.Max(_hp - damage, 0);
        _hpText.text = _hp.ToString();
    
        float quarterHp = _startHp / 4f;
        int previousQuarters = Mathf.CeilToInt(previousHp / quarterHp);
        int currentQuarters = Mathf.CeilToInt(_hp / quarterHp);
        int quartersLost = previousQuarters - currentQuarters;
        
        if (_hp == 0 && !_isDestructed)
        {
            _isDestructed = true;
            _destructParticle?.Play();
            //_cash.transform.SetParent(transform);
            _modelTransform.gameObject.SetActive(false);
            _hpText.gameObject.SetActive(false);
            _collider.enabled = false;
            
            JumpProcess(_cash.transform);
            HapticManager.Haptic(0);
        }
        else
        {
            if (quartersLost > 0)
            {
                float newYPosition = _modelTransform.localPosition.y - quartersLost * 0.25f;
                _modelTransform.DOKill(true);
                _modelTransform.DOLocalMoveY(newYPosition, 0.15f).SetEase(Ease.InQuad);
                _destructParticle?.Emit(15);
            }
            
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
            projectile.IsTriggered = true;
            
            var hitPoint = projectile.transform.position;
            var hitParticle = PoolManager.Instance.SpawnHitParticle().GetComponent<HitParticleController>();
            hitParticle.PlayHitParticle(hitPoint);
            
            var damage = Mathf.FloorToInt(UpgradeManager.Instance.Damage);
            Hit(damage);
            
            PoolManager.Instance.DespawnProjectile(projectile.gameObject);
            HapticManager.Haptic(0);
        }
    }

    private void JumpProcess(Transform jumpedObject)
    {
        var cash = jumpedObject.GetComponent<Cash>() ? jumpedObject.GetComponent<Cash>() : null;
        jumpedObject.GetComponent<Collider>().enabled = true;
        jumpedObject.SetParent(null);
        Vector3 targetPos = new Vector3(transform.position.x, 0.02f, transform.position.z);

        jumpedObject.DOScale(Vector3.one, 0.5f);
        if (cash != null)
        {
            cash.GetCashModelsParent().DOScale(Vector3.one * 0.6f, 0.5f);
        }
        
        jumpedObject.DOJump(targetPos, 1f, 1, 0.5f).OnComplete(() =>
        {
            jumpedObject.DOJump(jumpedObject.position, 0.25f, 2, 0.5f).OnComplete(() =>
            {
                if (cash != null)
                {
                    cash.SetIsMoving(true);
                }
            });
        });
    }
}
