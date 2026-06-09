using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    #region Unity API

    private void Update()
    {
        DefectEffect();

        if (_isFlickering)
        {
            FlickerIntensity();
        }
       
    }

    #endregion


    #region Main API
    private void DefectEffect()
    {
        _currentIntervalTimer += Time.deltaTime;
        if (_currentIntervalTimer < _flickerInterval) return;
        _currentIntervalTimer = 0;
        _isFlickering = true;
    }

    private void FlickerIntensity()
    {
        _currentTime += Time.deltaTime;
       
        float intensity = _flickerCurve.Evaluate(_currentTime/_animationLength);
        _torch.intensity = intensity;
        if (_currentTime >= _animationLength)
        {
            _isFlickering = false;
            _currentTime = 0;
        }
    }
    public void SetFlickerInterval(float interval)
    {
        _flickerInterval = interval;
    }

    #endregion


    #region Private and Protected

    [SerializeField] private Light2D _torch;
    private bool _isFlickering;
    [SerializeField] private AnimationCurve _flickerCurve;

    private float _currentTime;
    private float _animationLength = 1f;
    [SerializeField] private float _flickerInterval;
    private float _currentIntervalTimer;
    #endregion
}