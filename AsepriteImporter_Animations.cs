using Elendow.SpritedowAnimator;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public partial class AsepriteImporter
{
    

    [Button(ButtonSizes.Large), GUIColor("GetImportButtonColor"), BoxGroup("Importing Animations")]
    public void ImportSheetsToAnimations()
    {
        //lol
        if (GetImportButtonColor() == Color.red)
            return;

        foreach (AsepriteImporterSettingsSO settingsFile in SavedSettingsAsset)
        {
            if (settingsFile == null)
                continue;
            List<DirectoryFilePair> pairs = new List<DirectoryFilePair>();
            DirSearch(settingsFile.OutputRootPathSheets, pairs);

            foreach (var pair in pairs)
            {
                //Directory.CreateDirectory("Assets/Scripts/Editor/" + cmd.exportPath);
                foreach (var spritesheetPath in pair.Files)
                {
                    Object[] spriteSheet = AssetDatabase.LoadAllAssetsAtPath(spritesheetPath);
                    var sprites = spriteSheet.Where(q => q is Sprite).Cast<Sprite>().ToList();
                    SpriteAnimation animation = CreateInstance<SpriteAnimation>();
                    animation.FPS = 10;
                    //animation.Frames = sprites;
                    animation.name = Path.GetFileNameWithoutExtension(spritesheetPath);
                    Debug.Log(animation.name);
                    animation.Setup();

                    Debug.Log(spritesheetPath);
                    Debug.Log(sprites.Count);
                    foreach (Sprite sprite in sprites)
                    {
                        SpriteAnimationFrame spritedowFrame = new SpriteAnimationFrame(sprite, 1); // is 1 in ms?? no idea, I used 1 before
                        Debug.Log(sprite.name);
                        animation.Frames.Add(spritedowFrame);
                        //animation.FramesDuration.Add(1); // frameDuration no longer exists (upgrading to v1.2)
                    }

                    string endPath = spritesheetPath.Replace(settingsFile.OutputRootPathSheets, "");
                    endPath = endPath.Substring(0, endPath.Length - 4); // remove .png
                    string directoryPath = Path.GetDirectoryName(settingsFile.OutputRootPathAnims + endPath);
                    var info = Directory.CreateDirectory(directoryPath);
                    AssetDatabase.CreateAsset(animation, settingsFile.OutputRootPathAnims + endPath + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    private void DirSearch(string dir, List<DirectoryFilePair> pairs)
    {
        try
        {
            string[] files = Directory.GetFiles(dir, "*.png");
            DirectoryFilePair pair = new DirectoryFilePair(dir, files);
            pairs.Add(pair);

            foreach (string d in Directory.GetDirectories(dir))
                DirSearch(d, pairs);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private Color GetImportButtonColor()
    {
        return Color.white;
    }

    private void NotImplementedWarning()
    {
        foreach (AsepriteImporterSettingsSO settingsFile in SavedSettingsAsset)
        {
            if (settingsFile.OutputType == OutputAsset.Mechanim)
                Debug.LogWarning("This setting is not implemented yet: [" + settingsFile.OutputType.ToString() + "]");
        }
    }


    private class DirectoryFilePair
    {
        public DirectoryFilePair(string directoryName, string[] fileNames)
        {
            Directory = directoryName;
            Files = fileNames;
        }

        public string Directory;
        public string[] Files;

        public void Print()
        {
            Debug.Log("Directory: " + Directory);
            foreach (var file in Files)
                Debug.Log("File: "+ file);
        }
    }
}

#endif