using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class JYTilePreview : MonoBehaviour
    {
        [Tooltip("Relative to StreamingAssets/JYTest/SubScenes, or an absolute path.")]
        public string _sceneJsonPath = "scene001/scene001.json";

        [Tooltip("Optional. Empty derives it from the scene json.")]
        public string _atlasJsonPath = "";

        [Tooltip("Optional. Empty derives it from the atlas json image field.")]
        public string _texturePath = "";

        public int _tileId = 179;
        public float _pixelsPerUnit = 1.0f;

        private static readonly Dictionary<string, Texture2D> TextureCache =
            new Dictionary<string, Texture2D>();

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private void OnEnable()
        {
            Rebuild();
        }

        private void OnValidate()
        {
            Rebuild();
        }

        [ContextMenu("Rebuild Preview")]
        public void Rebuild()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            if (_meshFilter == null || _meshRenderer == null)
            {
                Debug.LogError("[JYTilePreview] MeshFilter or MeshRenderer is missing.", this);
                return;
            }

            string scenePath = ResolvePath(_sceneJsonPath);
            if (!File.Exists(scenePath))
            {
                Debug.LogError("[JYTilePreview] Scene json not found: " + scenePath, this);
                return;
            }

            JYSubscene scene = JsonConvert.DeserializeObject<JYSubscene>(File.ReadAllText(scenePath));
            if (scene == null)
            {
                Debug.LogError("[JYTilePreview] Failed to parse scene json: " + scenePath, this);
                return;
            }

            string sceneDirectory = Path.GetDirectoryName(scenePath);
            string atlasRelativePath = string.IsNullOrEmpty(_atlasJsonPath)
                ? Path.Combine(sceneDirectory, scene.AtlasJson)
                : _atlasJsonPath;
            string atlasPath = ResolvePath(atlasRelativePath);
            if (!File.Exists(atlasPath))
            {
                Debug.LogError("[JYTilePreview] Atlas json not found: " + atlasPath, this);
                return;
            }

            JYTileAtlas atlas = JsonConvert.DeserializeObject<JYTileAtlas>(File.ReadAllText(atlasPath));
            if (atlas == null || atlas.Tiles == null)
            {
                Debug.LogError("[JYTilePreview] Failed to parse atlas json: " + atlasPath, this);
                return;
            }

            JYTile tile;
            if (!atlas.Tiles.TryGetValue(_tileId.ToString(), out tile) || tile == null)
            {
                Debug.LogError("[JYTilePreview] Tile id not found in atlas: " + _tileId, this);
                return;
            }

            if (tile.Size == null || tile.Size.Length < 2 ||
                tile.Uv == null || tile.Uv.Length < 4 ||
                tile.Size[0] <= 0 || tile.Size[1] <= 0)
            {
                Debug.LogError("[JYTilePreview] Tile has invalid size or uv: " + _tileId, this);
                return;
            }

            string texturePath = string.IsNullOrEmpty(_texturePath)
                ? Path.Combine(Path.GetDirectoryName(atlasPath), atlas.Image)
                : ResolvePath(_texturePath);
            Texture2D texture = LoadTexture(texturePath);
            if (texture == null)
            {
                return;
            }

            BuildMesh(tile);
            ApplyTexture(texture);
        }

        private void BuildMesh(JYTile tile)
        {
            float pixelsPerUnit = Mathf.Max(0.0001f, _pixelsPerUnit);
            float halfWidth = tile.Size[0] / pixelsPerUnit * 0.5f;
            float halfHeight = tile.Size[1] / pixelsPerUnit * 0.5f;

            float u0 = tile.Uv[0];
            float v0 = tile.Uv[1];
            float u1 = tile.Uv[2];
            float v1 = tile.Uv[3];

            Mesh mesh = _meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                mesh.name = "JYTilePreviewMesh";
                mesh.hideFlags = HideFlags.HideAndDontSave;
                _meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            mesh.SetVertices(new[]
            {
                new Vector3(-halfWidth, 0.0f, halfHeight),
                new Vector3(halfWidth, 0.0f, halfHeight),
                new Vector3(-halfWidth, 0.0f, -halfHeight),
                new Vector3(halfWidth, 0.0f, -halfHeight)
            });
            mesh.SetUVs(0, new[]
            {
                new Vector2(u0, v1),
                new Vector2(u1, v1),
                new Vector2(u0, v0),
                new Vector2(u1, v0)
            });
            mesh.SetTriangles(new[] { 0, 1, 2, 1, 3, 2 }, 0);
            mesh.RecalculateBounds();
        }

        private void ApplyTexture(Texture2D texture)
        {
            if (_meshRenderer.sharedMaterial == null)
            {
                Shader shader = Shader.Find("JYTest/TilePreview");
                if (shader == null)
                {
                    Debug.LogError("[JYTilePreview] Shader JYTest/TilePreview not found.", this);
                    return;
                }

                Material material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
                _meshRenderer.sharedMaterial = material;
            }

            if (_propertyBlock == null)
            {
                _propertyBlock = new MaterialPropertyBlock();
            }

            _propertyBlock.SetTexture("_BaseMap", texture);
            _propertyBlock.SetColor("_BaseColor", Color.white);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        private static Texture2D LoadTexture(string path)
        {
            Texture2D texture;
            if (TextureCache.TryGetValue(path, out texture) && texture != null)
            {
                return texture;
            }

            if (!File.Exists(path))
            {
                Debug.LogError("[JYTilePreview] Texture not found: " + path);
                return null;
            }

            texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path), true))
            {
                Debug.LogError("[JYTilePreview] Failed to load texture: " + path);
                if (Application.isPlaying)
                {
                    Object.Destroy(texture);
                }
                else
                {
                    Object.DestroyImmediate(texture);
                }
                return null;
            }

            texture.name = Path.GetFileName(path);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.anisoLevel = 0;
            texture.hideFlags = HideFlags.HideAndDontSave;
            TextureCache[path] = texture;
            return texture;
        }

        private static string ResolvePath(string path)
        {
            return Path.IsPathRooted(path) ? path : JYPaths.FullPath(path);
        }
    }
}