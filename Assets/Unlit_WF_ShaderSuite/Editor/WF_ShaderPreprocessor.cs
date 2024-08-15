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

// #define WF_STRIP_DISABLE
#define WF_STRIP_LOG_RESULT
// #define WF_STRIP_LOG_TRACE
// #define WF_STRIP_LOG_VERBOSE

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;

namespace UnlitWF
{
#if UNITY_2019_1_OR_NEWER

    public class WF_ShaderPreprocessor : IPreprocessShaders
    {
        private bool initialized = false;
        private List<UsedShaderVariant> usedShaderVariantList;
        private WFEditorSetting settings;

        public int callbackOrder
        {
            get {
                ClearUsedShaderVariantList();
                return 100;
            }
        }

        public WF_ShaderPreprocessor() {
            ClearUsedShaderVariantList();
        }

        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data) {
#if !WF_STRIP_DISABLE
            if (IsStripTargetShader(shader)) {
                // 設定はここで読み込む
                InitUsedShaderVariantList();

                if (settings == null || !settings.enableStripping) {
                    // stripping しない
                    return;
                }

                var before = data.Count;
                var strip = 0;
                strip += DoStripForwardBasePass(shader, snippet, data);
                strip += DoStripMetaPass(shader, snippet, data);

#if WF_STRIP_LOG_RESULT
                if (data.Count < before) {
                    Debug.LogFormat("[WF][Preprocess] shader stripping: {0}/{1} at {2}/{3}/{4}", strip, before, shader.name, snippet.passName, snippet.shaderType);
                }
#endif
            }
#endif
        }

        protected int DoStripForwardBasePass(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data) {
            if (snippet.passType != PassType.ForwardBase) {
                // ここで stripping するのは ForwardBase だけ
                return 0;
            }

            // 存在するキーワードの配列
            var existingKwds = GetExistingShaderKeywords(shader, data);
            if (existingKwds.Length == 0) {
                // feature keyword が無いならば何もしない
                return 0;
            }

            var count = 0;

            for (int i = data.Count - 1; 0 <= i; i--) {
                var d = data[i];

                if (ContainsShaderVariant(settings.alwaysIncludeShaders, shader, snippet, d)) {
#if WF_STRIP_LOG_VERBOSE
                        Debug.LogFormat("[WF][Preprocess] always include: {0}/{1}/{2}/{3} ({4})", 
                            shader.name, 
                            snippet.passName, 
                            snippet.shaderType,
                            d.shaderCompilerPlatform,
                            string.Join(", ", ToKeywordArray(shader, d.shaderKeywordSet)));
#endif
                    continue;
                }

                if (usedShaderVariantList.Any(v => v.IsMatchVariant(shader, existingKwds, d))) {
#if WF_STRIP_LOG_VERBOSE
                        Debug.LogFormat("[WF][Preprocess] match variant: {0}/{1}/{2}/{3} ({4})", 
                            shader.name, 
                            snippet.passName, 
                            snippet.shaderType,
                            d.shaderCompilerPlatform,
                            string.Join(", ", ToKeywordArray(shader, d.shaderKeywordSet)));
#endif
                    // 使用しているバリアントならば何もしない
                    continue;
                }
                // 使用してないバリアントは削除
                data.RemoveAt(i);
                count++;
            }

            return count;
        }

        private bool ContainsShaderVariant(ShaderVariantCollection collection, Shader shader, ShaderSnippetData snippet, ShaderCompilerData data) {
            if (collection == null) {
                return false;
            }
            return collection.Contains(new ShaderVariantCollection.ShaderVariant(shader, snippet.passType, ToKeywordArray(shader, data.shaderKeywordSet)));
        }

        protected int DoStripMetaPass(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data) {
            if (snippet.passType != PassType.Meta) {
                // ここで stripping するのは Meta だけ
                return 0;
            }
            if (!settings.stripMetaPass) {
                // 設定で Meta パス削減しないときには何もしない
                return 0;
            }

            int count = data.Count;
            data.Clear();
            return count;
        }

        private string[] ToKeywordArray(Shader shader, ShaderKeywordSet keys) {
            return keys.GetShaderKeywords().Select(kwd => ShaderKeyword.GetKeywordName(shader, kwd)).ToArray();
        }

        private static bool IsStripTargetShader(Shader shader) {
            if (shader == null) {
                return false;
            }
            if (!WFCommonUtility.IsSupportedShader(shader)) {
                return false;
            }
            if (shader.name.Contains("WF_DebugView")) {
                return false;
            }
            if (shader.name.Contains("WF_UnToon_Hidden")) {
                return false;
            }
            return true;
        }

