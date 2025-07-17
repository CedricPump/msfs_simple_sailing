using System;

namespace msfs_simple_sailing.Model
{
    internal class SailboatModelV1 : SailboatModel
    {
        public SailboatModelV1() { }


        public override double Update(double windSpeed, double relativeWindDir)
        {
            double calculatedSpeed = 0.0;
            if (this.isMainSailSet)
            {
                double relativeWindAngle = Math.Abs(relativeWindDir > 180 ? 360 - relativeWindDir : relativeWindDir);
                double ratio = 0;
                double deflection = 0;
                double jibDeflection = 0;

                if (relativeWindAngle < 10)
                {
                    // In irons — too close to the wind
                    ratio = 0.0;
                    deflection = 0;
                    jibDeflection = 0;
                }
                else if (relativeWindAngle < 35)
                {
                    // Close-hauled — sails trimmed in tight
                    double t = (relativeWindAngle - 10) / (35 - 10);
                    ratio = Lerp(0.0, 0.6, t);
                    deflection = Lerp(0.0, 10.0, t);
                    jibDeflection = 15; // LOCKED close to centerline
                }
                else if (relativeWindAngle < 50)
                {
                    double t = (relativeWindAngle - 35) / (50 - 35);
                    ratio = Lerp(0.6, 0.85, t);
                    deflection = Lerp(10.0, 25.0, t);
                    jibDeflection = 20;
                }
                else if (relativeWindAngle < 75)
                {
                    double t = (relativeWindAngle - 50) / (75 - 50);
                    ratio = Lerp(0.85, 1.0, t);
                    deflection = Lerp(25.0, 45.0, t);
                    jibDeflection = 25;
                }
                else if (relativeWindAngle < 110)
                {
                    double t = (relativeWindAngle - 75) / (110 - 75);
                    ratio = Lerp(1.0, 0.85, t);
                    deflection = Lerp(45.0, 60.0, t);
                    jibDeflection = Lerp(25.0, 30.0, t);
                }
                else if (relativeWindAngle < 135)
                {
                    double t = (relativeWindAngle - 110) / (135 - 110);
                    ratio = Lerp(0.85, 0.7, t);
                    deflection = Lerp(60.0, 70.0, t);
                    jibDeflection = 30; // Fully eased
                }
                else if (relativeWindAngle < 160)
                {
                    double t = (relativeWindAngle - 135) / (160 - 135);
                    ratio = Lerp(0.7, 0.4, t); // Less effective downwind
                    deflection = Lerp(70.0, 85.0, t);
                    jibDeflection = Lerp(30.0, 45.0, t); // Winged out
                }
                else if (relativeWindAngle < 165)
                {
                    double t = (relativeWindAngle - 160) / (165 - 160);
                    ratio = 0.4;
                    deflection = 85.0;
                    jibDeflection = Lerp(45.0, -45.0, t); // Winged out
                }
                else
                {
                    // Deep downwind
                    ratio = 0.4;
                    deflection = 85.0;
                    jibDeflection = -45.0; // Fully winged out
                }

                bool windFromPort = (relativeWindDir + 360) % 360 > 180;
                BoomDeflectionDeg = windFromPort ? -deflection : deflection;
                JibDeflectionDeg = windFromPort ? -jibDeflection : jibDeflection;

                TotalPerformance = ratio;

                calculatedSpeed = windSpeed * ratio;
            }

            return calculatedSpeed;
        }

        public override void SetSail(bool set)
        {
            this.isMainSailSet = set;
            this.isJibSet = set;
        }

        public override void SetMainSheet(double percent)
        {
            // ignore
        }

        public override void SetStarJibSheet(double percent)
        {
            // ignore
        }

        public override void SetPortJibSheet(double percent)
        {
            // ignore
        }
    }
}
