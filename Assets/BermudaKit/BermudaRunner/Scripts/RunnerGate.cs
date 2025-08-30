using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Ali.Helper;
using Ali.Helper.Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RunnerGate : MonoBehaviour
{
    [SerializeField] private EffectType _effectType;
    [SerializeField] private float _amount;
    [SerializeField] private bool _isBarricaded = false;
    [SerializeField] private float _barricadeStartHealth;
    [SerializeField] private bool _isProgressibleGate = false;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private float _moveSpeed = 1.75f;
    [SerializeField] private float _increaseThreshold = 1f;
    [Space]
    [SerializeField] private TextMeshPro _amountText;
    [SerializeField] private TextMeshPro _gateTypeText;
    [SerializeField] private SpriteRenderer _labelBack;
    [SerializeField] private SpriteRenderer[] _iconSprites;
    [SerializeField] private ParticleSystem _sunParticle;
    [SerializeField] private int _spriteOrderIndex = 0;
    [SerializeField] private bool _hardIncrease = false;
    [Space]
    [SerializeField] private Renderer[] _gateModels;
    [SerializeField] private Transform _inner;
    [SerializeField] private SpriteRenderer _gradientSprite;
    [Space]
    [SerializeField] private Material _positiveMaterial;
    [SerializeField] private Material _negativeMaterial;
    [SerializeField] private Material _upgradeMaterial;
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Color _negativeColor;
    [SerializeField] private Color _upgradeColor;
    [SerializeField] private Color _notrColor;
    [Space]
    [SerializeField] private RunnerGate _pair;
    [SerializeField] private WoodenBarricade _woodenBarricade;
    [Space] 
    [SerializeField] private GameObject _normalGateModel;
    [SerializeField] private GameObject _progressibleGateModel;
    [SerializeField] private Transform _leftStaminaBar;
    [SerializeField] private Transform _rightStaminaBar;
    [SerializeField] private TextMeshPro _leftBarCurrentLevelText;
    [SerializeField] private TextMeshPro _leftBarNextLevelText;
    [SerializeField] private TextMeshPro _rightBarCurrentLevelText;
    [SerializeField] private TextMeshPro _rightBarNextLevelText;
    [Space]
    [SerializeField] private float _stamina = 0;
    [SerializeField] private float _maxStamina = 3;
    [SerializeField] private float _staminaThresholdMultiplier = 1.25f;
    [SerializeField] private float _scaleMaxY = 1;
    [SerializeField] private float _scaleMinY = 0.025f;
    private float _tempScaleCounter = 0f;
    
    private bool _used = false;
    private Tweener _punchTween;

    private void Start()
    {
        if (_effectType is EffectType.SpreadShot) _amount = 1;
        UpdateAmountText();
        UpdateView();
        CheckMove();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            UpdateGateValues(1);
        }
    }

    private void OnValidate()
    {
        if (_effectType is EffectType.SpreadShot) _amount = 1;
        UpdateAmountText();
        UpdateView();
        _woodenBarricade.SetVisibility(_isBarricaded);
    }

    void UpdateOrders()
    {
        int offset = 10;
        _gradientSprite.sortingOrder = (_spriteOrderIndex * offset) - 5;
        if (_amountText)
        {
            _amountText.sortingOrder = (_spriteOrderIndex * offset) - 4;
        }
        _labelBack.sortingOrder = (_spriteOrderIndex * offset) - 3;
        _gateTypeText.sortingOrder = (_spriteOrderIndex * offset) - 2;
        if (_iconSprites.Length > 0)
        {
            for (int i = 0; i < _iconSprites.Length; i++)
            {
                _iconSprites[i].sortingOrder = (_spriteOrderIndex * offset) - 1;
            }
        }
        _sunParticle.GetComponent<Renderer>().sortingOrder = (_spriteOrderIndex * offset) - 4;
    }

    public void SetGradientColor(Color color)
    {
        _gradientSprite.color = color;
        _labelBack.color = color;
        for (int i = 0; i < _gateModels.Length; i++)
        {
            _gateModels[i].material.SetColor("_Color", color);
        }
    }
    void UpdateAmountText()
    {
        if (_effectType is EffectType.SpreadShot)
        {
            _leftBarCurrentLevelText.text = _amount.ToString(CultureInfo.CurrentCulture);
            _rightBarCurrentLevelText.text = _amount.ToString(CultureInfo.CurrentCulture);

            _leftBarNextLevelText.text = (_amount + 1).ToString(CultureInfo.CurrentCulture);
            _rightBarNextLevelText.text = (_amount + 1).ToString(CultureInfo.CurrentCulture);
            
            foreach (var iconSprite in _iconSprites)
            {
                iconSprite.gameObject.SetActive(false);
            }
            if (_effectType is EffectType.SpreadShot) _iconSprites[0].gameObject.SetActive(true);
            _amountText.enabled = false;
        }
        else
        {
            _amountText.enabled = true;
            foreach (var iconSprite in _iconSprites)
            {
                iconSprite.gameObject.SetActive(false);
            }
        }

        if (_amount > 0)
        {
            _amountText.text = GameUtility.FormatFloatToReadableString(_amount,"", false,false);
            
            if (_amount > 0)
            {
                _amountText.text = "+" + GameUtility.FormatFloatToReadableString(_amount, "", false,false);
            }

            if (_effectType is EffectType.SpreadShot)
            {
                _gradientSprite.color = _upgradeColor;
                _labelBack.color = _upgradeColor;
                foreach (var gate in _gateModels)
                {
                    gate.material = _upgradeMaterial;
                }
            }
            else
            {
                _gradientSprite.color = _positiveColor;
                _labelBack.color = _positiveColor;
                foreach (var gate in _gateModels)
                {
                    gate.material = _positiveMaterial;
                }
            }
        }
        else
        {
            
            _amountText.text = GameUtility.FormatFloatToReadableString(_amount, "", false, false);
            
            if (_amount < 0)
            {
                _amountText.text = GameUtility.FormatFloatToReadableString(_amount, "", false,false);
            }

            
            if (_effectType is EffectType.SpreadShot)
            {
                _gradientSprite.color = _upgradeColor;
                _labelBack.color = _upgradeColor;
                foreach (var gate in _gateModels)
                {
                    gate.material = _upgradeMaterial;
                }
            }
            else
            {
                _gradientSprite.color = _negativeColor;
                _labelBack.color = _negativeColor;
                foreach (var gate in _gateModels)
                {
                    gate.material = _negativeMaterial;
                }
            }
        }
    }

    private void UpdateView()
    {
        if (_isProgressibleGate)
        {
            _progressibleGateModel.SetActive(true);
            _normalGateModel.SetActive(false);
            _amountText.transform.localPosition = Vector3.up * 0.6f;
        }
        else
        {
            _progressibleGateModel.SetActive(false);
            _normalGateModel.SetActive(true);
            _amountText.transform.localPosition = Vector3.up * 0.5f;
        }
        
        _gateTypeText.text = _effectType switch
        {
            EffectType.Damage => "DAMAGE",
            EffectType.FireRate => "FIRE RATE",
            EffectType.FireRange => "FIRE RANGE",
            EffectType.Income => "INCOME",
            EffectType.SpreadShot => "SPREAD SHOT",
            EffectType.LoseAll => "DAMAGE",
            _ => _gateTypeText.text
        };
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_used && _woodenBarricade.IsDestructed)
        {
            Trink();
            _used = true;
            if(_pair != null)
            {
                _pair.GetComponent<BoxCollider>().enabled = false;
            }
        }
        else if (other.GetComponent<Projectile>())
        {
            var projectile = other.GetComponent<Projectile>();
        
            if (projectile.IsTriggered) return;
            projectile.IsTriggered = true;

            var damage = UpgradeManager.Instance.Damage;
            
            if(_isBarricaded && !_woodenBarricade.IsDestructed)
            {
                var hitPoint = projectile.transform.position;
                var hitParticle = PoolManager.Instance.SpawnHitParticle().GetComponent<HitParticleController>();
                hitParticle.PlayHitParticle(hitPoint);
                _woodenBarricade.TakeHit(damage);
                AudioPool.Instance.PlayClipByName("wood", false, 0.1f);
            }
            else
            {
                if(_isProgressibleGate)
                    IncreaseStamina(CalculateReductedDamage(damage * _increaseThreshold));
                else
                {
                    UpdateGateValues(Mathf.FloorToInt(damage * _increaseThreshold));
                }
                
                AudioPool.Instance.PlayClipByName("hexHit", false, 0.1f);
            }
        
            projectile.DOKill();
            PoolManager.Instance.DespawnProjectile(projectile.gameObject);
        }
    }

    private void Trink()
    {
        var upgradeManager = UpgradeManager.Instance;

        if (_effectType is EffectType.Damage)
        {
            upgradeManager.UpdateDamage((int)_amount/5);
        }
        else if (_effectType is EffectType.FireRate)
        {
            upgradeManager.TempFireRate += (int) _amount /5;
            upgradeManager.UpdateFireRate();
        }
        else if (_effectType is EffectType.FireRange)
        {
            upgradeManager.UpdateFireRange((int)_amount/5);
        }
        else if(_effectType is EffectType.Income)
        {
            upgradeManager.UpdateIncome((int)_amount/5);
        }
        else if (_effectType is EffectType.SpreadShot)
        {
            GunController.Instance.SetSpreadShotActive(true);
        }
        else if (_effectType is EffectType.LoseAll)
        {
            upgradeManager.UpdateDamage((int)_amount);
            upgradeManager.TempFireRate += (int)_amount;
            upgradeManager.UpdateFireRate();
            upgradeManager.UpdateFireRange((int)_amount);
            upgradeManager.UpdateIncome((int)_amount);
            GunController.Instance.SetSpreadShotActive(false);
        }

        GetComponent<Collider>().enabled = false;
        _gradientSprite.color = _notrColor;
        AudioPool.Instance.PlayClipByName("gate", false, 0.3f);

        if (_amount > 0)
        {
            HapticManager.Haptic(0);
        }
        else
        {
            HapticManager.Haptic(1);
        }
    }
    private void IncreaseStamina(float damage)
    {
        _leftStaminaBar.transform.DOKill();
        _rightStaminaBar.transform.DOKill();

        _stamina += damage;
        Punch(0.05f, 0.2f);
        
        if (_stamina >= _maxStamina)
        {
            _amount++;
            _stamina = 0;
            _maxStamina *= _staminaThresholdMultiplier;
            UpdateAmountText();
            Punch(0.15f, 0.25f);
        }
        var targetYScale = _stamina / _maxStamina * (_scaleMaxY - _scaleMinY) + _scaleMinY;

        _leftStaminaBar.DOScaleY(targetYScale, 0.25f).SetEase(Ease.OutSine);
        _rightStaminaBar.DOScaleY(targetYScale, 0.25f).SetEase(Ease.OutSine);
    }
    private float CalculateReductedDamage(float baseDamage) // every fifth level for progressible bar the damage divided by 2
    {
        var levelInterval = 5;
        var damageReductionFactor = Mathf.FloorToInt(_amount / levelInterval);

        return baseDamage / (int)Mathf.Pow(2, damageReductionFactor);
    }
    public float GetGateAmount()
    {
        return _amount;
    }

    public EffectType GetEffectType()
    {
        return _effectType;
    }

    private void CheckMove()
    {
        if (_isMoving)
        {
            const float leftPos = -1.15f;
            const float rightPos = 1.15f;
            var moverObject = _inner.transform.parent;
        
            var firstTarget = moverObject.localPosition.x >= 0 ? rightPos : leftPos;
            var secondTarget = moverObject.localPosition.x >= 0 ? leftPos : rightPos;

            moverObject.DOLocalMoveX(firstTarget, _moveSpeed).SetSpeedBased().OnComplete(() =>
            {
                moverObject.DOLocalMoveX(secondTarget, _moveSpeed).SetSpeedBased().SetLoops(-1, LoopType.Yoyo);
            });
        }
    }


    public void SetSpriteOrderIndex(int index)
    {
        _spriteOrderIndex = index;
        UpdateOrders();
    }

    public string GetHeaderText()
    {
        return _gateTypeText.text;
    }

    public string GetInnerText()
    {
        return _amountText.text;
    }

    public void UpdateGateValues(float value)
    {
        if (_used)
        {
            return;
        }
        if(_hardIncrease)
        {
            if(_amount < 3)
            {
                _amount += 0.1f * value;
            }
            else
            {
                _amount += 0.01f * value;
            }
        }
        else
        {
            _amount += 1 * value;
        }
        
        UpdateAmountText();
        Punch();
    }

    void Punch(float punchScale = 0.1f, float duration = 0.25f)
    {
        _punchTween?.Complete();
        _punchTween = _inner.DOPunchScale(_inner.localScale * punchScale, duration);
    }
    public void IncreaseAmount(int amount)
    {
        _amount += amount;
        UpdateAmountText();
        Punch();
    }

    public bool GetIsBarricaded() => _isBarricaded;
    public float GetBarricadeStartHealth() => _barricadeStartHealth;
}


public enum EffectType
{
    Damage, FireRate, FireRange, Income, SpreadShot, LoseAll
}