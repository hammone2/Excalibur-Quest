using UnityEngine;
using UnityEditor;
using System.IO;

public static class FBXAnimationExtractor
{
    [MenuItem("Tools/Animation/Extract Animations From Selected FBX")]
    private static void ExtractAnimations()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog(
                "No FBX Selected",
                "Please select an FBX file in the Project window.",
                "OK"
            );

            return;
        }

        string fbxPath = AssetDatabase.GetAssetPath(selectedObject);

        if (string.IsNullOrEmpty(fbxPath) ||
            Path.GetExtension(fbxPath).ToLower() != ".fbx")
        {
            EditorUtility.DisplayDialog(
                "Invalid Selection",
                "Please select an FBX file in the Project window.",
                "OK"
            );

            return;
        }

        // Get all assets contained inside the FBX.
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

        int extractedCount = 0;

        // Create an "ExtractedAnimations" folder next to the FBX.
        string fbxDirectory = Path.GetDirectoryName(fbxPath);
        string outputFolder = fbxDirectory + "/ExtractedAnimations";

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            string parentFolder = fbxDirectory;
            string newFolderName = "ExtractedAnimations";

            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }

        foreach (Object asset in assets)
        {
            AnimationClip sourceClip = asset as AnimationClip;

            if (sourceClip == null)
                continue;

            // Unity generates several internal animation clips
            // that should not normally be extracted.
            if (sourceClip.name.StartsWith("__preview__"))
                continue;

            string clipName = SanitizeFileName(sourceClip.name);

            string assetPath =
                outputFolder + "/" + clipName + ".anim";

            // Prevent accidentally overwriting an existing animation.
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            // Duplicate the AnimationClip.
            AnimationClip newClip =
                Object.Instantiate(sourceClip);

            newClip.name = clipName;

            // Create the .anim asset.
            AssetDatabase.CreateAsset(newClip, assetPath);

            extractedCount++;

            Debug.Log(
                $"Extracted animation: {sourceClip.name} -> {assetPath}"
            );
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Extraction Complete",
            $"Extracted {extractedCount} animation clip(s).\n\n" +
            $"Output folder:\n{outputFolder}",
            "OK"
        );

        // Select the output folder.
        Object outputFolderObject =
            AssetDatabase.LoadAssetAtPath<Object>(outputFolder);

        if (outputFolderObject != null)
        {
            Selection.activeObject = outputFolderObject;
            EditorGUIUtility.PingObject(outputFolderObject);
        }
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char invalidCharacter in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(
                invalidCharacter.ToString(),
                "_"
            );
        }

        return fileName;
    }

    // Enables the menu item only when an FBX is selected.
    [MenuItem(
        "Tools/Animation/Extract Animations From Selected FBX",
        true
    )]
    private static bool ValidateExtractAnimations()
    {
        Object selectedObject = Selection.activeObject;

        if (selectedObject == null)
            return false;

        string path = AssetDatabase.GetAssetPath(selectedObject);

        return !string.IsNullOrEmpty(path) &&
               Path.GetExtension(path).ToLower() == ".fbx";
    }
}