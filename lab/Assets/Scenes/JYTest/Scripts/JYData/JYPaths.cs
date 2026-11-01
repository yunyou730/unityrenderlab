using System.IO;
using UnityEngine;

namespace ayy.jy.data
{
    /// <summary>
    /// 数据文件的位置。
    ///
    /// 目录结构（每个场景一个文件夹）：
    ///
    ///     Assets/StreamingAssets/JYTest/SubScenes/
    ///         scene001/scene001.json                 ← 地图说明书
    ///         scene001/atlas_scene001.json            ← 图集切割说明
    ///         scene001/atlas_scene001.png             ← 图集纹理
    ///
    /// 运行时 = Application.streamingAssetsPath + "/JYTest/SubScenes/..."
    ///
    /// 不依赖 index.json —— 场景编号直接进路径（JYPaths.SceneJsonFile(1)）。
    ///
    /// ⚠️ StreamingAssets 里的文件用 **DefaultImporter**（原样字节），不是 Texture2D ——
    ///    所以贴图没有 Unity 的导入设置可用（过滤模式/压缩/mipmap 全都不存在），
    ///    要在运行时自己解码并设 filterMode。见 JYDataLoader.LoadTexture。
    /// </summary>
    public static class JYPaths
    {
        /// <summary>数据根目录。默认 StreamingAssets；可改成别的。</summary>
        public static string Root = Application.streamingAssetsPath;

        /// <summary>场景数据子目录（相对 Root）。</summary>
        public const string SceneFolder = "JYTest/SubScenes";

        /// <summary>单个场景的文件夹名（相对 SceneFolder）。</summary>
        public static string SceneDir(int sceneId) { return string.Format("scene{0:000}", sceneId); }

        // ---- 单个场景的四个文件（相对 SceneFolder）----

        public static string SceneJsonFile(int sceneId)
        {
            return string.Format("scene{0:000}/scene{0:000}.json", sceneId);
        }

        public static string AtlasJsonFile(int sceneId)
        {
            return string.Format("scene{0:000}/atlas_scene{0:000}.json", sceneId);
        }

        public static string AtlasImageFile(int sceneId)
        {
            return string.Format("scene{0:000}/atlas_scene{0:000}.png", sceneId);
        }

        // ---- 路径拼接 ----

        /// <summary>拼一个绝对路径（走文件系统 IO 时用）。</summary>
        public static string FullPath(string relativeToSceneFolder)
        {
            return Path.Combine(Root, SceneFolder, relativeToSceneFolder);
        }

        /// <summary>拼一个 URL（走 UnityWebRequest 时用）。</summary>
        public static string Url(string relativeToSceneFolder)
        {
            string root = Root.Replace('\\', '/');
            if (!root.EndsWith("/")) root += "/";
            return root + SceneFolder + "/" + relativeToSceneFolder;
        }
    }
}
