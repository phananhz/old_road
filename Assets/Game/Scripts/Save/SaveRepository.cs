using System;
using System.IO;
using UnityEngine;

namespace TheOldRoad.Save
{
    public sealed class SaveRepository
    {
        private readonly string savePath;

        public SaveRepository(string savePath)
        {
            this.savePath = savePath;
        }

        public string SavePath => savePath;

        public static SaveRepository CreateDefault()
        {
            return new SaveRepository(Path.Combine(Application.persistentDataPath, "the-old-road-vertical-slice.save.json"));
        }

        public bool TryLoad(out SaveData data, out string status)
        {
            data = null;
            if (!File.Exists(savePath))
            {
                status = "No save found. Starting a new vertical slice.";
                return false;
            }

            try
            {
                string json = File.ReadAllText(savePath);
                if (!SaveSerializer.TryDeserialize(json, out data))
                {
                    status = "Save file exists but is invalid or unsupported.";
                    return false;
                }

                status = "Save loaded.";
                return true;
            }
            catch (Exception ex)
            {
                status = "Save load failed: " + ex.Message;
                return false;
            }
        }

        public bool TrySave(SaveData data, out string status)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                string tempPath = savePath + ".tmp";
                string backupPath = savePath + ".bak";

                File.WriteAllText(tempPath, SaveSerializer.Serialize(data));
                if (File.Exists(savePath)) File.Copy(savePath, backupPath, true);
                if (File.Exists(savePath)) File.Delete(savePath);
                File.Move(tempPath, savePath);

                status = "Saved.";
                return true;
            }
            catch (Exception ex)
            {
                status = "Save failed: " + ex.Message;
                return false;
            }
        }
    }
}
