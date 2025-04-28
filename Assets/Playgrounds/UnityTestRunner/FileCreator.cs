using System;
using System.IO;
using UnityEngine;

public class FileCreator
{
    public static string k_Directory = "FileDirectory";

    public FileCreator()
    {
    }

    public void CreateEmptyFile(string expectedFileName)
    {   
        File.Create(Path.Combine(k_Directory, expectedFileName));
    }
}