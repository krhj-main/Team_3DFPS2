﻿/*
 *  The MIT License
 *
 *  Copyright 2018-2021 whiteflare.
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 *  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 *  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 *  IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 *  TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnlitWF
{
    internal static class WFCommonUtility
    {
        private static readonly Regex PAT_DISP_NAME = new Regex(@"^\[(?<label>[A-Z][A-Z0-9]*)\]\s+(?<name>.+)$");
        private static readonly Regex PAT_PROP_NAME = new Regex(@"^_(?<prefix>[A-Z][A-Z0-9]*)_(?<name>.+?)(?<suffix>(?:_\d+)?)$");
        private static readonly Regex PAT_ENABLE_KEYWORD = new Regex(@"^_(?<prefix>[A-Z][A-Z0-9]*)_ENABLE(?<suffix>(?:_\d+)?)$", RegexOptions.Compiled);

        /// <summary>
        /// プロパティのディスプレイ名から、Prefixと名前を分割する。
        /// </summary>
        /// <param name="text">ディスプレイ名</param>
        /// <param name="label">Prefix</param>
        /// <param name="name">名前</param>
        /// <param name="dispName">ディスプレイ文字列</param>
        /// <returns></returns>
        public static bool FormatDispName(string text, out string label, out string name, out string dispName) {
            var mm = PAT_DISP_NAME.Match(text ?? "");
            if (mm.Success) {
                label = mm.Groups["label"].Value.ToUpper();
                name = mm.Groups["name"].Value;
                dispName = "[" + label + "] " + name;
                return true;
            }
            else {
                label = null;
                name = text;
                dispName = text;
                return false;
            }
        }

        /// <summary>
        /// プロパティ名の文字列から、Prefix+Suffixと名前を分割する。
        /// </summary>
        /// <param name="text">プロパティ名</param>
        /// <param name="label">Prefix+Suffix</param>
        /// <param name="name">名前</param>
        /// <returns></returns>
        public static bool FormatPropName(string text, out string label, out string name) {
            var mm = PAT_PROP_NAME.Match(text ?? "");
            if (mm.Success) {
                label = mm.Groups["prefix"].Value.ToUpper() + mm.Groups["suffix"].Value.ToUpper();
                name = mm.Groups["name"].Value;
                return true;
            }
            else {
                label = null;
                name = text;
                return false;
            }
        }

        /// <summary>
        /// プロパティ物理名からラベル文字列を抽出する。特殊な名称は辞書を参照してラベル文字列を返却する。
        /// </summary>
        /// <param name="prop_name"></param>
        /// <returns></returns>
        public static string GetPrefixFromPropName(string prop_name) {
            string label = WFShaderDictionary.SpecialPropNameToLabelMap.GetValueOrNull(prop_name);
            if (label != null) {
                return label;
            }
            string name;
            WFCommonUtility.FormatPropName(prop_name, out label, out name);
            return label;
        }

        /// <summary>
        /// プロパティ物理名から Enable トグルかどうかを判定する。
        /// </summary>
        /// <param name="prop_name"></param>
        /// <returns></returns>
        public static bool IsEnableToggleFromPropName(string prop_name) {
            string label, name;
            WFCommonUtility.FormatPropName(prop_name, out label, out name);
            return IsEnableToggle(label, name);
        }

        /// <summary>
        /// キーワード文字列が Enable キーワードかどうかを判定する。
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static bool IsEnableKeyword(string keyword) {
            return PAT_ENABLE_KEYWORD.IsMatch(keyword);
        }

        /// <summary>
        /// ラベル＋プロパティ名から Enable トグルかどうかを判定する。
        /// </summary>
        /// <param name="label"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsEnableToggle(string label, string name) {
            return label != null && name.ToLower() == "enable";
        }

        /// <summary>
        /// 見つけ次第削除するシェーダキーワード
        /// </summary>
        private static readonly List<string> DELETE_KEYWORD = new List<string>() {
            "_",
            "_ALPHATEST_ON",
            "_ALPHABLEND_ON",
            "_ALPHAPREMULTIPLY_ON",
        };

        /// <summary>
        /// 各マテリアルのEnableキーワードを設定する
        /// </summary>
        /// <param name="mats"></param>
        public static void SetupShaderKeyword(params Material[] mats) {
            // 不要なシェーダキーワードは削除
            foreach (var mat in mats) {
                if (!IsSupportedShader(mat)) {
                    continue;
                }
                foreach (var key in DELETE_KEYWORD) {
                    if (mat.IsKeywordEnabled(key)) {
                        mat.DisableKeyword(key);
                    }
                }
            }
            // Enableキーワードを整理する
#if UNITY_2019_1_OR_NEWER
            foreach (var mat in mats) {
                if (!IsSupportedShader(mat)) {
                    continue;
                }
                for (int idx = 0; idx < mat.shader.GetPropertyCount(); idx++) {
                    var prop_name = mat.shader.GetPropertyName(idx);

                    // 対応するキーワードが指定されているならばそれを設定する
                    var kwd = WFShaderDictionary.SpecialPropNameToKeywordMap.GetValueOrNull(prop_name);
                    if (kwd != null) {
                        var value = 0.001f < Math.Abs(mat.GetFloat(prop_name));
                        SetEnableKeyword(mat, kwd, value);
                        continue;
                    }

                    // Enableプロパティならば、それに対応するキーワードを設定する
                    if (IsEnableToggleFromPropName(prop_name)) {
                        var value = 0.001f < Math.Abs(mat.GetFloat(prop_name));
                        SetEnableKeyword(mat, prop_name.ToUpper(), value);
                        continue;
                    }
                }
            }
#endif
        }

        private static void SetEnableKeyword(Material mat, string kwd, bool value) {
            if (mat.IsKeywordEnabled(kwd) != value) {
                if (value) {
                    mat.EnableKeyword(kwd);
                }
                else {
                    mat.DisableKeyword(kwd);
                }
            }
        }

        /// <summary>
        /// マテリアルの shader を指定の名前のものに変更する。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mats"></param>
        public static void ChangeShader(string name, params Material[] mats) {
            if (string.IsNullOrWhiteSpace(name) || mats.Length == 0) {
                return; // なにもしない
            }
            var newShader = Shader.Find(name);
            if (newShader != null) {
                Undo.RecordObjects(mats, "change shader");
                foreach (var m in mats) {
                    if (m == null) {
                        continue;
                    }
                    var oldShader = m.shader;

                    // 初期化処理の呼び出し (カスタムエディタを取得してAssignNewShaderToMaterialしたかったけど手が届かなかったので静的アクセス)
                    if (WF_DebugViewEditor.IsSupportedShader(newShader)) {
                        WF_DebugViewEditor.PreChangeShader(m, oldShader, newShader);
                    }
                    else if (ShaderCustomEditor.IsSupportedShader(newShader)) {
                        ShaderCustomEditor.PreChangeShader(m, oldShader, newShader);
                    }
                    // マテリアルにシェーダ割り当て
                    m.shader = newShader;
                    // 初期化処理の呼び出し (カスタムエディタを取得してAssignNewShaderToMaterialしたかったけど手が届かなかったので静的アクセス)
                    if (WF_DebugViewEditor.IsSupportedShader(newShader)) {
                        WF_DebugViewEditor.PostChangeShader(m, oldShader, newShader);
                    }
                    else if (ShaderCustomEditor.IsSupportedShader(newShader)) {
                        ShaderCustomEditor.PostChangeShader(m, oldShader, newShader);
                    }
                }
            }
            else {
                Debug.LogErrorFormat("[WF][Common] Shader Not Found in this projects: {0}", name);
            }
        }

        /// <summary>
        /// Object[] -> Material[] のユーティリティ関数。
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Material[] AsMaterials(params UnityEngine.Object[] array) {
            return array == null ? new Material[0] : array.Select(obj => obj as Material).Where(m => m != null).ToArray();
        }

        /// <summary>
        /// ShaderがUnlitWFでサポートされるものかどうか判定する。
        /// </summary>
        /// <param name="shader"></param>
        /// <returns></returns>
        public static bool IsSupportedShader(Shader shader) {
            return shader != null && shader.name.Contains("UnlitWF");
        }

        /// <summary>
        /// ShaderがUnlitWFでサポートされるものかどうか判定する。
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static bool IsSupportedShader(Material mat) {
            return mat != null && IsSupportedShader(mat.shader);
        }

        /// <summary>
        /// 最新リリースのVersionInfo
        /// </summary>
        private static WFVersionInfo LatestVersion = null;

        /// <summary>
        /// 最新リリースのVersionInfoを返却する。不明のときはnullを返却する。
        /// </summary>
        /// <returns></returns>
        public static WFVersionInfo GetLatestVersion() {
            return LatestVersion;
        }

        /// <summary>
        /// 最新リリースのVersionInfoを設定する。
        /// </summary>
        /// <param name="ver"></param>
        public static void SetLatestVersion(WFVersionInfo ver) {
            LatestVersion = ver != null && ver.HasValue() ? ver : null;
        }

        /// <summary>
        /// 指定のバージョン文字列が最新リリースよりも古いかどうか判定する。不明のときはfalseを返す。
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static bool IsOlderShaderVersion(string version) {
            if (LatestVersion == null || version == null) {
                return false;
            }
            return version.CompareTo(LatestVersion.latestVersion) < 0;
        }

        /// <summary>
        /// 最新リリースのダウンロードページを開く。
        /// </summary>
        public static void OpenDownloadPage() {
            if (LatestVersion == null) {
                return;
            }
            Application.OpenURL(LatestVersion.downloadPage);
        }
    }

    [Serializable]
    public class WFVersionInfo
    {
        public string latestVersion;
        public string downloadPage;

        public bool HasValue() {
            return latestVersion != null && downloadPage != null;
        }
    }

    public class WFShaderFunction
    {
        private static List<string> uniqueLabel = new List<string>();

        public readonly string Label;
        public readonly string Prefix;
        public readonly string Name;
        private readonly Func<WFShaderFunction, Material, bool> _contains;

        internal WFShaderFunction(string label, string prefix, string name) : this(label, prefix, name,
                (self, mat) => {
                    var nm = "_" + self.Prefix + "_Enable";
                    if (mat.HasProperty(nm)) {
                        return mat.GetInt(nm) != 0;
                    }
                    return false;
                }
            ) {
        }

        internal WFShaderFunction(string label, string prefix, string name, Func<WFShaderFunction, Material, bool> contains) {
            Label = label;
            Prefix = prefix;
            Name = name;
            _contains = contains;

            if (uniqueLabel.Contains(Label)) {
                Debug.LogWarningFormat("[WF][Common] WFShaderFunction duplicate Label: " + Label);
            }
            else {
                uniqueLabel.Add(Label);
            }
        }

        public bool Contains(Material mat) {
            if (!WFCommonUtility.IsSupportedShader(mat)) {
                return false;
            }
            return _contains(this, mat);
        }

        public static List<string> LabelToPrefix(List<string> labelList) {
            return labelList.Select(LabelToPrefix).Where(prefix => prefix != null).Distinct().ToList();
        }

        public static string LabelToPrefix(string label) {
            return WFShaderDictionary.ShaderFuncList.Where(func => func.Label == label).Select(func => func.Prefix).FirstOrDefault();
        }
    }

    internal enum EditorLanguage
    {
        English, 日本語
    }

    internal class WFI18NTranslation
    {
        public readonly string Before;
        public readonly string After;
        private readonly HashSet<string> Tags = new HashSet<string>();

        public WFI18NTranslation(string before, string after) : this(null, before, after) {
        }

        public WFI18NTranslation(string tag, string before, string after) {
            Before = before;
            After = after;
            AddTag(tag);
        }

        public WFI18NTranslation AddTag(params string[] tags) {
            foreach(var tag in tags) {
                if (tag != null) {
                    Tags.Add(tag);
                }
            }
            return this;
        }

        public bool ContainsTag(string tag) {
            return tag != null && (Tags.Count == 0 || Tags.Contains(tag));
        }

        public bool HasNoTag() {
            return Tags.Count == 0;
        }
    }

    internal static class WFI18N
    {
        private static readonly string KEY_EDITOR_LANG = "UnlitWF.ShaderEditor/Lang";
        private static readonly Dictionary<string, List<WFI18NTranslation>> EN = new Dictionary<string, List<WFI18NTranslation>>();
        private static readonly Dictionary<string, List<WFI18NTranslation>> JA = ToDict(WFShaderDictionary.LangEnToJa);

        private static EditorLanguage? langMode = null;

        public static EditorLanguage LangMode
        {
            get {
                if (langMode == null) {
                    string lang = EditorPrefs.GetString(KEY_EDITOR_LANG);
                    if (lang == "ja") {
                        langMode = EditorLanguage.日本語;
                    }
                    else {
                        langMode = EditorLanguage.English;
                    }
                }
                return langMode.Value;
            }
            set {
                if (langMode != value) {
                    langMode = value;
                    switch (langMode) {
                        case EditorLanguage.日本語:
                            EditorPrefs.SetString(KEY_EDITOR_LANG, "ja");
                            break;
                        default:
                            EditorPrefs.DeleteKey(KEY_EDITOR_LANG);
                            break;
                    }
                }
            }
        }

        static Dictionary<string, List<WFI18NTranslation>> GetDict() {
            switch (LangMode) {
                case EditorLanguage.日本語:
                    return JA;
                default:
                    return EN;
            }
        }

        public static string Translate(string before) {
            before = before ?? "";

            var current = GetDict();
            if (current == null || current.Count == 0) {
                return before; // 無いなら変換しない
            }

            // ラベルなしでテキストが一致するものを検索する
            if (current.TryGetValue(before, out var list)) {
                var after = list.Where(t => t.HasNoTag()).Select(t => t.After).FirstOrDefault();
                if (after != null) {
                    return after;
                }
            }
            // マッチするものがないなら変換しない
            return before;
        }

        public static string Translate(string label, string before) {
            before = before ?? "";

            var current = GetDict();
            if (current == null || current.Count == 0) {
                return before; // 無いなら変換しない
            }

            // テキストと一致する変換のなかからラベルも一致するものを翻訳にする
            if (current.TryGetValue(before, out var list)) {
                var after = list.Where(t => t.ContainsTag(label)).Select(t => t.After).FirstOrDefault();
                if (after != null) {
                    return after;
                }
            }
            // マッチするものがないなら変換しない
            return before;
        }

        private static string SplitAndTranslate(string before) {
            if (WFCommonUtility.FormatDispName(before, out var label, out var text, out var _)) {
                // text がラベルとテキストに分割できるならば
                return "[" + label + "] " + Translate(label, text);
            } else {
                // そうでなければ
                return Translate(before);
            }
        }

        public static GUIContent GetGUIContent(string text) {
            var localized = SplitAndTranslate(text);
            var tooltip = text != localized ? text : null;
            return new GUIContent(localized, tooltip);
        }

        public static GUIContent GetGUIContent(string label, string text, string tooltip = null) {
            string localized = Translate(label, text);
            if (text != localized && tooltip == null) {
                tooltip = text;
            }
            return new GUIContent("[" + label + "] " + localized, tooltip);
        }

        private static Dictionary<string, List<WFI18NTranslation>> ToDict(List<WFI18NTranslation> from) {
            var result = new Dictionary<string, List<WFI18NTranslation>>();

            foreach (var group in from.GroupBy(t => t.Before)) {
                result[group.Key] = new List<WFI18NTranslation>(group.OrderBy(t => t.HasNoTag()));
            }

            return result;
        }
    }

    internal class WFShaderName
    {
        public string Familly { get; private set; }
        public string Variant { get; private set; }
        public string RenderType { get; private set; }
        public string Name { get; private set; }

        public WFShaderName(string familly, string variant, string renderType, string name) {
            this.Familly = familly;
            this.Variant = variant;
            this.RenderType = renderType;
            this.Name = name;
        }
    }

    internal class WFShaderNameVariantComparer : IEqualityComparer<WFShaderName>
    {
        public bool Equals(WFShaderName x, WFShaderName y) {
            if (System.Object.ReferenceEquals(x, y)) {
                return true;
            }
            if (x == null || y == null) {
                return false;
            }
            return x.Familly == y.Familly && x.Variant == y.Variant;
        }

        public int GetHashCode(WFShaderName x) {
            if (System.Object.ReferenceEquals(x, null)) {
                return 0;
            }
            return x.Familly.GetHashCode() ^ x.Variant.GetHashCode();
        }
    }

    internal class WFShaderNameRenderTypeComparer : IEqualityComparer<WFShaderName>
    {
        public bool Equals(WFShaderName x, WFShaderName y) {
            if (System.Object.ReferenceEquals(x, y)) {
                return true;
            }
            if (x == null || y == null) {
                return false;
            }
            return x.Familly == y.Familly && x.RenderType == y.RenderType;
        }

        public int GetHashCode(WFShaderName x) {
            if (System.Object.ReferenceEquals(x, null)) {
                return 0;
            }
            return x.Familly.GetHashCode() ^ x.RenderType.GetHashCode();
        }
    }

    internal static class WFShaderNameDictionary
    {
        public static WFShaderName TryFindFromName(string name) {
            return WFShaderDictionary.ShaderNameList.Where(nm => nm.Name == name).FirstOrDefault();
        }

        public static List<WFShaderName> GetVariantList(WFShaderName name) {
            if (name == null) {
                return new List<WFShaderName>();
            }
            return WFShaderDictionary.ShaderNameList.Where(nm => nm.Familly == name.Familly && nm.RenderType == name.RenderType).ToList();
        }

        public static List<WFShaderName> GetVariantList(WFShaderName name, out List<WFShaderName> other) {
            var result = GetVariantList(name);
            var items = result.Select(p => p.Variant).ToList();

            // 異なるVariantのShaderNameをotherに詰める
            other = new List<WFShaderName>();
            other.AddRange(WFShaderDictionary.ShaderNameList.Where(nm => nm.Familly == name.Familly));
            other.RemoveAll(a => items.Contains(a.Variant));

            return result;
        }

        public static List<WFShaderName> GetRenderTypeList(WFShaderName name) {
            if (name == null) {
                return new List<WFShaderName>();
            }
            return WFShaderDictionary.ShaderNameList.Where(nm => nm.Familly == name.Familly && nm.Variant == name.Variant).ToList();
        }

        public static List<WFShaderName> GetRenderTypeList(WFShaderName name, out List<WFShaderName> other) {
            var result = GetRenderTypeList(name);
            var items = result.Select(p => p.RenderType).ToList();

            // 異なるRenderTypeのShaderNameをotherに詰める
            other = new List<WFShaderName>();
            other.AddRange(WFShaderDictionary.ShaderNameList.Where(nm => nm.Familly == name.Familly));
            other.RemoveAll(a => items.Contains(a.RenderType));

            return result;
        }
    }

    internal class OldPropertyReplacement
    {
        public readonly string beforeName;
        public readonly string afterName;
        public readonly Action<ShaderSerializedProperty> onAfterCopy;

        public OldPropertyReplacement(string beforeName, string afterName, Action<ShaderSerializedProperty> onAfterCopy) {
            this.beforeName = beforeName;
            this.afterName = afterName;
            this.onAfterCopy = onAfterCopy ?? (p => { });
        }

        public OldPropertyReplacement(string beforeName, string afterName) : this(beforeName, afterName, null) {
        }
    }
}

#endif
