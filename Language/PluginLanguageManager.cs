using SuchByte.MacroDeck.Language;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace sugoides.HWiNFO64_Plugin.Language;

public static class PluginLanguageManager
{
    public static PluginStrings PluginStrings = new PluginStrings();

    public static void Initialize()
    {
        LoadLanguage();
        LanguageManager.LanguageChanged += (s, e) => LoadLanguage();
    }

    private static void LoadLanguage()
    {
        string languageName = LanguageManager.GetLanguageName();
        try
        {
            using TextReader reader = new StringReader(GetXMLLanguageResource(languageName));
            PluginStrings = (PluginStrings)new XmlSerializer(typeof(PluginStrings)).Deserialize(reader);
        }
        catch
        {
            PluginStrings = new PluginStrings();
        }
    }

    private static string GetXMLLanguageResource(string languageName)
    {
        var assembly = typeof(PluginStrings).Assembly;
        var allNames = assembly.GetManifestResourceNames();

        // 通过后缀匹配定位资源，避免依赖 RootNamespace 具体值
        string suffix = $".Resources.Languages.{languageName}.xml";
        string resourceName = allNames.FirstOrDefault(n => n.EndsWith(suffix));
        if (resourceName == null)
        {
            // 回退到英文（英文资源必须存在，否则视为构建错误）
            resourceName = allNames.FirstOrDefault(n => n.EndsWith(".Resources.Languages.English.xml"));
        }

        using var resourceStream = assembly.GetManifestResourceStream(resourceName);
        using var streamReader = new StreamReader(resourceStream);
        return streamReader.ReadToEnd();
    }
}
