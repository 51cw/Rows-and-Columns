using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class BinaryDataStream
{
    public static void Save<T>(T serializedObject, string FileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path);

        BinaryFormatter formater = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + FileName + ".dat", FileMode.Create);

        try
        {
            formater.Serialize(fileStream, serializedObject);
        }
        catch(SerializationException e)
        {
            Debug.LogError("save file. Error:" + e.Message);
        }
        finally 
        { 
            fileStream.Close(); 
        }
    }

    public static bool Exist(string FileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fileName = FileName + ".dat";
        return File.Exists(path + fileName);
    }

    public static T Read<T>(string FileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter formater = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + FileName + ".dat", FileMode.Open);
        T returnType = default(T);

        try
        {
            returnType = (T)formater.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.LogError("read file. Error:" + e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnType;
    }
}
