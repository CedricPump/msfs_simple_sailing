using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using msfs_simple_sail.Model;
using msfs_simple_sail_core.Core;
using Microsoft.FlightSimulator.SimConnect;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace msfs_simple_sail
{
    public abstract class Aircraft
    {
        public bool isInit = false;
        private SimConnect simconnect;
        private IntPtr m_hWnd = new IntPtr(0);
        public const int WM_USER_SIMCONNECT = 0x0402;
        private Dictionary<DATA_DEFINE_ID, DataDefinition> definitions = new Dictionary<DATA_DEFINE_ID, DataDefinition>();
        private Dictionary<string, DataDefinition> definitions_by_string = new Dictionary<string, DataDefinition>();

        // Ident
        public string Model { get; private set; }
        public string Type { get; private set; }
        public string Title { get; private set; }
        // Position
        public double Altitude { get; private set; }
        public double Height_AGL { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Heading { get; private set; }
        // Movement
        public double GroundSpeed { get; private set; }
        public double VerticalSpeed { get; private set; }
        public double vX { get; private set; }
        public double vY { get; private set; }
        public double vZ { get; private set; }
        public double gforce { get; private set; }
        public double airspeedTrueRaw { get; private set; }
        // Orientation
        public double pitch { get; private set; }
        public double bank { get; private set; }
        // State
        public bool IsOnRundway { get; private set; }
        public bool IsOnGround { get; private set; }
        public bool IsEngineOn { get; private set; }
        public bool IsParkingBreak { get; private set; }
        public double Fuel { get; private set; }
        public double FuelPercent { get; private set; }
        public bool FuelUnlimited { get; private set; }

        // Env
        public double AltitudeAGL { get; private set; }

        // Sim
        public bool IsSimConnectConnected { get; private set; } = false;
        public bool SimDisabled { get; private set; } = true;
        // AntiCheat
        public bool CrashEnabled { get; private set; } = true;
        public bool IsSlew { get; private set; } = true;
        public int TimeAcceleration { get; private set; } = 1;
        // Weight
        public double TotalWeight { get; private set; }
        public double MaxTotalWeight { get; private set; }

        // Ambient
        public double WindX { get; private set; } = 0.0;
        public double WindY { get; private set; } = 0.0;

        // controls
        public double RudderTrimPct { get; private set; } = 0.0;
        public double ElevatorTrimPct { get; private set; } = 0.0;
        public double AileronTrimPct { get; private set; } = 0.0;
        public double RudderDefelctionPct { get; private set; } = 0.0;
        public double AileronDefelctionPct { get; private set; } = 0.0;

        private Config conf;





        public string toString()
        {
            return Model + " [" + Latitude + "," + Longitude + "," + Altitude + "] " + GroundSpeed + "knts " + Heading + "° ";
        }

        const uint SIMCONNECT_OBJECT_ID_USER = 0;

        enum GROUP_ID
        {
            GROUP_A,
        };

        public enum EVENTS
        {
            SimStart,
            SimStop,
            Crashed,
            AircraftLoaded,
            FlightLoaded,
            LANDING_LIGHTS_TOGGLE,
            PARKING_BRAKES,
            PARKING_BRAKE_SET,
            SMOKE_OFF,
            SMOKE_ON,
            SMOKE_SET,
            SMOKE_TOGGLE,
            TOW_PLANE_RELEASE,
            HORN_TRIGGER,
            PAUSE_TOGGLE,
            PAUSE_ON,
            PAUSE_OFF,
            SIM_RATE,
            SIM_RATE_DECR,
            SIM_RATE_INCR,
            SIM_RATE_SET
        };

        public static EVENTS[] SystemEvents = new EVENTS[] {
            EVENTS.SimStart,
            EVENTS.SimStop,
            EVENTS.Crashed,
            EVENTS.AircraftLoaded,
            EVENTS.FlightLoaded
        };


        public Aircraft()
        {
            IsSimConnectConnected = false;
            this.conf = Config.GetInstance();
            ConnectSimConnect();
            // simconnect.Text(SIMCONNECT_TEXT_TYPE.SCROLL_GREEN, 5.0f, null, "eSTOL_Training_Tool connected");
        }

        public Telemetrie GetTelemetrie()
        {
            return new Telemetrie
            {
                Position = new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude * 0.3048),
                Altitude = this.Altitude,
                AltitudeAGL = this.AltitudeAGL,
                Height = 0.0,
                GroundSpeed = this.GroundSpeed,
                Heading = this.Heading,
                vX = this.vX,
                vY = this.vY,
                vZ = this.vZ,
                pitch = this.pitch,
                bank = this.bank,
                verticalSpeed = this.VerticalSpeed,
                gForce = this.gforce,
                mainWheelRPM = 0,
                centerWheelRPM = 0
            };
        }

        public double getWindTotal()
        {
            double windTotal = Math.Sqrt(WindX * WindX + WindY * WindY);
            return windTotal;
        }

        public double getRelDir()
        {
            double angleRad = Math.Atan2(-WindX, -WindY); // flip windX to get tailwind at 0°
            double angleDeg = angleRad * (180.0 / Math.PI);

            // Normalize to [0, 360)
            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        public AircraftState GetState()
        {
            return new AircraftState
            {
                EngineOn = this.IsEngineOn,
                Fuel = this.Fuel,
                FuelPercent = this.FuelPercent,
                ParkingBrake = this.IsParkingBreak,
                Weight = this.TotalWeight,
                MaxWeightPercent = this.TotalWeight / this.MaxTotalWeight * 100,
                PilotWeight = 0,
                FuelUnlimited = this.FuelUnlimited,
            };
        }

        private void InitSimConnect(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            definitions = new Dictionary<DATA_DEFINE_ID, DataDefinition>();
            definitions_by_string = new Dictionary<string, DataDefinition>();

            Console.Write("simconnect init ");
            // Identity
            CreateDataDefinition("ATC MODEL", "", true);
            CreateDataDefinition("ATC TYPE", "", true);
            CreateDataDefinition("TITLE", "", true);
            // Position
            CreateDataDefinition("PLANE ALTITUDE", "feet");
            CreateDataDefinition("PLANE LATITUDE", "degrees");
            CreateDataDefinition("PLANE LONGITUDE", "degrees");
            // Orientation
            CreateDataDefinition("PLANE BANK DEGREES", "degrees");
            CreateDataDefinition("PLANE PITCH DEGREES", "degrees");
            CreateDataDefinition("PLANE HEADING DEGREES TRUE", "degrees");
            // CreateDataDefinition("PLANE HEADING DEGREES MAGNETIC", "degrees");
            // Speed
            CreateDataDefinition("GROUND VELOCITY", "knots");
            // CreateDataDefinition("AIRSPEED INDICATED", "knots");
            // CreateDataDefinition("AIRSPEED TRUE", "knots");

            CreateDataDefinition("VERTICAL SPEED", "feet per minute"); 
            CreateDataDefinition("G FORCE", "Gforce");
            CreateDataDefinition("VELOCITY BODY X", "knots");
            CreateDataDefinition("VELOCITY BODY Y", "knots");
            CreateDataDefinition("VELOCITY BODY Z", "knots");
            // Anti-Cheat
            CreateDataDefinition("REALISM CRASH DETECTION", "");
            CreateDataDefinition("IS SLEW ACTIVE", "");
            // State
            CreateDataDefinition("ENG COMBUSTION", "Bool");
            CreateDataDefinition("BRAKE PARKING POSITION", "Bool");
            CreateDataDefinition("GEAR IS ON GROUND", "Bool");
            CreateDataDefinition("SIM ON GROUND", "Bool");
            CreateDataDefinition("ON ANY RUNWAY", "Bool");
            //CreateDataDefinition("NAV LOC AIRPORT IDENT", "", true);
            // Fuel
            CreateDataDefinition("FUEL TOTAL QUANTITY WEIGHT", "pounds");
            CreateDataDefinition("FUEL SELECTED QUANTITY PERCENT", "percent over 100");

            // Action
            CreateDataDefinition("SMOKE ENABLE", "Bool");
            CreateDataDefinition("SIM DISABLED", "Bool");

            CreateDataDefinition("TOTAL WEIGHT", "lbs");

            // Ambient
            CreateDataDefinition("AIRCRAFT WIND X", "knots");
            CreateDataDefinition("AIRCRAFT WIND Z", "knots");
            CreateDataDefinition("AIRSPEED TRUE RAW", "knots");

            CreateDataDefinition("AILERON TRIM PCT", "percent over 100");
            CreateDataDefinition("RUDDER TRIM PCT", "percent over 100");
            CreateDataDefinition("ELEVATOR TRIM PCT", "percent over 100");
            CreateDataDefinition("RUDDER DEFLECTION PCT", "percent over 100");
            CreateDataDefinition("AILERON RIGHT DEFLECTION PCT", "percent over 100");
            CreateDataDefinition("GENERAL ENG COMBUSTION:1", "Bool");
            

            RegiserDefinitions();

            this.isInit = true;
            Console.WriteLine("done");
        }

        private void RegiserDefinitions()
        {
            foreach (DataDefinition def in definitions.Values)
            {
                RegisterDataDefinition(def);
            }
        }

        private DataDefinition CreateDataDefinition(string name, string unit = "", bool isString = false)
        {
            DataDefinition def = new DataDefinition(name, unit, isString);
            this.definitions.Add(def.defId, def);
            this.definitions_by_string.Add(name, def);
            return def;
        }

        private void RegisterDataDefinition(DataDefinition def)
        {
            if (def.isString)
            {
                simconnect.AddToDataDefinition(def.defId, def.dname, "", SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.RegisterDataDefineStruct<DataStruct>(def.defId);
                simconnect.RequestDataOnSimObjectType(def.reqId, def.defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            else
            {
                simconnect.AddToDataDefinition(def.defId, def.dname, def.dunit, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.RegisterDataDefineStruct<double>(def.defId);
                simconnect.RequestDataOnSimObjectType(def.reqId, def.defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
        }

        private DataDefinition getDefinitionByName(string name) 
        {
            DataDefinition def = definitions_by_string[name];
            return def;
        }

        private SimConnect ConnectSimConnect()
        {
            try
            {
                // The constructor is similar to SimConnect_Open in the native API
                Console.Write("conneting to sim ");
                simconnect = new SimConnect("Simconnect - Simvar test", m_hWnd, WM_USER_SIMCONNECT, null, 0);
                simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(InitSimConnect);
                simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnRecvQuit);
                simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnRecvException);
                simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecvSimobjectDataBytype);
                simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(OnRecvSimobjectDataBytype);
                simconnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(simconnect_OnRecvEvent);

                foreach (EVENTS entry in Enum.GetValues(typeof(EVENTS)))
                {
                    
                    if(Array.IndexOf(SystemEvents, entry) >= 0)
                    {
                        //Console.WriteLine($"sys: {entry}");
                        simconnect.SubscribeToSystemEvent(entry, entry.ToString());
                    }
                    else 
                    {
                        //Console.WriteLine($"client: {entry}");
                        simconnect.MapClientEventToSimEvent(entry, entry.ToString());
                        simconnect.AddClientEventToNotificationGroup(GROUP_ID.GROUP_A, entry, false);
                    }
                    

                }

                // set Group Priority
                simconnect.SetNotificationGroupPriority(GROUP_ID.GROUP_A, SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST);

                Console.WriteLine("done");
                IsSimConnectConnected = true;
                return simconnect;
            }
            catch
            {
                Console.WriteLine("Unable to connect, Check if MSFS is running!");
                simconnect = null;
                IsSimConnectConnected = false;
                return null;
            }
        }

        public void Update()
        {
            if (simconnect != null)
            {
                simconnect.ReceiveMessage();
                simconnect.ReceiveDispatch(new SignalProcDelegate(MyDispatchProcA));
                foreach (int i in definitions.Keys)
                {
                    // simconnect.RequestDataOnSimObjectType(definitions[(DATA_DEFINE_ID)i].reqId, definitions[(DATA_DEFINE_ID)i].defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);

                    simconnect.RequestDataOnSimObject(definitions[(DATA_DEFINE_ID)i].reqId, definitions[(DATA_DEFINE_ID)i].defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, this.conf.SimconnectFrames, 0);
                }
            }
            else
            {
                ConnectSimConnect();
            }
        }

        void simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT recEvent)
        {
            EVENTS ReceivedEvent = (EVENTS)recEvent.uEventID;
            // Console.WriteLine(ReceivedEvent);

            OnEvent(ReceivedEvent);
        }

        public abstract void OnEvent(EVENTS recEvent);

        public abstract void OnQuit();

        private void MyDispatchProcA(SIMCONNECT_RECV pData, uint cData)
        {
            // Console.WriteLine("MyDispatchProcA "+pData+" "+cData);
        }

        public void setValue(string name, double value)
        {
            DataDefinition def = getDefinitionByName(name);
            simconnect.SetDataOnSimObject(def.defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, value);
        }

        public void setPosition(GeoCoordinate position, double heading) 
        {
            double offset = 0.0;

            GeoCoordinate offsetPos = GeoUtils.GetOffsetPosition(position, heading, offset);

            this.setValue("SIM DISABLED", 1);
            this.setValue("PLANE LATITUDE", offsetPos.Latitude);
            this.setValue("PLANE LONGITUDE", offsetPos.Longitude);
            this.setValue("PLANE ALTITUDE", offsetPos.Altitude);
            this.setValue("PLANE HEADING DEGREES TRUE", heading);
            System.Threading.Thread.Sleep(100);
            this.setValue("SIM DISABLED", 0);
        }

        public void SpawnObject(string objectName, double latitude, double longitude, double altitude)
        {
            if (simconnect != null)
            {
                SIMCONNECT_DATA_INITPOSITION initPos = new SIMCONNECT_DATA_INITPOSITION
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = altitude,
                    Pitch = 0.0f,
                    Bank = 0.0f,
                    Heading = 0.0f,
                    OnGround = 1, // 1 = spawn on ground, 0 = spawn in air
                    Airspeed = 0
                };

                simconnect.AICreateSimulatedObject(objectName, initPos, REQUEST_ID.SPAWN_OBJECT);
                Console.WriteLine($"Spawning {objectName} at {latitude}, {longitude}, {altitude}");
            }
        }


        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data) 
        {
            DataDefinition def = definitions[(DATA_DEFINE_ID)data.dwDefineID];
            if (def.isString)
            {
                DataStruct result = (DataStruct)data.dwData[0];
                //Console.WriteLine("SimConnect " + def.dname + " value: " + result.sValue);
                switch (def.dname)
                {
                    case "ATC MODEL":
                        {
                            Model = result.sValue;
                            break;
                        }
                    case "ATC TYPE":
                        {
                            Type = result.sValue;
                            break;
                        }
                    case "TITLE":
                        {
                            Title = result.sValue;
                            break;
                        }
                }
            }
            else
            {
                //Console.WriteLine("SimConnect " + def.dname + " value: " + data.dwData[0]);
                switch (def.dname)
                {
                    case "PLANE ALTITUDE":
                        {
                            Altitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LATITUDE":
                        {
                            Latitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LONGITUDE":
                        {
                            Longitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE HEADING DEGREES TRUE":
                        {
                            Heading = (double)data.dwData[0];
                            break;
                        }
                    case "GROUND VELOCITY":
                        {
                            GroundSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE ALT ABOVE GROUND MINUS CG":
                        {
                            AltitudeAGL = (double)data.dwData[0];
                            // Console.WriteLine("AltitudeAGL: " + AltitudeAGL);
                            break;
                        }

                    case "VERTICAL SPEED":
                        {
                            VerticalSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE PITCH DEGREES":
                        {
                            pitch = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE BANK DEGREES":
                        {
                            bank = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY X":
                        {
                            vX = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY Y":
                        {
                            vY = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY Z":
                        {
                            vZ = (double)data.dwData[0];
                            break;
                        }
                    case "G FORCE":
                        {
                            gforce = (double)data.dwData[0];
                            break;
                        }
                    case "ON ANY RUNWAY":
                        {
                            IsOnRundway = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM ON GROUND":
                        {
                            IsOnGround = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "ENG COMBUSTION":
                        {
                            IsEngineOn = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM DISABLED":
                        {
                            SimDisabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "BRAKE PARKING POSITION":
                        {
                            IsParkingBreak = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "FUEL TOTAL QUANTITY WEIGHT":
                        {
                            Fuel = (double)data.dwData[0];
                            break;
                        }
                    case "FUEL SELECTED QUANTITY PERCENT":
                        {
                            FuelPercent = (double)data.dwData[0] * 100;
                            break;
                        }
                    case "UNLIMITED FUEL":
                        {
                            FuelUnlimited = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "REALISM CRASH DETECTION":
                        {
                            CrashEnabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "IS SLEW ACTIVE":
                        {
                            IsSlew = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "TOTAL WEIGHT":
                        {
                            TotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "MAX GROSS WEIGHT":
                        {
                            MaxTotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "AIRCRAFT WIND X":
                        {
                            WindX = (double)data.dwData[0];
                            break;
                        }
                    case "AIRCRAFT WIND Z":
                        {
                            WindY = (double)data.dwData[0];
                            break;
                        }
                    case "AIRSPEED TRUE RAW":
                        {
                            airspeedTrueRaw = (double)data.dwData[0];
                            break;
                        }
                    case "AILERON TRIM PCT":
                        {
                            AileronTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "RUDDER TRIM PCT":
                        {
                            RudderTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "ELEVATOR TRIM PCT":
                        {
                            ElevatorTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "RUDDER DEFLECTION PCT":
                        {
                            RudderDefelctionPct = (double)data.dwData[0];
                            break;
                        }
                    case "AILERON RIGHT DEFLECTION PCT":
                        {
                            AileronDefelctionPct = (double)data.dwData[0];
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            DataDefinition def = definitions[(DATA_DEFINE_ID)data.dwDefineID];
            if (def.isString)
            {
                DataStruct result = (DataStruct)data.dwData[0];
                // Console.WriteLine("SimConnect " + def.dname + " value: " + result.sValue);
                switch (def.dname)
                {
                    case "ATC MODEL":
                        {
                            Model = result.sValue;
                            break;
                        }
                    case "ATC TYPE":
                        {
                            Type = result.sValue;
                            break;
                        }
                    case "TITLE":
                        {
                            Title = result.sValue;
                            break;
                        }
                }
            }
            else
            {
                // Console.WriteLine("SimConnect " + def.dname + " value: " + data.dwData[0]);
                switch (def.dname)
                {
                    case "PLANE ALTITUDE":
                        {
                            Altitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LATITUDE":
                        {
                            Latitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LONGITUDE":
                        {
                            Longitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE HEADING DEGREES TRUE":
                        {
                            Heading = (double)data.dwData[0];
                            break;
                        }
                    case "GROUND VELOCITY":
                        {
                            GroundSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE ALT ABOVE GROUND MINUS CG":
                        {
                            AltitudeAGL = (double)data.dwData[0];
                            break;
                        }
                        
                    case "VERTICAL SPEED":
                        {
                            VerticalSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE PITCH DEGREES":
                        {
                            pitch = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE BANK DEGREES":
                        {
                            bank = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY X":
                        {
                            vX = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY Y":
                        {
                            vY = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY BODY Z":
                        {
                            vZ = (double)data.dwData[0];
                            break;
                        }
                    case "G FORCE":
                        {
                            gforce = (double)data.dwData[0];
                            break;
                        }
                    case "ON ANY RUNWAY":
                        {
                            IsOnRundway = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM ON GROUND":
                        {
                            IsOnGround = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "ENG COMBUSTION":
                        {
                            IsEngineOn = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM DISABLED":
                        {
                            SimDisabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "BRAKE PARKING POSITION":
                        {
                            IsParkingBreak = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "FUEL TOTAL QUANTITY WEIGHT":
                        {
                            Fuel = (double)data.dwData[0];
                            break;
                        }
                    case "FUEL SELECTED QUANTITY PERCENT":
                        {
                            FuelPercent = (double)data.dwData[0] * 100;
                            break;
                        }
                    case "UNLIMITED FUEL":
                        {
                            FuelUnlimited = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "REALISM CRASH DETECTION":
                        {
                            CrashEnabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "IS SLEW ACTIVE":
                        {
                            IsSlew = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "TOTAL WEIGHT":
                        {
                            TotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "MAX GROSS WEIGHT":
                        {
                            MaxTotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "AIRCRAFT WIND X":
                        {
                            WindX = (double)data.dwData[0];
                            break;
                        }
                    case "AIRCRAFT WIND Z":
                        {
                            WindY = (double)data.dwData[0];
                            break;
                        }
                    case "AIRSPEED TRUE RAW":
                        {
                            airspeedTrueRaw = (double)data.dwData[0];
                            break;
                        }
                    case "AILERON TRIM PCT":
                        {
                            AileronTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "RUDDER TRIM PCT":
                        {
                            RudderTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "ELEVATOR TRIM PCT":
                        {
                            ElevatorTrimPct = (double)data.dwData[0];
                            break;
                        }
                    case "RUDDER DEFLECTION PCT":
                        {
                            RudderDefelctionPct = (double)data.dwData[0];
                            break;
                        }
                    case "AILERON RIGHT DEFLECTION PCT":
                        {
                            AileronDefelctionPct = (double)data.dwData[0];
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("SimConnect exception: " + data.dwException + " " + data.dwIndex);
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            simconnect.Dispose();
            simconnect = null;
            IsSimConnectConnected = false;
            SimDisabled = false;
            Console.WriteLine("SimConnect quit");
            this.OnQuit();
        }

    }
}
