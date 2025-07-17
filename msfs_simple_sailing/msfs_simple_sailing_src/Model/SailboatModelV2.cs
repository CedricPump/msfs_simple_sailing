using System;
using System.Diagnostics;

namespace msfs_simple_sailing.Model
{
    internal class SailboatModelV2 : SailboatModel
    {
        private const double MaxBoomDeflection = 85.0;
        private const double MaxJibDeflection = 45.0;

        public override double Update(double windSpeed, double relativeWindDir)
        {
            if (!isMainSailSet && !isJibSet)
                return 0.0;

            bool windFromPort = relativeWindDir > 180;

            // angle from wind direction
            double freeBoomDirection = windFromPort ? 360-relativeWindDir : -relativeWindDir;
            // limit by Max Deflection
            double limitedBoomDeflection = Math.Min(Math.Abs(freeBoomDirection), MaxBoomDeflection * MainSheetSlack / 100);
            BoomDeflectionDeg = freeBoomDirection > 0 ? limitedBoomDeflection : -limitedBoomDeflection;
            var BoomPointDeg = GetOppositeAngle(180 - BoomDeflectionDeg);

            // angle from wind direction
            double freeJibDirection = windFromPort ? 360 - relativeWindDir : -relativeWindDir;
            // limit by Max Deflection
            double activeJibSheetSlack = windFromPort ? StarJibSheetSlack : PortJibSheetSlack;
            double limitedJibDeflection = Math.Min(Math.Abs(freeJibDirection), MaxJibDeflection * activeJibSheetSlack / 100);
            JibDeflectionDeg = freeJibDirection > 0 ? limitedJibDeflection : -limitedJibDeflection;
            var JibPointDeg = GetOppositeAngle(180-JibDeflectionDeg);

            // Calculate angle to wind (angle of attack)
            double boomAoA = windFromPort ? BoomPointDeg - relativeWindDir : -(BoomPointDeg - relativeWindDir);
            double jibAoA = windFromPort ? JibPointDeg - relativeWindDir : -(JibPointDeg - relativeWindDir);

            // Efficiency Calculations
            double boomLift = ComputeLiftEfficiency(boomAoA);
            double boomDrag = ComputeDragEfficiency(Math.Abs(boomAoA));
            double jibLift = ComputeLiftEfficiency(jibAoA);
            double jibDrag = ComputeDragEfficiency(Math.Abs(jibAoA));

            this.MainDraftPerc = boomLift + boomDrag;
            this.JibDraftPerc = jibLift + jibDrag;

            // Weighted sail performance
            double mainPerformance = 0.8 * boomLift + 0.5 * boomDrag;
            double jibPerformance = 0.8 * jibLift + 0.5 * jibDrag;

            TotalPerformance = 0;
            if (isMainSailSet)
                TotalPerformance += mainPerformance * 0.75;
            if (isJibSet)
                TotalPerformance += jibPerformance * 0.30;

            double TargetSpeed = windSpeed * TotalPerformance;

            Console.WriteLine("Debug: ---");
            Console.WriteLine($"  Wind: {windSpeed,2:0}kt at {relativeWindDir:0.0}°");
            Console.WriteLine("  Boom");
            Console.WriteLine($"  BoomDeflectionDeg: {BoomDeflectionDeg,4:0}° AoA: {boomAoA,3:0}° Lift: {boomLift,4:0.0} Drag: {boomDrag,4:0.0} Perf: {mainPerformance * 100,3:0}%");
            Console.WriteLine("  Jib");
            Console.WriteLine($"  JibDeflectionDeg: {JibDeflectionDeg,4:0}° AoA: {jibAoA,3:0}° Lift: {jibLift,4:0.0} Drag: {jibDrag,4:0.0} Perf: {jibPerformance * 100,3:0}%");
            Console.WriteLine($"  TotalPerformance: {TotalPerformance * 100,3:0}%");
            Console.WriteLine($"  TargetSpeed: {TargetSpeed,3:0}kt");

            return TargetSpeed;
        }

        public override void SetMainSheet(double percent)
        {
            this.MainSheetSlack = Clamp01(percent);
        }

        public override void SetStarJibSheet(double percent)
        {
            percent = Clamp01(percent);
            if (percent > this.StarJibSheetSlack)
                this.PortJibSheetSlack = 100;
            this.StarJibSheetSlack = percent;
        }

        public override void SetPortJibSheet(double percent)
        {
            percent = Clamp01(percent);
            if (percent > this.PortJibSheetSlack)
                this.StarJibSheetSlack = 100;
            this.PortJibSheetSlack = percent;
        }

        public override void SetSail(bool set)
        {
            this.isMainSailSet = set;
            this.isJibSet = set;
        }

        private double Clamp01(double value)
        {
            return Math.Max(0.0, Math.Min(100.0, value));
        }

        private double Clamp(double deflected, double maxAbs)
        {
            return Math.Max(-maxAbs, Math.Min(maxAbs, deflected));
        }

        private double GetOppositeAngle(double angle)
        {
            return (angle + 180.0) % 360.0;
        }

        private double NormalizeWindDirection(double angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }

        private double ComputeAoA(double sailAngle, double windDir)
        {
            double aoa = sailAngle - windDir;

            while (aoa > 180.0) aoa -= 360.0;
            while (aoa < -180.0) aoa += 360.0;

            return aoa;
        }

        private double ComputeLiftEfficiency(double aoa)
        {
            if (Math.Abs(aoa) < 2.0 || Math.Abs(aoa) > 30.0)
                return 0.0;

            double absAoA = Math.Abs(aoa);
            double peak = 15.0;
            double maxEff = 1.0;

            if (absAoA <= peak)
            {
                return (absAoA - 2.0) / (peak - 2.0) * maxEff;
            }
            else
            {
                return (30.0 - absAoA) / (30.0 - peak) * maxEff;
            }
        }

        private double ComputeDragEfficiency(double angle)
        {
            double peak = 90.0;
            double falloff = 60.0;
            double diff = Math.Abs(angle - peak);
            return Math.Max(0.0, 1.0 - (diff / falloff));
        }
    }
}
