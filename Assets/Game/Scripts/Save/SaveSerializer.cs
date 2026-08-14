using System;
using System.Text.Json;

namespace TheOldRoad.Save
{
    public static class SaveSerializer
    {
        public const int CurrentVersion = 2;
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };

        public static string Serialize(SaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            data.saveVersion = CurrentVersion;
            return JsonSerializer.Serialize(data, Options);
        }

        public static bool TryDeserialize(string json, out SaveData data)
        {
            data = null;
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                SaveData candidate = JsonSerializer.Deserialize<SaveData>(json, Options);
                if (candidate == null || candidate.saveVersion < 1 || candidate.saveVersion > CurrentVersion) return false;
                data = candidate;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
