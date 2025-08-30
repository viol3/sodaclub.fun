using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;
//
public class FBIKAnimator : MonoBehaviour
{
    [SerializeField] private FullBodyBipedIK _fbik;
    [SerializeField] private LookAtIK _lookAtIK;
    private Transform _rightHandTarget;
    private Transform _leftHandTarget;
    private Transform _rightFootTarget;
    private Transform _leftFootTarget;

    [HideInInspector]
    public  Transform SpineTarget;

    private Vector3 _rightHandFirstPosition;
    private Vector3 _leftHandFirstPosition;
    private Vector3 _rightFootFirstPosition;
    private Vector3 _leftFootFirstPosition;

    private Vector3 _spineFirstPosition;

    private Quaternion _rightFootFirstRotation;
    private Quaternion _leftFootFirstRotation;
    private void Awake()
    {
        SpineTarget = new GameObject("SpineTarget").transform;
        SpineTarget.position = _fbik.solver.bodyEffector.bone.position;
        SpineTarget.SetParent(_fbik.transform, true);
        _spineFirstPosition = SpineTarget.localPosition;

        _rightHandTarget = new GameObject("RightHandTarget").transform;
        _rightHandTarget.position = _fbik.solver.rightHandEffector.bone.position;
        _rightHandTarget.SetParent(_fbik.transform, true);
        _rightHandFirstPosition = _rightHandTarget.localPosition;

        _leftHandTarget = new GameObject("LeftHandTarget").transform;
        _leftHandTarget.position = _fbik.solver.leftHandEffector.bone.position;
        _leftHandTarget.SetParent(_fbik.transform, true);
        _leftHandFirstPosition = _leftHandTarget.localPosition;

        _leftFootTarget = new GameObject("LeftFootTarget").transform;
        _leftFootTarget.position = _fbik.solver.leftFootEffector.bone.position;
        _leftFootTarget.rotation = _fbik.solver.leftFootEffector.bone.rotation;
        _leftFootTarget.SetParent(_fbik.transform, true);
        _leftFootFirstPosition = _leftFootTarget.localPosition;
        _leftFootFirstRotation = _leftFootTarget.localRotation;

        _rightFootTarget = new GameObject("RightFootTarget").transform;
        _rightFootTarget.position = _fbik.solver.rightFootEffector.bone.position;
        _rightFootTarget.rotation = _fbik.solver.rightFootEffector.bone.rotation;
        _rightFootTarget.SetParent(_fbik.transform, true);
        _rightFootFirstPosition = _rightFootTarget.localPosition;
        _rightFootFirstRotation = _rightFootTarget.localRotation;

        _fbik.solver.rightHandEffector.target = _rightHandTarget;
        _fbik.solver.leftHandEffector.target = _leftHandTarget;
        _fbik.solver.rightFootEffector.target = _rightFootTarget;
        _fbik.solver.leftFootEffector.target = _leftFootTarget;
        _fbik.solver.bodyEffector.target = SpineTarget;
    }

    public void SetLookAtIKWeight(float weight)
    {
        _lookAtIK.solver.SetLookAtWeight(weight);
    }

    public void LocalMoveLeftHandTo(Vector3 position, float duration, bool relative)
    {
        //_leftHandTarget.DOKill();
        _leftHandTarget.DOLocalMove(position, duration).SetRelative(relative);
    }

    public void LocalMoveRightHandTo(Vector3 position, float duration, bool relative)
    {
        //_rightHandTarget.DOKill();
        _rightHandTarget.DOLocalMove(position, duration).SetRelative(relative);
    }

    public void LocalMoveLeftHandBack(float duration)
    {
        //_leftHandTarget.DOKill();
        _leftHandTarget.DOLocalMove(_leftHandFirstPosition, duration);
    }

    public void LocalMoveRightHandBack(float duration)
    {
        //_rightHandTarget.DOKill();
        _rightHandTarget.DOLocalMove(_rightHandFirstPosition, duration);
    }

    public void LocalMoveLeftFootTo(Vector3 position, float duration, bool relative)
    {
        //_leftFootTarget.DOKill();
        _leftFootTarget.DOLocalMove(position, duration).SetRelative(relative);
    }

    public void LocalMoveRightFootTo(Vector3 position, float duration, bool relative)
    {
        //_rightFootTarget.DOKill();
        _rightFootTarget.DOLocalMove(position, duration).SetRelative(relative);
    }

