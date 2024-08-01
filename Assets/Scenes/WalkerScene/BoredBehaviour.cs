using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoredBehaviour : StateMachineBehaviour
{
    bool _isBored = false;
    float _idleTime;
    public float TimeUntilBored = 3.0f;
    public int BoredAnimationsCount = 2;
    int _boredAnimation = 0;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!_isBored)
        {
            _idleTime += Time.deltaTime;
            if (_idleTime > TimeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                _isBored = true;
                _boredAnimation = Random.Range(1, BoredAnimationsCount + 1);
                _boredAnimation = _boredAnimation * 2 - 1;
                animator.SetFloat("BoredAnimation", _boredAnimation - 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f) // end of looped animation
        {
            ResetIdle();
        }
        animator.SetFloat("BoredAnimation", _boredAnimation, 0.2f, Time.deltaTime);
    }
    private void ResetIdle()
    {
        if (_isBored)
        {
            _boredAnimation--;
            _isBored = false;
        }
        _idleTime = 0.0f;
    }
}