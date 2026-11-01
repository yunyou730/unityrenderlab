using System.Collections.Generic;
using Newtonsoft.Json;

namespace ayy.jy.data
{
    // ============================================================
    //  这套 class 是 out/engine 里 json 的**逐字段镜像**，不含任何逻辑。
    //  字段名用 PascalCase，靠 [JsonProperty] 对上 json 里的 snake_case。
    //  格式说明见 jygame-code/docs/09-scene-json说明书.md
    // ============================================================

    // ---------------- sceneNNN.json ----------------

    public class JYSubscene
    {
        [JsonProperty("kind")] public string Kind;
        [JsonProperty("version")] public int Version;
        [JsonProperty("scene_id")] public int SceneId;
        [JsonProperty("size")] public int Size;                    // 网格边长（64）
        [JsonProperty("atlas")] public string Atlas;
        [JsonProperty("atlas_json")] public string AtlasJson;
        [JsonProperty("readme")] public Dictionary<string, string> Readme;
        [JsonProperty("projection")] public JYProjection Projection;
        [JsonProperty("sort")] public JYSortSpec Sort;
        [JsonProperty("height_unit")] public string HeightUnit;    // "pixel"
        [JsonProperty("bounds")] public JYBounds Bounds;
        [JsonProperty("missing_tiles")] public List<int> MissingTiles;

        /// <summary>层号("0".."5") → 该层 64×64 的数据。索引顺序 [y][x]。</summary>
        [JsonProperty("grid")] public Dictionary<string, int[][]> Grid;

        [JsonProperty("layer_names")] public Dictionary<string, string> LayerNames;
        
        [JsonProperty("character")] public JYCharacter Character;
    }

    public class JYProjection
    {
        [JsonProperty("tile_half_w")] public int TileHalfW;        // 18
        [JsonProperty("tile_half_h")] public int TileHalfH;        // 9
        [JsonProperty("grid_w")] public int GridW;                // 36
        [JsonProperty("grid_h")] public int GridH;                // 18
        [JsonProperty("screen_x")] public string ScreenX;          // "(x - y) * 18"
        [JsonProperty("screen_y")] public string ScreenY;          // "(x + y) * 9"
        [JsonProperty("y_axis")] public string YAxis;
        [JsonProperty("origin")] public string Origin;
    }

    public class JYSortSpec
    {
        [JsonProperty("key")] public string Key;                  // "(x + y, x, l)"
        [JsonProperty("layer_index")] public Dictionary<string, int> LayerIndex;
        [JsonProperty("note")] public string Note;
    }

    public class JYBounds
    {
        [JsonProperty("min_x")] public int MinX;
        [JsonProperty("min_y")] public int MinY;
        [JsonProperty("max_x")] public int MaxX;
        [JsonProperty("max_y")] public int MaxY;
    }

    public class JYCharacter
    {
        [JsonProperty("sort_layer")] public int SortLayer;        // 3
        [JsonProperty("player")] public JYPlayer Player;
    }

    public class JYPlayer
    {
        [JsonProperty("ids")] public List<int> Ids;               // 2501..2528
        [JsonProperty("frames_per_direction")] public int FramesPerDirection;
        [JsonProperty("idle_frame")] public int IdleFrame;
        [JsonProperty("walk_frames")] public List<int> WalkFrames;
        [JsonProperty("directions")] public List<string> Directions;
        [JsonProperty("note")] public string Note;
    }

    // ---------------- atlas_sceneNNN.json ----------------

    public class JYTileAtlas
    {
        [JsonProperty("kind")] public string Kind;
        [JsonProperty("version")] public int Version;
        [JsonProperty("bank")] public string Bank;                // "smap"
        [JsonProperty("scene_id")] public int SceneId;
        [JsonProperty("image")] public string Image;
        [JsonProperty("width")] public int Width;
        [JsonProperty("height")] public int Height;
        [JsonProperty("gutter")] public int Gutter;
        [JsonProperty("format")] public string Format;            // "RGBA32"
        [JsonProperty("filter")] public string Filter;            // "point"
        [JsonProperty("readme")] public Dictionary<string, string> Readme;
        [JsonProperty("tiles")] public Dictionary<string, JYTile> Tiles;
    }

    // ---------------- 图块信息（两个 json 里结构一样） ----------------

    public class JYTile
    {
        /// <summary>图集里的像素矩形 [x, y, w, h]，原点在左上。</summary>
        [JsonProperty("rect")] public int[] Rect;

        /// <summary>归一化 UV [u0, v0, u1, v1]，Unity 约定：原点在左下（v0 是底边，v1 是顶边）。</summary>
        [JsonProperty("uv")] public float[] Uv;

        /// <summary>这张图的像素尺寸 [w, h]。</summary>
        [JsonProperty("size")] public int[] Size;

        /// <summary>锚点 [dx, dy] —— 这张图"站在地上的那一点"。贴图左上角 = 格顶点 - 锚点。</summary>
        [JsonProperty("anchor")] public int[] Anchor;

        /// <summary>调色板动画帧数（1 = 静态）。</summary>
        [JsonProperty("frames")] public int Frames;

        /// <summary>只在图块库里不存在该编号时才出现（洋红占位）。</summary>
        [JsonProperty("missing")] public bool Missing;
    }
}
