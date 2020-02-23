using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Persistor
{
    static string path = Application.persistentDataPath + "save.eee";

    public static void save(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData load()
    {
        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        else{
            Debug.LogError("Save file not found");
            return null;
        }
    }

}