        private string[] GetExistingShaderKeywords(Shader shader, IList<ShaderCompilerData> data) {
            return data.SelectMany(d => d.shaderKeywordSet.GetShaderKeywords())
                .Where(k => ShaderKeyword.IsKeywordLocal(k))
                .Select(k => ShaderKeyword.GetKeywordName(shader, k))
                .Where(kwd => WFCommonUtility.IsEnableKeyword(kwd)).Distinct().ToArray();
        }

        private void ClearUsedShaderVariantList() {
            initialized = false;
            usedShaderVariantList = null;
        }

        private void InitUsedShaderVariantList() {
            if (initialized) {
                // 初期化済みならば何もしない
                return;
            }

            // Assets 内に WF_EditorSetting があるならば読み込み
            settings = WFEditorSetting.GetOneOfSettings();

            // シーンから UsedShaderVariant を回収
            var used = new List<UsedShaderVariant>();
            foreach (var mat in GetAllSceneAllMaterial()) {
                AppendUsedShaderVariant(used, mat, mat.shader);
            }

            // EditorSettings から UsedShaderVariant を回収
            if (settings.alwaysIncludeMaterials != null) {
                foreach (var mat in settings.alwaysIncludeMaterials) {
                    if (mat != null) {
                        AppendUsedShaderVariant(used, mat, mat.shader);
                    }
                }
            }

            usedShaderVariantList = used.Distinct().ToList();
            initialized = true;

#if WF_STRIP_LOG_TRACE
            Debug.LogFormat("[WF][Preprocess] used variants: {0}", usedShaderVariantList.Count);
            foreach (var uv in usedShaderVariantList) {
                Debug.LogFormat("[WF][Preprocess] used variant: {0}", uv);
            }
#endif
        }

        private Material[] GetAllSceneAllMaterial() {
            var result = new List<Material>();
            for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
                var scene = EditorSceneManager.GetSceneAt(i);
                result.AddRange(scene.GetRootGameObjects()
                    .SelectMany(go => go.GetComponentsInChildren<Renderer>(true))
                    .SelectMany(mf => mf.sharedMaterials)
                    .Where(mat => mat != null));
            }
            return result.Distinct().ToArray();
        }

        private void AppendUsedShaderVariant(List<UsedShaderVariant> result, Material mat, Shader shader) {
            if (mat == null || !IsStripTargetShader(shader)) {
                return;
            }

            // マテリアルから _XX_ENABLE となっているキーワードを回収
            IEnumerable<string> keywords = mat.shaderKeywords.Where(kwd => WFCommonUtility.IsEnableKeyword(kwd));

            UsedShaderVariant usv = new UsedShaderVariant(shader.name, keywords);
            if (!result.Contains(usv)) {
                result.Add(usv);

                // 直接のシェーダではなく、そのフォールバックを利用できるならばそれも追加する
                if (settings == null || !settings.stripFallback) {
                    var pi = shader.FindPropertyIndex("_FallBack");
                    if (0 <= pi) {
                        var fallback = shader.GetPropertyDescription(pi);
                        AppendUsedShaderVariant(result, mat, Shader.Find(fallback));
                    }
                }
            }
        }

        public class UsedShaderVariant
        {
            public readonly string shaderName;
            public readonly List<string> keywords;

            public UsedShaderVariant(string shaderName, IEnumerable<string> keywords) {
                this.shaderName = shaderName;
                this.keywords = new List<string>(keywords);
                this.keywords.Sort();
            }

            public override bool Equals(object obj) {
                var variant = obj as UsedShaderVariant;
                return variant != null &&
                       shaderName == variant.shaderName &&
                       EqualityComparer<List<string>>.Default.Equals(keywords, variant.keywords);
            }

            public override int GetHashCode() {
                var hashCode = -94728968;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(shaderName);
                hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(keywords);
                return hashCode;
            }

            public bool IsMatchVariant(Shader shader, IEnumerable<string> existing, ShaderCompilerData data) {
                if (shader.name != shaderName) {
                    return false;
                }

                foreach (var kwd in existing) {
                    if (keywords.Contains(kwd) != data.shaderKeywordSet.IsEnabled(new ShaderKeyword(shader, kwd))) {
                        return false;
                    }
                }
                return true;
            }

            public override string ToString() {
                return shaderName + "(" + string.Join(", ", keywords) + ")";
            }
        }

    }
#endif
}
