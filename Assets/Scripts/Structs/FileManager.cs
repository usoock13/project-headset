using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class FileManager {
    private static string saveDir = Application.persistentDataPath + "\\headset\\";

    public static void Initialize() {
        if(!Directory.Exists(saveDir))
            Directory.CreateDirectory(saveDir);
    }

    // async public static Task<string> LoadJson(string fileName) {
    //     if(Directory.Exists(saveDir) && File.Exists(fileName)) {
    //         var sr = new StreamReader(fileName);
    //         string json = await sr.ReadToEndAsync();
    //         sr.Close();
    //         return json;
    //     } else
    //         return null;
    // }

    public static string LoadJson(string fileName) {
        if(Directory.Exists(saveDir) && File.Exists(fileName)) {
            var sr = new StreamReader(fileName);
            string json = sr.ReadToEnd();
            sr.Close();
            return json;
        } else
            return null;
    }

    public static void SaveJson(string json, string fileName) {
        if(!File.Exists(saveDir + fileName))
            CreateFile(fileName);
        StreamWriter sw = new StreamWriter(saveDir + fileName);
        sw.WriteLine(json);
        sw.Close();
    }

    private static void CreateFile(string fileName) {
        StreamWriter sw = new StreamWriter(saveDir + fileName);
        sw.WriteLine();
        sw.Close();
    }
}