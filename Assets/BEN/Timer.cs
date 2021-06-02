using UnityEngine;

public struct Timer
{
    private float _currentValue;
    private float _targetValue;

    /// <summary>
    /// Initialise a value with 0f and targetValue as value to reach
    /// </summary>
    /// <param name="targetValue">value to reach</param>
    /// <param name="sendTrueOnFirstFrame">on the first frame from Update()</param>
    public void SetTargetValue(float targetValue)
    {
        _currentValue = 0f; 
        _targetValue = targetValue;
    }

    /// <summary>
    /// Updates initial value by 0.02f until it reaches target value from Set()
    /// </summary>
    /// <returns>return true when target value is reached, along with the final value</returns>
    public (bool targetIsreached, bool isFirstFrame) Update()
    { 
        _currentValue += 0.02f;
        if (_currentValue < 0.03f) 
        {
            return (false, true); 
        } 
        
        return _currentValue >= _targetValue ? (true, false) : ( false, false); 
    }

    public void Reset()
    {
        _currentValue = 0f; 
    }
}
