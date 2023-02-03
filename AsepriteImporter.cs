using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
public partial class AsepriteImporter : OdinEditorWindow
{
    private static readonly string ASEPRITE_STANDARD_PATH_WINDOWS = @"C:\Program Files\Aseprite\Aseprite.exe";
    private static readonly string ASEPRITE_STANDARD_PATH_MACOSX = @"/Applications/Aseprite.app/Contents/MacOS/aseprite";

    [BoxGroup("SettingsSO"), OnValueChanged("LoadSettingsAsset")]
    public AsepriteImporterSettingsSO[] SavedSettingsAsset;

    [Sirenix.OdinInspector.Title("Drag and drop works too"), BoxGroup("Exporting spritesheets")]
    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true), BoxGroup("Exporting spritesheets"), OnValueChanged("ValidateExecutable")]
    public string AsepriteExecutable = StandardApplicationPath;

    public static string StandardApplicationPath
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return ASEPRITE_STANDARD_PATH_WINDOWS;
            }
            else
            {
                return ASEPRITE_STANDARD_PATH_MACOSX;
            }
        }
    }

    private bool _running = false; // I was trying to use this to change the button color

    [Button(ButtonSizes.Large), GUIColor("GetExportButtonColor"), BoxGroup("Exporting spritesheets")]
    public void ExportToSpritesheets()
    {
        //lol
        if (GetExportButtonColor() == Color.red)
            return;

        _running = true;
        foreach (AsepriteImporterSettingsSO settingsFile in SavedSettingsAsset) {
            if (settingsFile == null)
                continue;

            AsepriteExporter exporter = new AsepriteExporter();
            exporter.Setup(settingsFile);
            exporter.ExportAllTagsAsSpriteSheetForEachLayer();

            exporter.BlockToFinish();
            AssetDatabase.Refresh();
        }
        _running = false;
    }
    private void LoadSettingsAsset()
    {
        if (SavedSettingsAsset.Length > 0)
            Debug.Log("new settings asset! " + SavedSettingsAsset[SavedSettingsAsset.Length].name);
    }

/*    private void LoadJsonPreview()
    {
        TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath(SavedSettingsAsset.AsepriteJsonSettings, typeof(TextAsset));
        if (asset == null)
        {
            Debug.LogError("Not the right type of asset!");
            SavedSettingsAsset.AsepriteJsonSettings = string.Empty;
            return;
        }
        //_jsonSettings = JsonUtility.FromJson<AsepriteExporter.JsonSettings>(asset.text);
    }*/

    private void ValidateExecutable()
    {
        if (AsepriteExecutable.EndsWith("Aseprite.exe") == false)
        {
            AsepriteExecutable = string.Empty;
        }
    }

    private Color GetExportButtonColor()
    {
        if (AsepriteExecutable == string.Empty)
            return Color.red;
        if (SavedSettingsAsset == null)
            return Color.red;
        foreach (AsepriteImporterSettingsSO settingsFile in SavedSettingsAsset)
        {
            if (settingsFile == null)
                continue;
            if (settingsFile.AsepriteFile == string.Empty)
                return Color.red;
            if (settingsFile.AsepriteJsonSettings == string.Empty)
                return Color.red;
            if (settingsFile.OutputRootPathSheets == string.Empty)
                return Color.red;
            if (_running)
                return Color.yellow;
        }

        return Color.green;
    }

    [MenuItem("Tools/Aseprite/Importer")]
    private static void OpenWindow()
    {
        GetWindow<AsepriteImporter>().Show();
    }

    public enum OutputAsset
    {
        Spritedow,
        Mechanim
    }
}

#endif
