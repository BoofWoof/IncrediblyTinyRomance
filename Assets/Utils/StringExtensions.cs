public static class StringExtensions
{
    public static string CleanResourcePath(this string rawPath)
    {
        if (string.IsNullOrEmpty(rawPath)) return string.Empty;

        // Normalize slashes
        rawPath = rawPath.Replace("\\", "/");

        // Remove "Assets/" if present
        if (rawPath.StartsWith("Assets/"))
        {
            rawPath = rawPath.Substring("Assets/".Length);
        }

        // Remove "Resources/" if present
        int resourcesIndex = rawPath.IndexOf("Resources/");
        if (resourcesIndex >= 0)
        {
            rawPath = rawPath.Substring(resourcesIndex + "Resources/".Length);
        }

        // Remove file extension like ".asset" or ".prefab" if present
        if (rawPath.EndsWith(".asset"))
        {
            rawPath = rawPath.Substring(0, rawPath.Length - ".asset".Length);
        }
        else if (rawPath.EndsWith(".prefab"))
        {
            rawPath = rawPath.Substring(0, rawPath.Length - ".prefab".Length);
        }

        return rawPath;
    }
}