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
        FormUI? form;
        SailboatModel model;
        public bool isSailUp = false;

        public Controller()
        {
            this.config = Config.Load();
            plane = new Plane(OnPlaneEventCallback);
            this.model = new SailboatModel();
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
                        double windSpeed = plane.getWindTotal();
                        double windDir = plane.getRelDir();
                        double rudder = plane.AileronDefelctionPct * 100;
                        double trim = plane.AileronTrimPct * 100;
                        double groundspeed = plane.vZ;
                        double airspeedTrueRaw = plane.airspeedTrueRaw;

                        form.SetSpeed(groundspeed);
                        form.SetWind(windSpeed, windDir);

                        double speed = model.update(windSpeed, windDir);
                        form.SetBoomDeflection(model.boomDeflectionDeg, model.jibDeflectionDeg);

                        // pause acceleration while steering
                        // only accelerate when > Groudnspeed
                        // max speed 30 knots for water stability
                        double appliedSpeed = 0;
                        if (speed > groundspeed && speed <= 30 && Math.Abs(rudder) < 5)
                        {
                            // limit acceleration 
                            appliedSpeed = Math.Max(groundspeed + 0.03, speed);
                            // plane.setValue("AIRSPEED TRUE RAW", appliedSpeed);
                            plane.setValue("VELOCITY BODY Z", appliedSpeed);

                        }

                        form.setLog($"" +
                            $"sail set: {isSailUp} \r\n" +
                            $"wind: {windSpeed,4:0.0} knots {windDir,4:0.0}° \r\n" +
                            $"[x: {plane.vX,4:0.0}, y: {plane.vY,4:0.0}, z: {plane.vZ,4:0.0}]\r\n" +
                            $"groundspeed: {groundspeed,4:0.0} knots \r\n" +
                            $"airspeedTrueRaw: {airspeedTrueRaw,4:0.0} knots \r\n" +
                            $"appliedSpeed: {appliedSpeed,4:0.0} knots \r\n" +
                            $"trim: {trim,4:0.0}% \r\n" +
                            $"rudder: {rudder,4:0.0}% \r\n");
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
        }

        public void toggleSail() 
        {
            this.isSailUp = !this.isSailUp;
            this.model.SetSail(this.isSailUp);
        }
    }
}
