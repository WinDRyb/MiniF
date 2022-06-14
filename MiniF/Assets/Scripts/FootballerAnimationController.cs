using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootballerAnimationController : MonoBehaviour {

    private Rigidbody _rigidbody;
    private Animator _animator;
    
    private void Awake() {
        _rigidbody = transform.parent.gameObject.GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (_rigidbody.velocity.sqrMagnitude > 0.01f) {
            _animator.SetBool("IsMoving", true);
        } else {
            _animator.SetBool("IsMoving", false);
        }
    }

    public void PlaySlideTackleAnimation() {
        _animator.Play("Base Layer.Footballer_SlideTackle");
    }

    public void PlayFallAnimation() {
        _animator.Play("Base Layer.Footballer_Fall");
    }
}
