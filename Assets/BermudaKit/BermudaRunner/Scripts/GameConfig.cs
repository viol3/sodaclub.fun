using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configurations/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Move Settings")] 
    [SerializeField] private float _startDistance = 5f;
    [SerializeField] private float _forwardSpeed = 2.25f;
    [SerializeField] private float _strafeSpeed = 1.25f;
    [SerializeField] private float _strafeLerpSpeed = 4f;
    [SerializeField] private float _clampLocalX = 2f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _rotateAngle = 100f;
    [SerializeField] private float _swerveSpeedMultiplier = 1f;
    [SerializeField] private bool _moveEnabled = true;
    [SerializeField] private bool _rotateEnabled = false;
    [SerializeField] private bool _swerve = true;
    
    [Space(50)] [Header("Upgrade Settings")]
    [Space] [Header("DAMAGE")]
    [SerializeField] private float _damageStartPrice = 50f;
    [SerializeField] private float _damageStartIncrementalPrice = 50f;
    [SerializeField] private float _damageAddIncrementalPrice = 100f;
    [Space]
    [SerializeField] private int _damageMaxLevel = 200;
    [SerializeField] private float _damageAtMinLevel = 1f;
    [SerializeField] private float _damageAtMaxLevel = 50f;
    [SerializeField] private float _damageIncrementMultiplier = 1f;
    
    [Space] [Header("FIRE RATE")]
    [SerializeField] private float _fireRateStartPrice = 50f;
    [SerializeField] private float _fireRateStartIncrementalPrice = 50f;
    [SerializeField] private float _fireRateAddIncrementalPrice = 100f;
    [Space]
    [SerializeField] private int _fireRateMaxLevel = 50;
    [SerializeField] private float _fireRateAtMinLevel = 0.75f;
    [SerializeField] private float _fireRateAtMaxLevel = 2f;
    [SerializeField] private float _fireRateIncrementMultiplier = 1f;

    [Space] [Header("FIRE RANGE")]
    [SerializeField] private float _fireRangeStartPrice = 50f;
    [SerializeField] private float _fireRangeStartIncrementalPrice = 50f;
    [SerializeField] private float _fireRangeAddIncrementalPrice = 100f;
    [Space]
    [SerializeField] private int _fireRangeMaxLevel = 75;
    [SerializeField] private float _fireRangeAtMinLevel = 15f;
    [SerializeField] private float _fireRangeAtMaxLevel = 30f;
    [SerializeField] private float _fireRangeIncrementMultiplier = 1;
    
    [Space] [Header("INCOME")]
    [SerializeField] private float _incomeStartPrice = 50f;
    [SerializeField] private float _incomeStartIncrementalPrice = 50f;
    [SerializeField] private float _incomeAddIncrementalPrice = 100f;
    
    [Space(50)] [Header("Grind Bar Settings")]
    [SerializeField] private int[] _grindBarLevelThresholds;

    [Space(50)] [Header("Projectile Settings")] 
    [SerializeField] private Material[] _projectileMaterials;
    [SerializeField] private float _spreadShotAngle = 60f;
    [SerializeField] private float _spreadShotProjectilesSpacing = 0.25f;
    [SerializeField] private float _projectileThrowSpeed = 20f;
    
    [Space(50)] [Header("Skybox and Ground Color")]
    [Header("1 - 10 Level Colors")]
    [SerializeField] private Color _groundColor1;
    [SerializeField] private Material _skyboxMaterial1;
    [Space]
    [Header("Skybox and Ground Color")]
    [Header("10 - 20 Level Colors")]
    [SerializeField] private Color _groundColor2;
    [SerializeField] private Material _skyboxMaterial2;
    [Space]
    [Header("20 - 30 Level Colors")]
    [SerializeField] private Color _groundColor3;
    [SerializeField] private Material _skyboxMaterial3;
    
    [Space(50)] [Header("GAME BALANCE SETTINGS")] 
    [SerializeField] private float _hangeableObjectHitMultiplier = 1.0f;
    [SerializeField] private float partsForForGrindBarPointValue = 0.1f;

    #region PROPERTIES

    #region MOVE SETTINGS PROPERTIES
    public float StartDistance { get => _startDistance; private set => _startDistance = value; } // Player start position on platform.
    public float ForwardSpeed { get => _forwardSpeed; set => _forwardSpeed = value; } // Player moving forward speed
    public float StrafeSpeed { get => _strafeSpeed; private set => _strafeSpeed = value; } // Player strafing speed
    public float StrafeLerpSpeed { get => _strafeLerpSpeed; private set => _strafeLerpSpeed = value; } // Player strafing lerp speed, it's for smooth strafe
    public float ClampLocalX { get => _clampLocalX; private set => _clampLocalX = value; } // Player local x bounds on platform
    public float RotateSpeed { get => _rotateSpeed; private set => _rotateSpeed = value; } // Player rotate speed
    public float RotateAngle { get => _rotateAngle; private set => _rotateAngle = value; } // Player rotate angle
    public float SwerveSpeedMultiplier { get => _swerveSpeedMultiplier; private set => _swerveSpeedMultiplier = value; } // Player swerve multiplier
    public bool MoveEnabled { get => _moveEnabled; set => _moveEnabled = value; } // Move enabled or not bool value
    public bool RotateEnabled { get => _rotateEnabled; set => _rotateEnabled = value; } // Rotation enabled or not bool value
    public bool CanSwerve { get => _swerve; set => _swerve = value; } // Can player swerve?

    #endregion

    #region UPGRADE SETTINGS PROPERTIES
    /// <summary>
    /// Time Value Settings
    /// </summary>
    public float DamageStartPrice { get => _damageStartPrice; private set => _damageStartPrice = value; } // DAMAGE Upgrade Button Start upgrade price.
    public float DamageStartIncrementalPrice { get => _damageStartIncrementalPrice; private set => _damageStartIncrementalPrice = value; } // DAMAGE Upgrade Button Start Incremental Price
    public float DamageAddIncrementalPrice { get => _damageAddIncrementalPrice; private set => _damageAddIncrementalPrice = value; } // DAMAGE Upgrade Button Add Incremental Price
    public int DamageMaxLevel { get => _damageMaxLevel; private set => _damageMaxLevel = value; } // Max DAMAGE Level
    public float DamageAtMinLevel { get => _damageAtMinLevel; private set => _damageAtMinLevel = value; } // DAMAGE value at minimum level
    public float DamageAtMaxLevel { get => _damageAtMaxLevel; private set => _damageAtMaxLevel = value; } // DAMAGE value at maximum level
    public float DamageIncrementMultiplier { get => _damageIncrementMultiplier; set => _damageIncrementMultiplier = value; }
    /// <summary>
    /// Fire Rate Settings
    /// </summary>
    public float FireRateStartPrice { get => _fireRateStartPrice; private set => _fireRateStartPrice = value; } // Fire Rate Upgrade Button Start upgrade price.
    public float FireRateStartIncrementalPrice { get => _fireRateStartIncrementalPrice; private set => _fireRateStartIncrementalPrice = value; } // Fire Rate Upgrade Button Start Incremental Price
    public float FireRateAddIncrementalPrice { get => _fireRateAddIncrementalPrice; private set => _fireRateAddIncrementalPrice = value; } // Fire Rate Upgrade Button Add Incremental Price
    public int FireRateMaxLevel { get => _fireRateMaxLevel; private set => _fireRateMaxLevel = value; } // Max Fire Rate Level
    public float FireRateAtMinLevel { get => _fireRateAtMinLevel; private set => _fireRateAtMinLevel = value; } // Fire Rate value at minimum level
    public float FireRateAtMaxLevel { get => _fireRateAtMaxLevel; private set => _fireRateAtMaxLevel = value; } // Fire Rate value at maximum level
    public float FireRateIncrementMultiplier { get => _fireRateIncrementMultiplier; set => _fireRateIncrementMultiplier = value; }
    /// <summary>
    /// Fire Range Settings
    /// </summary>
    public float FireRangeStartPrice { get => _fireRangeStartPrice; private set => _fireRangeStartPrice = value; } // Fire Range Upgrade Button Start upgrade price.
    public float FireRangeStartIncrementalPrice { get => _fireRangeStartIncrementalPrice; private set => _fireRangeStartIncrementalPrice = value; } // Fire Range Upgrade Button Start Incremental Price
    public float FireRangeAddIncrementalPrice { get => _fireRangeAddIncrementalPrice; private set => _fireRangeAddIncrementalPrice = value; } // Fire Range Upgrade Button Add Incremental Price
    public int FireRangeMaxLevel { get => _fireRangeMaxLevel; private set => _fireRangeMaxLevel = value; } // Max Fire Range Level
    public float FireRangeAtMinLevel { get => _fireRangeAtMinLevel; private set => _fireRangeAtMinLevel = value; } // Fire Range value at minimum level
    public float FireRangeAtMaxLevel { get => _fireRangeAtMaxLevel; private set => _fireRangeAtMaxLevel = value; } // Fire Range value at maximum level
    public float FireRangeIncrementMultiplier { get => _fireRangeIncrementMultiplier; set => _fireRangeIncrementMultiplier = value; }
    /// <summary>
    /// Income Settings
    /// </summary>
    public float IncomeStartPrice { get => _incomeStartPrice; private set => _incomeStartPrice = value; } // Income Upgrade Button Start upgrade price.
    public float IncomeStartIncrementalPrice { get => _incomeStartIncrementalPrice; private set => _incomeStartIncrementalPrice = value; } // Income Upgrade Button Start Incremental Price
    public float IncomeAddIncrementalPrice { get => _incomeAddIncrementalPrice; private set => _incomeAddIncrementalPrice = value; } // Income Upgrade Button Add Incremental Price
    #endregion

    #region GRIND BAR SETTINGS PROPERTIES
    public int[] GrindBarLevelThresholds { get => _grindBarLevelThresholds; set => _grindBarLevelThresholds = value; }

    #endregion

    #region PROJECTILE SETTINGS PROPERTIES
    public Material[] ProjectileMaterials { get => _projectileMaterials; private set => _projectileMaterials = value; }
    public float ProjectileThrowSpeed { get => _projectileThrowSpeed; private set => _projectileThrowSpeed = value; }
    public float SpreadShotAngle { get => _spreadShotAngle; private set => _spreadShotAngle = value; }
    public float SpreadShotProjectilesSpacing { get => _spreadShotProjectilesSpacing; private set => _spreadShotProjectilesSpacing = value; }

    #endregion

    #region SKYBOX SETTINGS PROPERITES

    public Material GetSkyBox1()
    {
        return _skyboxMaterial1;
    }
    public Material GetSkyBox2()
    {
        return _skyboxMaterial2;
    }
    public Material GetSkyBox3()
    {
        return _skyboxMaterial3;
    }
    public Color GetGroundColor1()
    {
        return _groundColor1;
    }
    public Color GetGroundColor2()
    {
        return _groundColor2;
    }
    public Color GetGroundColor3()
    {
        return _groundColor3;
    }

    #endregion

    #region GAME BALANCE SETTINGS PROPERTIES
    public float HangeableObjectHitMultiplier { get => _hangeableObjectHitMultiplier; private set => _hangeableObjectHitMultiplier = value; }
    public float PartsForForGrindBarPointValue { get => partsForForGrindBarPointValue; private set => partsForForGrindBarPointValue = value; }
    
    #endregion

    #endregion

}
