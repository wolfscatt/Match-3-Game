using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class NamespaceFromFolder : AssetModificationProcessor
{
    private const string BaseNamespace = "Match3Game";

    // Namespace'in klasörlere göre başlayacağı kök path
    private const string NamespaceRootPath = "Assets/Scripts";

    static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (!path.EndsWith(".cs")) return;

        // Dosya fiziksel olarak olu�madan okuma yapmaya kalkmamak i�in geciktiriyoruz
        EditorApplication.delayCall += () => TryApplyNamespace(path);
    }

    private static void TryApplyNamespace(string assetPath)
    {
        if (!File.Exists(assetPath)) return;

        // Zaten namespace varsa elle dokunma (istersen bunu kald�rabilirsin)
        var text = File.ReadAllText(assetPath);
        if (Regex.IsMatch(text, @"^\s*namespace\s+[A-Za-z0-9_.]+", RegexOptions.Multiline))
            return;

        var ns = BuildNamespaceFromPath(assetPath);
        if (string.IsNullOrWhiteSpace(ns)) return;

        // using bloklar�n�n bitti�i yeri bul
        var lines = text.Replace("\r\n", "\n").Split('\n').ToList();

        int insertIndex = 0;
        while (insertIndex < lines.Count && lines[insertIndex].StartsWith("using "))
            insertIndex++;

        // using'lardan sonra tek bo� sat�r b�rak
        if (insertIndex < lines.Count && string.IsNullOrWhiteSpace(lines[insertIndex]))
            insertIndex++;

        // Kalan i�eri�i indentle
        var before = lines.Take(insertIndex).ToList();
        var body = lines.Skip(insertIndex)
            .Select(l => string.IsNullOrWhiteSpace(l) ? l : "    " + l)
            .ToList();

        var wrapped = before
            .Concat(new[]
            {
                $"namespace {ns}",
                "{"
            })
            .Concat(body)
            .Concat(new[]
            {
                "}",
                ""
            });

        File.WriteAllText(assetPath, string.Join("\n", wrapped));

        AssetDatabase.ImportAsset(assetPath);
        AssetDatabase.Refresh();
    }

    private static string BuildNamespaceFromPath(string assetPath)
    {
        // �rn: Assets/Scripts/Managers/UI/Foo.cs
        // Root: Assets/Scripts
        var dir = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");
        if (string.IsNullOrWhiteSpace(dir)) return BaseNamespace;

        // Root'tan sonras� namespace par�alar� olacak
        if (!dir.StartsWith(NamespaceRootPath))
        {
            // �stersen root d���ndakiler i�in full Assets bazl� da yapabilirsin
            return BaseNamespace;
        }

        var relative = dir.Substring(NamespaceRootPath.Length).Trim('/');
        if (string.IsNullOrWhiteSpace(relative)) return BaseNamespace;

        var parts = relative.Split('/')
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(ToSafeNamespacePart);

        return BaseNamespace + "." + string.Join(".", parts);
    }

    private static string ToSafeNamespacePart(string raw)
    {
        // Klas�r ad�n� namespace par�as�na �evir:
        // "game-manager" -> "GameManager"
        // "2d" -> "_2d"
        // "UI Kit" -> "UIKit"
        var cleaned = Regex.Replace(raw, @"[^A-Za-z0-9]+", " ");
        var words = cleaned.Split(' ')
            .Where(w => !string.IsNullOrWhiteSpace(w))
            .Select(w => char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w.Substring(1) : ""));

        var pascal = string.Concat(words);
        if (string.IsNullOrWhiteSpace(pascal)) pascal = "Folder";

        // Namespace par�as� rakamla ba�layamaz
        if (char.IsDigit(pascal[0])) pascal = "_" + pascal;

        return pascal;
    }
}