    public void LocalMoveLeftFootBack(float duration)
    {
        //_leftFootTarget.DOKill();
        _leftFootTarget.DOLocalMove(_leftFootFirstPosition, duration);
    }

    public void LocalMoveRightFootBack(float duration)
    {
        //_rightFootTarget.DOKill();
        _rightFootTarget.DOLocalMove(_rightFootFirstPosition, duration);
    }

    public void LocalRotateRightHandTo(Quaternion rotation, float duration, bool relative)
    {
        //_rightFootTarget.DOKill();
        _rightHandTarget.DOLocalRotateQuaternion(rotation, duration).SetRelative(relative);
    }

    public void LocalRotateRightHandTo(Vector3 rotation, float duration, bool relative)
    {
        //_rightFootTarget.DOKill();
        _rightHandTarget.DOLocalRotate(rotation, duration).SetRelative(relative);
    }

    public void LocalRotateLeftHandTo(Vector3 rotation, float duration, bool relative)
    {
        //_rightFootTarget.DOKill();
        _leftHandTarget.DOLocalRotate(rotation, duration).SetRelative(relative);
    }

    public void LocalRotateLeftFootTo(Quaternion rotation, float duration, bool relative)
    {
        //_leftFootTarget.DOKill();
        _leftFootTarget.DOLocalRotateQuaternion(rotation, duration).SetRelative(relative);
    }

    public void LocalRotateRightFootTo(Quaternion rotation, float duration, bool relative)
    {
        //_rightFootTarget.DOKill();
        _rightFootTarget.DOLocalRotateQuaternion(rotation, duration).SetRelative(relative);
    }

    public void LocalRotateLeftFootBack(float duration)
    {
        //_leftFootTarget.DOKill();
        _leftFootTarget.DOLocalRotateQuaternion(_leftFootFirstRotation, duration);
    }

    public void LocalRotateRightFootBack(float duration)
    {
        //_rightFootTarget.DOKill();
        _rightFootTarget.DOLocalRotateQuaternion(_rightFootFirstRotation, duration);
    }

    public void SetRightHandRotationWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.rightHandEffector.rotationWeight, x => _fbik.solver.rightHandEffector.rotationWeight = x, weight, duration);
    }

    private Tweener _rightHandPositionTweenWeight = null;
    public void SetRightHandPositionWeight(float weight, float duration)
    {
        if(_rightHandPositionTweenWeight != null)
        {
            _rightHandPositionTweenWeight.Kill();
        }
        _rightHandPositionTweenWeight = DOTween.To(() => _fbik.solver.rightHandEffector.positionWeight, x => _fbik.solver.rightHandEffector.positionWeight = x, weight, duration);
    }

    public void SetLeftHandRotationWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.leftHandEffector.rotationWeight, x => _fbik.solver.leftHandEffector.rotationWeight = x, weight, duration);
    }

    private Tweener _leftHandPositionTweenWeight = null;
    public void SetLeftHandPositionWeight(float weight, float duration)
    {
        if (_leftHandPositionTweenWeight != null)
        {
            _leftHandPositionTweenWeight.Kill();
        }
        _leftHandPositionTweenWeight = DOTween.To(() => _fbik.solver.leftHandEffector.positionWeight, x => _fbik.solver.leftHandEffector.positionWeight = x, weight, duration);
    }

    public void SetRightFootRotationWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.rightFootEffector.rotationWeight, x => _fbik.solver.rightFootEffector.rotationWeight = x, weight, duration);
    }

    public void SetRightFootPositionWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.rightFootEffector.positionWeight, x => _fbik.solver.rightFootEffector.positionWeight = x, weight, duration);
    }

    public void SetLeftFootRotationWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.leftFootEffector.rotationWeight, x => _fbik.solver.leftFootEffector.rotationWeight = x, weight, duration);
    }

    public void SetLeftFootPositionWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.leftFootEffector.positionWeight, x => _fbik.solver.leftFootEffector.positionWeight = x, weight, duration);
    }

    public void SetSpinePositionWeight(float weight, float duration)
    {
        DOTween.To(() => _fbik.solver.bodyEffector.positionWeight, x => _fbik.solver.bodyEffector.positionWeight = x, weight, duration);
    }

    public Transform GetLeftFootBoneTransform()
    {
        return _fbik.solver.leftFootEffector.bone;
    }

    public Transform GetRightFootBoneTransform()
    {
        return _fbik.solver.rightFootEffector.bone;
    }

    public Transform GetRightHandTargetTransform()
    {
        return _rightHandTarget;
    }
}