using System;
using System.IO;
using System.Text.Json;

namespace GuitarRigLauncher
{
    public class Settings
    {
        public string GuitarRigPath { get; set; } = "";

        public static Settings LoadFrom(string settingsPath)
        {
            if (!File.Exists(settingsPath)) throw new FileNotFoundException();
            return JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsPath));
        }

        public void Save(string path)
        {
            try
            {
                File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));

            }
            catch (Exception)
            {
                //todo: warning
            }
        }
    }
}