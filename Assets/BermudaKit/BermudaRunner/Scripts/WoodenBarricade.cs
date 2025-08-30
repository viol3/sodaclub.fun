using System.Collections;
using Ali.Helper;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WoodenBarricade : MonoBehaviour
{
    [SerializeField] private RunnerGate _runnerGate;
    [SerializeField] private GameObject[] _woodModels;
    [SerializeField] private TextMeshPro _healthText;
    
    private float _currentHealth;
    private Transform _currentWood;
    private Tweener _healthTextPunchTween;
    private Tweener _woodPunchTween;

    public bool IsDestructed { get; set; } = false;

    private void Start()
    {
        SetVisibility(_runnerGate.GetIsBarricaded());
    }
    public void TakeHit(int damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
        UpdateHealthText();
        
        float healthPerWood = _runnerGate.GetBarricadeStartHealth() / _woodModels.Length;
        
        for (int i = _woodModels.Length - 1; i >= 0; i--)
        {
            float woodHealthThreshold = healthPerWood * i;
            if (_currentHealth <= woodHealthThreshold && _woodModels[i].activeInHierarchy)
            {
                DestroyNextWood(_woodModels[i]);
            }
        }

        if (_currentHealth <= 0)
        {
            IsDestructed = true;
            _healthText.gameObject.SetActive(false);
        }
        
        PunchWood();
    }
    private void UpdateHealthText()
    {
        _healthText.text = "-" + GameUtility.FormatFloatToReadableString(_currentHealth, "", false, false);

        if (!Application.isPlaying) return;
        
        _healthTextPunchTween?.Complete();
        _healthTextPunchTween = _healthText.transform.DOPunchScale(_healthText.transform.localScale * 0.25f, 0.25f);

    }

    private void DestroyNextWood(GameObject wood)
    {
        StartCoroutine(DestroyNextWoodProcess(wood));
    }
    
    private IEnumerator DestroyNextWoodProcess(GameObject wood)
    {
        var destructParticle = PoolManager.Instance.SpawnWoodParticle().GetComponent<ParticleSystem>();
        if (destructParticle)
        {
            destructParticle.transform.SetParent(wood.transform);
            destructParticle.transform.localScale = Vector3.one;
            destructParticle.transform.localPosition = new Vector3(0, 0, 1.25f);
            destructParticle.transform.localEulerAngles = Vector3.zero;
            destructParticle.Play();
            destructParticle.transform.SetParent(wood.transform.parent);
        }
        
        wood.SetActive(false);
        yield return null;
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            _currentHealth = _runnerGate.GetBarricadeStartHealth();
            _healthText.gameObject.SetActive(true);
            UpdateHealthText();

            for (var i = 0; i < _woodModels.Length; i++)
            {
                var wood = _woodModels[i];
                if (i == _woodModels.Length-1) _currentWood = wood.transform;
                wood.SetActive(true);
            }

            IsDestructed = false;
        }
        else
        {
            foreach (var wood in _woodModels)
            {
                wood.SetActive(false);
            }
            _healthText.gameObject.SetActive(false);
            IsDestructed = true;
        }
    }

    private void PunchWood()
    {
        _woodPunchTween?.Complete();
        _woodPunchTween = _currentWood.DOPunchScale(_currentWood.localScale * 0.1f, 0.25f);
    }
}