using System;
using msfs_simple_sail_core.Core;

namespace msfs_simple_sailing.Model
{
    internal class SailboatModelV2 : SailboatModel
    {
        private const double MaxBoomDeflection = 85.0;
        private const double MaxJibDeflection = 180.0;

        public override double Update(double windSpeed, double relativeWindDir)
        {
            if (!isMainSailSet && !isJibSet)
                return 0.0;

            bool windFromPort = relativeWindDir > 180;

            // angle from wind direction
            double freeBoomDirection = windFromPort ? 360-relativeWindDir : -relativeWindDir;
            // limit by Max Deflection
            double limitedBoomDeflection = Math.Min(Math.Abs(freeBoomDirection), MaxBoomDeflection * MainSheetUsage / 100);
            double sheetedBoomdeflection = MaxBoomDeflection * MainSheetUsage / 100;
            mainSheetUnderTension = Math.Abs(freeBoomDirection) > sheetedBoomdeflection;
            BoomDeflectionDeg = freeBoomDirection >= 0 ? limitedBoomDeflection : -limitedBoomDeflection;
            var BoomPointDeg = GetOppositeAngle(180 - BoomDeflectionDeg);

            // angle from wind direction
            double freeJibDirection = windFromPort ? 360 - relativeWindDir : -relativeWindDir;
            // limit by Max Deflection
            double activeJibSheetSlack = windFromPort ? StarJibSheetUsage : PortJibSheetUsage;
            double limitedJibDeflection = Math.Min(Math.Abs(freeJibDirection), MaxJibDeflection * activeJibSheetSlack / 100);
            if (windFromPort)
            {
                starJibSheetUnderTension = Math.Abs(freeJibDirection) > limitedJibDeflection;
                portJibSheetUnderTension = false;
            }
            else
            {
                portJibSheetUnderTension = Math.Abs(freeJibDirection) > limitedJibDeflection;
                starJibSheetUnderTension = false;
            }

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
                TotalPerformance += mainPerformance * 0.80;
            if (isJibSet)
                TotalPerformance += jibPerformance * 0.30;

            double TargetSpeed = windSpeed * TotalPerformance;

            if (Config.GetInstance().debug)
            {
                Console.WriteLine("Debug: ---");
                Console.WriteLine($"  Wind: {windSpeed,2:0}kt at {relativeWindDir:0.0}°");
                Console.WriteLine("  Boom");
                Console.WriteLine($"  BoomDeflectionDeg: {BoomDeflectionDeg,4:0}° AoA: {boomAoA,3:0}° Lift: {boomLift,4:0.0} Drag: {boomDrag,4:0.0} Perf: {mainPerformance * 100,3:0}%");
                Console.WriteLine("  Jib");
                Console.WriteLine($"  JibDeflectionDeg: {JibDeflectionDeg,4:0}° AoA: {jibAoA,3:0}° Lift: {jibLift,4:0.0} Drag: {jibDrag,4:0.0} Perf: {jibPerformance * 100,3:0}%");
                Console.WriteLine($"  TotalPerformance: {TotalPerformance * 100,3:0}%");
                Console.WriteLine($"  TargetSpeed: {TargetSpeed,3:0}kt");
            }
            

            return TargetSpeed;
        }

        public override void SetMainSheet(double percent)
        {
            this.MainSheetUsage = Clamp01(percent);
        }

        public override void SetStarJibSheet(double percent)
        {
            percent = Clamp01(percent);
            if (percent > this.StarJibSheetUsage)
                this.PortJibSheetUsage = 100;
            this.StarJibSheetUsage = percent;
        }

        public override void SetPortJibSheet(double percent)
        {
            percent = Clamp01(percent);
            if (percent > this.PortJibSheetUsage)
                this.StarJibSheetUsage = 100;
            this.PortJibSheetUsage = percent;
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

        private double GetOppositeAngle(double angle)
        {
            return (angle + 180.0) % 360.0;
        }


        private double ComputeLiftEfficiency(double aoa)
        {
            const double aoaUpperLimit = 45.0;
            const double aoaLowerLimit = 2.0;
            const double peakAoA = 15.0;

            double absAoA = Math.Abs(aoa);

            if (absAoA < aoaLowerLimit || absAoA > aoaUpperLimit)
                return 0.0;

            // Normalize AoA to a 0–1 scale centered at peakAoA
            double normalized = (absAoA - aoaLowerLimit) / (aoaUpperLimit - aoaLowerLimit);
            double peakNorm = (peakAoA - aoaLowerLimit) / (aoaUpperLimit - aoaLowerLimit);

            // Compute a cosine bell curve, peaking at peakNorm
            double x = (normalized - peakNorm) / peakNorm; // from -1 to +1
            double lift = Math.Cos(x * Math.PI / 2);       // cosine bell
            return Math.Pow(lift, 2); // smooth falloff
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
