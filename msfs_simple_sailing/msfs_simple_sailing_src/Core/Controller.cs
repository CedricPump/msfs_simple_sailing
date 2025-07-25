using System;
using System.ComponentModel.Design.Serialization;
using msfs_simple_sail;
using msfs_simple_sail_core.UI;
using msfs_simple_sailing.Model;


namespace msfs_simple_sail_core.Core
{
    public class Controller
    {
        Plane plane;
        Config config;
        private readonly Autopilot autopilot;
        FormUI? form;
        public SailboatModel model;
        public bool isSailUp = false;
        public bool headingHold = false;
        public double autopilotSelectedHeading = 0.0;
        internal double rudder;

        public Controller()
        {
            this.config = Config.Load();
            this.autopilot = new Autopilot();
            plane = new Plane(OnPlaneEventCallback);
            this.model = new SailboatModelV2();
        }

        private void OnPlaneEventCallback(PlaneEvent planeEvent)
        {
            // throw new NotImplementedException();
        }

        public void SetUI(FormUI form) 
        {
            this.form = form;
        }

        public void Init()
        {
            // Update once to trigger connect to sim
            plane.Update();

            return;
        }

        public void Run()
        {
            while (true)
            {
                if (form == null)
                {
                    System.Threading.Thread.Sleep(330);
                    continue;
                }
                try
                {
                    if(!plane.IsSimConnectConnected) form.setLog("conectiong to sim ...");


                    plane.Update();
                    if(!plane.isInit) { 
                        plane.Update();
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };

                    if (plane.Title == null)
                    {
                        plane.Update();
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };

                    if (plane.isInit) 
                    {
                        if(plane.IsEngineOn) 
                        {
                            this.isSailUp = false;
                        }

                        double windSpeed = plane.getWindTotal();
                        double windDir = plane.getRelDir();
                        this.rudder = plane.AileronDefelctionPct * 100;
                        double trim = plane.AileronTrimPct * 100;
                        double groundspeed = plane.vZ;
                        double airspeedTrueRaw = plane.airspeedTrueRaw;

                        form.SetSpeed(groundspeed);
                        form.SetWind(windSpeed, windDir);

                        UpdateTrim();

                        double speed = model.Update(windSpeed, windDir);
                        form.SetBoomDeflection(model.BoomDeflectionDeg, model.JibDeflectionDeg, model.MainDraftPerc, model.JibDraftPerc);

                        // pause acceleration while steering
                        // only accelerate when > Groudnspeed
                        // max speed 30 knots for water stability
                        double appliedSpeed = 0;
                        if (speed > groundspeed && speed <= 30)
                        {
                            if (!config.pauseOnSteer || Math.Abs(this.rudder) < 5)
                            {
                                // limit acceleration 
                                appliedSpeed = Math.Max(groundspeed + 0.03, speed);
                                // plane.setValue("AIRSPEED TRUE RAW", appliedSpeed);
                                plane.setValue("VELOCITY BODY Z", appliedSpeed);
                            }
                        }

                        form.setLog($"Debug:" +
                            $"  sail set: {isSailUp} \r\n" +
                            $"  engine on: {plane.IsEngineOn} \r\n" +
                            $"  wind: {windSpeed,4:0.0} knots {windDir,4:0.0}° \r\n" +
                            //$"  [x: {plane.vX,4:0.0}, y: {plane.vY,4:0.0}, z: {plane.vZ,4:0.0}]\r\n" +
                            $"  groundspeed: {groundspeed,4:0.0} knots \r\n" +
                            $"  airspeedTrueRaw: {airspeedTrueRaw,4:0.0} knots \r\n" +
                            $"  appliedSpeed: {appliedSpeed,4:0.0} knots \r\n" +
                            $"  trim: E {plane.ElevatorTrimPct*100,4:0}% A {plane.AileronTrimPct * 100,4:0}% R {plane.RudderTrimPct * 100,4:0}%\r\n" +
                            $"  sheets: main{model.MainSheetUsage,4:0}% port jib {model.PortJibSheetUsage,4:0}% starboard jib{model.StarJibSheetUsage,4:0}% \r\n" +
                            $"  performance: {model.TotalPerformance*100,4:0}%\r\n" +
                            $"  rudder: {this.rudder,4:0.0}% \r\n");

                        if (headingHold)
                        {
                            double rudderInput = autopilot.CalculateRequiredRudder(plane.Heading);
                            rudderInput = Math.Clamp(rudderInput, -1.0, 1.0);  // ensure rudderInput stays in range

                            double aileronCommand = rudderInput * 1;
                            aileronCommand = Math.Clamp(aileronCommand, -16000, 16000);

                            // Console.WriteLine($"Rudder input: {rudderInput * 100:F1}%, Aileron command: {aileronCommand:F0}");

                            plane.setValue("AILERON POSITION", aileronCommand);
                        }
                    }

                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // sleep until next interval
                int intervall = plane.IsSimConnectConnected ? config.RefreshInterval : config.IdleRefreshInterval;
                System.Threading.Thread.Sleep(intervall);
            };
        }

        public void setSail(bool isSailUp) 
        { 
            this.isSailUp = isSailUp;
            this.model.SetSail(this.isSailUp);
            if (this.isSailUp && plane.IsEngineOn) 
            {
                plane.setValue("GENERAL ENG COMBUSTION:1", 0);
            }
        }

        public void toggleSail() 
        {
            this.isSailUp = !this.isSailUp;
            this.model.SetSail(this.isSailUp);
        }

        private void UpdateTrim() 
        {
            var ElevatorTrim = plane.ElevatorTrimPct * 100;
            var AileronTrim = plane.AileronTrimPct * 100;
            model.SetMainSheet(-ElevatorTrim);
            if (AileronTrim > 0) {
                // right side
                model.SetStarJibSheet(AileronTrim);
            } else {
                // left side
                model.SetPortJibSheet(-AileronTrim);
            }
            // Todo: losen Jibsheets if trim == 0?
            this.form.SetTrims((int) Math.Floor(model.PortJibSheetUsage), (int)Math.Floor(model.MainSheetUsage), (int)Math.Floor(model.StarJibSheetUsage));
        }

        public void SetJibSheetPort(int jibPort) 
        {
            //plane.setValue("AILERON TRIM PCT", -jibPort/100);
        }

        public void SetJibSheetStar(int jibStar) 
        {
            //plane.setValue("AILERON TRIM PCT", jibStar/100);
        }

        public void SetMainSheet(int main) 
        {
            //plane.setValue("ELEVATOR TRIM PCT", main/100);
        }

        public void setAutopilot(bool enable)
        {
            if (enable && !headingHold)
            {
                autopilot.SetTargetHeading(plane.Heading);
            }
            headingHold = enable;
        }

    }
}
