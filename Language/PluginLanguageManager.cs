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
        if (string.IsNullOrEmpty(languageName)
            || !assembly.GetManifestResourceNames().Any(name => name.EndsWith($"{languageName}.xml")))
        {
            languageName = "English";
        }

        string languageFileName = $"sugoides.HWiNFO64_Plugin.Resources.Languages.{languageName}.xml";

        using var resourceStream = assembly.GetManifestResourceStream(languageFileName);
        using var streamReader = new StreamReader(resourceStream);
        return streamReader.ReadToEnd();
    }
}
