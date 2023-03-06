#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AsepriteExporter
{
    public static string PathToAsepriteExe = ASEPRITE_STANDARD_PATH_WINDOWS;
    private const string ASEPRITE_STANDARD_PATH_WINDOWS = @"C:\Program Files\Aseprite\Aseprite.exe";
    private const string ASEPRITE_STANDARD_PATH_MACOSX = @"/Applications/Aseprite.app/Contents/MacOS/aseprite";

    public string PathToAsepriteFile = @"Assets/Art/Admurin/Aseprite/_SheetMaster_Tagged.aseprite";
    //public string PathToAsepriteFile = @"Assets/Art/Admurin/Aseprite/_SheetMasterDeaths_Tagged.aseprite";
    public string PathToAsepriteExportJson = @"Assets\Scripts\Editor\aseprite-exporter-settings.json";
    //public string PathToAsepriteExportJson = @"Assets\Scripts\Editor\aseprite-exporter-settings_deaths_test.json";
    public string PathToOutputRoot;
    public int CurrentIteration = 0;
    public int MaxIteration = 0;

    private JsonSettings _jsonSettings;
    private string _workingDirectory;
    private bool _ready = false;
    private Thread _thread;

    public void Setup(AsepriteImporterSettingsSO settings)
    {
        PathToAsepriteExportJson = settings.AsepriteJsonSettings;
        PathToAsepriteFile = settings.AsepriteFile;
        PathToOutputRoot = settings.OutputRootPathSheets;
        _workingDirectory = Application.dataPath.Replace("Assets", "");
        _ready = true;
    }

    public void ExportAllTagsAsSpriteSheetForEachLayer()
    {
        if (_ready == false)
            return;

        ReadInJSONSettings();
        _thread = new Thread(new ThreadStart(RunCLI));
        _thread.Start();
    }

    private void ReadInJSONSettings()
    {
        TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath(PathToAsepriteExportJson, typeof(TextAsset));
        _jsonSettings = JsonUtility.FromJson<JsonSettings>(asset.text);
    }

    //private List<int> RunCLI()
    private void RunCLI()
    {
        //List<int> results = new List<int>();
        MaxIteration = _jsonSettings.exportCommands.Length;
        CurrentIteration = 0;

        Parallel.ForEach(_jsonSettings.exportCommands, cmd =>
        {
/*            if (Directory.Exists("Assets/Scripts/Editor/" + cmd.exportPath))
            {
                Directory.CreateDirectory("Assets/Scripts/Editor/" + cmd.exportPath);
            }*/

            char space = ' ';
            string layerParam = "";
            foreach (var layer in cmd.layers)
            {
                layerParam += space + "--layer" + space + $"\"{layer}\"";
            }

            foreach (var tag in _jsonSettings.tags)
            {
                string tagParam = space + "--tag" + space + $"\"{tag}\"";
                string filePathParam = space + PathToAsepriteFile;
                //string sheetTypeParam = space + "--sheet-pack";
                string sheetTypeParam = "";
                string modifiedTag = tag.Replace(" ", "");
                modifiedTag = modifiedTag.Replace(":", "-");
                string saveAsParam = space + "--sheet" + space + $@"{PathToOutputRoot}/{cmd.exportPath}/{cmd.name + "_" + modifiedTag.Replace(" ", "")}.png";
                string finalParameters = layerParam + tagParam + filePathParam + saveAsParam + sheetTypeParam;

                //string workingDirectory = Application.dataPath.Replace("Assets", "") + @"Assets/Scripts/Editor/";

                //Debug.Log(workingDirectory);
                System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
                start.Arguments = " -b " + finalParameters;
                //Debug.Log(start.Arguments);
                start.FileName = ASEPRITE_STANDARD_PATH_WINDOWS;
                start.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                start.CreateNoWindow = true;
                start.UseShellExecute = false;
                start.WorkingDirectory = _workingDirectory;
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;

                // Run the external process & wait for it to finish
                using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start(start))
                {
//                    string output = proc.StandardOutput.ReadToEnd();
//                    Debug.Log(output);
                    string error = proc.StandardError.ReadToEnd();
                    if (error.Length > 0)
                        Debug.Log("Error: " + error);
                    proc.WaitForExit();
                    //results.Add(proc.ExitCode);
                }
            }
            Interlocked.Increment(ref CurrentIteration);
        });
    }

    public void BlockToFinish()
    {
        if (_thread != null)
            _thread.Join();
    }

    [System.Serializable]
    public class JsonSettings
    {
        public CommandSettings[] exportCommands;
        public string[] tags;

        [System.Serializable]
        public class CommandSettings
        {
            public string name;
            public string[] layers;
            public string exportPath;
        }
    }
}
#endif