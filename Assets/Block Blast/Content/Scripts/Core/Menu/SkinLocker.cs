using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SkinLocker {

    public static void setupLocker (AllSkins allSkins) {

        string path = Application.persistentDataPath + "/locker.bin";


        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, allSkins);

        stream.Close();

        
    }

    public static AllSkins getLocker () {
        string path = Application.persistentDataPath + "/locker.bin";

        if (File.Exists(path)) {

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            
            AllSkins allSkins = formatter.Deserialize(stream) as AllSkins;

            stream.Close();

            return allSkins;

        } else {
            Debug.Log("No Data Found");
            return null;
        }

    }

    public static void DeleteFile () {
        string path = Application.persistentDataPath + "/locker.bin";
        if (File.Exists(path)) {
            File.Delete(path);
        }
        PlayerPrefs.DeleteKey("TOTAL_SKINS");
    }

}
