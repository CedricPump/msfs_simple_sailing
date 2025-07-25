using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace msfs_simple_sailing.Model
{
    public abstract class SailboatModel
    {
        protected bool isMainSailSet = false;
        protected bool isJibSet = false;
        public double BoomDeflectionDeg { get; protected set; } = 0; // deg
        public double JibDeflectionDeg { get; protected set; } = 0; // deg
        public double MainDraftPerc { get; protected set; } = 0; // %
        public double JibDraftPerc { get; protected set; } = 0; // %
        public double MainSheetUsage { get; protected set; } = 0; // %
        protected double jibSailDeflectiosn = 0; // deg
        public double PortJibSheetUsage { get; protected set; } = 0; // %
        public double StarJibSheetUsage { get; protected set; } = 0; // %
        protected double mainSailTensiton = 0; // %
        public bool mainSheetUnderTension = false;
        public bool starJibSheetUnderTension = false;
        public bool portJibSheetUnderTension = false;
        protected double boomAngle = 0; // Angle from centerline, positive = starboard, negative = port

        public double TotalPerformance {  get; protected set; } = 0;

        // auto topping lift and kicking strap, ignore boom raising


        public SailboatModel() { }


        public abstract double Update(double windSpeed, double relativeWindDir);

        protected double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        public abstract void SetSail(bool set);

        public abstract void SetMainSheet(double percent);

        public abstract void SetStarJibSheet(double percent);

        public abstract void SetPortJibSheet(double percent);
    }
}
