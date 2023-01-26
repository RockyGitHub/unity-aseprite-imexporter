using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New AsepriteImporter Settings", menuName = "Scriptable Objects/Settings/AsepriteImporter", order = 1)]
public class AsepriteImporterSettingsSO : ScriptableObject
{
    [Header("Drag and drop works too")]
    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true, Extensions = ".aseprite"), BoxGroup("SpriteSheet Settings"), OnValueChanged("CheckIfAsepriteFile")]
    public string AsepriteFile;
    [Sirenix.OdinInspector.FilePath(RequireExistingPath = true, Extensions = ".json"), BoxGroup("SpriteSheet Settings"), OnValueChanged("LoadJsonPreview")]
    public string AsepriteJsonSettings;
    [FolderPath(RequireExistingPath = true), BoxGroup("SpriteSheet Settings")]
    public string OutputRootPathSheets;


    [EnumPaging, BoxGroup("Animation asset settings"), OnValueChanged("NotImplementedWarning")]
    public AsepriteImporter.OutputAsset OutputType = AsepriteImporter.OutputAsset.Spritedow;

/*    [FolderPath(RequireExistingPath = true), BoxGroup("Animation asset settings")]
    public string InputRootPath;*/

    [FolderPath(RequireExistingPath = true), BoxGroup("Animation asset settings")]
    public string OutputRootPathAnims;

    private void CheckIfAsepriteFile()
    {
        if (AsepriteFile.EndsWith(".aseprite") == false)
        {
            Debug.LogError("Not the right type of asset!");
            AsepriteFile = string.Empty;
            return;
        }
    }

    private void LoadJsonPreview()
    {
        TextAsset asset = (TextAsset)AssetDatabase.LoadAssetAtPath(AsepriteJsonSettings, typeof(TextAsset));
        if (asset == null)
        {
            Debug.LogError("Not the right type of asset!");
            AsepriteJsonSettings = string.Empty;
            return;
        }
        //_jsonSettings = JsonUtility.FromJson<AsepriteExporter.JsonSettings>(asset.text);
    }

    private void NotImplementedWarning()
    {
        if (OutputType == AsepriteImporter.OutputAsset.Mechanim)
            Debug.LogWarning("This setting is not implemented yet: [" + OutputType.ToString() + "]");
    }
}
