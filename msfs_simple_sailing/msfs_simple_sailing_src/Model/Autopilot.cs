using System;
using System.Diagnostics;

internal sealed class Autopilot
{
    public double Kp { get; set; } = 0.01;
    public double Ki { get; set; } = 0.001;
    public double Kd { get; set; } = 0.1;

    public double OutputMin { get; set; } = -1.0;
    public double OutputMax { get; set; } = 1.0;
    public double IntegralLimit { get; set; } = 5.0;
    public double HeadingDeadbandDeg { get; set; } = 2.0;

    public double ErrorLpfTau { get; set; } = 1.0;     // Smooth error to reject waves
    public double DerivativeLpfTau { get; set; } = 0.5; // Smooth derivative to avoid noise

    public double MaxDeltaTime { get; set; } = 0.25;
    public double DefaultDeltaTime { get; set; } = 0.016;
    public double RudderRateLimitPerSec { get; set; } = 16000;

    private double _integral;
    private double _prevFilteredError;
    private double _filteredError;
    private double _filteredDerivative;
    private double _lastOutput;
    private double _lastTimeSeconds;
    private bool _first = true;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private double _targetHeading;

    public Autopilot(double kp = 0.03, double ki = 0.01, double kd = 0.01)
    {
        Kp = kp; Ki = ki; Kd = kd;
        _lastTimeSeconds = _stopwatch.Elapsed.TotalSeconds;
    }

    public void SetTargetHeading(double headingDeg)
    {
        _targetHeading = Normalize360(headingDeg);
    }

    public double CalculateRequiredRudder(double currentHeadingDeg)
    {
        double now = _stopwatch.Elapsed.TotalSeconds;
        double dt = now - _lastTimeSeconds;
        _lastTimeSeconds = now;

        if (_first || dt <= 0)
        {
            dt = DefaultDeltaTime;
            _first = false;
            _filteredError = 0;
            _filteredDerivative = 0;
            _prevFilteredError = 0;
        }
        if (dt > MaxDeltaTime)
            dt = MaxDeltaTime;

        double currentHeading = Normalize360(currentHeadingDeg);
        double error = Wrap180(_targetHeading - currentHeading);
        // Console.WriteLine($"error: {error}°");

        // Deadband
        if (Math.Abs(error) < HeadingDeadbandDeg)
            error = 0;

        // Low-pass filter error
        double alphaError = dt / (ErrorLpfTau + dt);
        _filteredError += alphaError * (error - _filteredError);

        // Low-pass derivative
        double rawDerivative = (_filteredError - _prevFilteredError) / dt;
        double alphaDer = dt / (DerivativeLpfTau + dt);
        _filteredDerivative += alphaDer * (rawDerivative - _filteredDerivative);
        _prevFilteredError = _filteredError;

        // Integrate only when outside deadband
        if (error != 0)
        {
            _integral += _filteredError * dt;
            _integral = Clamp(_integral, -IntegralLimit, IntegralLimit);
        }

        // PID calculation
        double output =
            (Kp * _filteredError) +
            (Ki * _integral) +
            (Kd * _filteredDerivative);

        // Clamp and rate-limit
        output = Clamp(output, OutputMin, OutputMax);
        double maxStep = RudderRateLimitPerSec * dt;
        output = Clamp(output, _lastOutput - maxStep, _lastOutput + maxStep);

        _lastOutput = output;
        // Console.WriteLine($"rudder: {output}%");
        return output;
    }

    public void Reset()
    {
        _integral = 0;
        _prevFilteredError = 0;
        _filteredError = 0;
        _filteredDerivative = 0;
        _lastOutput = 0;
        _first = true;
        _lastTimeSeconds = _stopwatch.Elapsed.TotalSeconds;
    }

    private static double Normalize360(double angle)
    {
        angle %= 360.0;
        return angle < 0 ? angle + 360.0 : angle;
    }

    private static double Wrap180(double angle)
    {
        angle = Normalize360(angle);
        return (angle > 180.0) ? angle - 360.0 : angle;
    }

    private static double Clamp(double v, double min, double max)
    {
        return v < min ? min : (v > max ? max : v);
    }
}
