using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msfs_simple_sailing.Model
{
    internal class SailboatModel
    {
        bool isMainSailSet = false;
        bool isJibSet = false;
        public double boomDeflectionDeg = 0; // deg
        public double jibDeflictionDeg = 0; // deg
        double mainSheetSlack = 0; // %
        double jibSailDeflectiosn = 0; // deg
        double portJibSheetSlack = 0; // %
        double starJibSheetSlack = 0; // %
        double mainSailTensiton = 0; // %
        double boomAngle = 0; // Angle from centerline, positive = starboard, negative = port

        // auto topping lift and kicking strap, ignore boom raising


        public SailboatModel() { }


        public double update(double windSpeed, double relativeWindDir)
        {
            double calculatedSpeed = 0.0;
            if (isMainSailSet)
            {
                double relativeWindAngle = Math.Abs(relativeWindDir > 180 ? 360 - relativeWindDir : relativeWindDir);
                double ratio = 0;
                double deflection = 0;
                double jibDeflection = 0;

                if (relativeWindAngle < 10)
                {
                    ratio = 0.0;
                    deflection = 0;
                    jibDeflection = 0;
                    jibDeflection = 0;
                }
                else if (relativeWindAngle < 35)
                {
                    double t = (relativeWindAngle - 10) / (35 - 10);
                    ratio = Lerp(0.0, 0.6, t);
                    deflection = Lerp(0.0, 10.0, t);
                    jibDeflection = Lerp(0.0, 15.0, t); ;
                }
                else if (relativeWindAngle < 50)
                {
                    double t = (relativeWindAngle - 35) / (50 - 35);
                    ratio = Lerp(0.6, 0.85, t);
                    deflection = Lerp(10.0, 25.0, t);
                    jibDeflection = 15;
                }
                else if (relativeWindAngle < 75)
                {
                    double t = (relativeWindAngle - 50) / (75 - 50);
                    ratio = Lerp(0.85, 1.0, t);
                    deflection = Lerp(25.0, 45.0, t);
                    jibDeflection = 15;
                }
                else if (relativeWindAngle < 110)
                {
                    double t = (relativeWindAngle - 75) / (110 - 75);
                    ratio = Lerp(1.0, 0.85, t);
                    deflection = Lerp(45.0, 60.0, t);
                    jibDeflection = 30;
                }
                else if (relativeWindAngle < 135)
                {
                    double t = (relativeWindAngle - 110) / (135 - 110);
                    ratio = Lerp(0.85, 0.7, t);
                    deflection = Lerp(60.0, 70.0, t);
                    jibDeflection = 30;
                }
                else if (relativeWindAngle < 160)
                {
                    double t = (relativeWindAngle - 135) / (160 - 135);
                    ratio = Lerp(0.7, 0.55, t);
                    deflection = Lerp(70.0, 85.0, t);
                    jibDeflection = 30;
                }
                else
                {
                    ratio = 0.55;
                    deflection = 85.0;
                    jibDeflection = -45.0;
                }

                bool windFromPort = (relativeWindDir + 360) % 360 > 180;
                boomDeflectionDeg = windFromPort ? -deflection : deflection;
                jibDeflictionDeg = windFromPort ? -jibDeflection : jibDeflection;

                calculatedSpeed = windSpeed * ratio;
            }

            return calculatedSpeed;
        }

        private double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }




        public void SetBoomDeflection(double boomDeflectionDeg) 
        {
            this.boomDeflectionDeg=boomDeflectionDeg;
        }

        public void SetSail(bool set)
        {
            this.isMainSailSet = set;
            this.isJibSet = set;
        }
    }
}
