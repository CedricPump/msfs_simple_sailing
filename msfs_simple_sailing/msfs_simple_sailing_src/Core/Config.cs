
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace msfs_simple_sail_core.Core
{
    internal class Config
    {
        private const string ConfigFilePath = "config.json";

        public int IdleRefreshInterval { get; set; } = 10000;
        public int RefreshInterval { get; set; } = 50;
        public uint SimconnectFrames { get; set; } = 2;
        public bool debug { get; set; } = false;
        public bool alwaysOnTop { get; set; } = true;
        public int uiRefreshIntervall { get; set; } = 1000;
        public int transparencyPercent { get; set; } = 0;
        public bool darkModeEnabled { get; set; } = true;
        public bool darkModeSystem { get; set; } = true;


        private static Config instance = null;

        public static Config Load()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    instance = JsonSerializer.Deserialize<Config>(json) ?? new Config();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading config, using defaults: " + ex.Message);
                    instance = new Config();
                }
            }
            else
            {
                var defaultConfig = new Config();
                defaultConfig.Save(); // Save the default config
                instance = defaultConfig;
            }
            
            return instance;
        }

        public static Config GetInstance() 
        {
            if (instance != null) return instance;
            return Config.Load();
        }  

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            try
            {
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("unable to save config"); 
            }
        }
    }
}
