/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

//Helper Class used to Write and Read all of local persistent data.
public class FileWriteRead<T>
{
    //Methods
    /// <summary>
    /// Write a list of objects to file in specified path.
    /// </summary>
    /// <param name="path">File's path</param>
    /// <param name="obj">The object to stream to file</param>
    public void WriteToFile(String path, List<T> dataType)
    {
        Stream fileStream;
        if (!File.Exists(path))
            fileStream = File.Create(path);
        else
            fileStream = File.OpenWrite(path);
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(fileStream, dataType);
        fileStream.Close();
    }

    /// <summary>
    /// Reads a file from given path.
    /// </summary>
    /// <param name="path">Path of the file to read</param>
    /// <returns>A list of object from file</returns>
    public List<T> ReadFromFile(String path)
    {
        List<T> list = new List<T>();
        if (File.Exists(path))
        {
            Stream fileStream = File.OpenRead(path);
            BinaryFormatter deserializer = new BinaryFormatter();
            list = (List<T>)deserializer.Deserialize(fileStream);
            fileStream.Close();
            return list;
        }
        else
            return null;
    }

    /// <summary>
    /// Deletes file from given path.
    /// </summary>
    /// <param name="path">A path of the file to delete</param>
    /// <returns>True if file existed and got deleted, false else.</returns>
    public bool DeleteFile(String path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            return true;
        }
        return false;
    }
}
    