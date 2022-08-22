using System;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace PalicneKonstrukcijeMKE.Utility;

public static class DataStorage
{
    public static T? Open<T>(out string returnedFilePath, string? FilePath = null, JsonConverter? CustomConverter = null)
    {

        if(FilePath == null)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if ((bool)ofd.ShowDialog()!)
            {
                returnedFilePath = ofd.FileName;
                return ReadFromJSONFile<T>(ofd.FileName, CustomConverter);
            }
            else
            {
                returnedFilePath = string.Empty;
                return default(T);
            }
        }
        else
        {
            returnedFilePath = FilePath;
            return ReadFromJSONFile<T>(FilePath);
        }
    }

    public static string Save<T>(T ObjectToSave, string? FilePath = null)
    {
        SaveFileDialog sfd = new SaveFileDialog();

        if (FilePath != null)
        {
            return WriteToJSONFile<T>(FilePath, ObjectToSave);
        }
        else if ((bool)sfd.ShowDialog()!)
        {
            return WriteToJSONFile<T>(sfd.FileName, ObjectToSave);
        }
        else return "";
    }

    #region private

    private static T? ReadFromJSONFile<T>(string FilePath, JsonConverter? CustomConverter = null)
    {
        string fileContent = File.ReadAllText(FilePath);

        T? jsonObject = default(T);
        try
        {
            if(CustomConverter == null) jsonObject = JsonConvert.DeserializeObject<T>(fileContent);
            else
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(CustomConverter!);
                jsonObject = JsonConvert.DeserializeObject<T>(fileContent, settings);
            }
        }
        catch
        {
            jsonObject = default(T);
        }

        return jsonObject;
    }

    public static string WriteToJSONFile<T>(string filePath, T objectToWrite)
    {
        string json = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented);
        try
        {
            File.WriteAllText(filePath, json);
            return filePath;
        }
        catch
        {
            return "";
        }
    }

    #endregion // private
}
