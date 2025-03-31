using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class BinaryDataStream
{
    // Saves any serializable object to a binary file
    // Parameters:
    //   serializedObject - The object to serialize and save
    //   FileName - Name of the file (without extension)
    public static void Save<T>(T serializedObject, string FileName)
    {
        // Set up save directory in persistent data path
        string path = Application.persistentDataPath + "/saves/";
        Directory.CreateDirectory(path); // Ensure directory exists

        BinaryFormatter formater = new BinaryFormatter();
        // Create or overwrite the file
        FileStream fileStream = new FileStream(path + FileName + ".dat", FileMode.Create);

        try
        {
            // Serialize and write the object to file
            formater.Serialize(fileStream, serializedObject);
        }
        catch (SerializationException e)
        {
            Debug.LogError("save file. Error:" + e.Message);
        }
        finally
        {
            fileStream.Close(); // Always ensure stream is closed
        }
    }

    // Checks if a save file exists
    // Parameters:
    //   FileName - Name of the file to check (without extension)
    // Returns:
    //   bool - True if file exists, false otherwise
    public static bool Exist(string FileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fileName = FileName + ".dat";
        return File.Exists(path + fileName);
    }

    // Reads and deserializes an object from a binary file
    // Parameters:
    //   FileName - Name of the file to read (without extension)
    // Returns:
    //   T - The deserialized object of specified type
    public static T Read<T>(string FileName)
    {
        string path = Application.persistentDataPath + "/saves/";
        BinaryFormatter formater = new BinaryFormatter();
        // Open existing file for reading
        FileStream fileStream = new FileStream(path + FileName + ".dat", FileMode.Open);
        T returnType = default(T); // Initialize default return value

        try
        {
            // Deserialize the file contents
            returnType = (T)formater.Deserialize(fileStream);
        }
        catch (SerializationException e)
        {
            Debug.LogError("read file. Error:" + e.Message);
        }
        finally
        {
            fileStream.Close(); // Always ensure stream is closed
        }

        return returnType;
    }
}