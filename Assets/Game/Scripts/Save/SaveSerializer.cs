using System;
using UnityEngine;

namespace TheOldRoad.Save
{
    public static class SaveSerializer
    {
        public const int CurrentVersion = 3;

        public static string Serialize(SaveData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            data.saveVersion = CurrentVersion;
            return JsonUtility.ToJson(data, true);
        }

        public static bool TryDeserialize(string json, out SaveData data)
        {
            data = null;
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                SaveData candidate = JsonUtility.FromJson<SaveData>(json);
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
