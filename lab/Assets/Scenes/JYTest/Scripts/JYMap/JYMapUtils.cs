using System.Collections;
using System.Collections.Generic;
using System.IO;
using ayy.jy.data;
using ayy.jy.record;
using Newtonsoft.Json;
using UnityEngine;

namespace ayy.jy
{
    public class JYMapUtils
    {
        // 一个菱形 tile图 本身是矩形的, 这个矩形贴图的 宽,高
        public const int ISO_TILE_WIDTH = 36;
        public const int ISO_TILE_HEIGHT = 18;
        
        public const float ISO_TILE_HALF_WIDTH = ISO_TILE_WIDTH / 2.0f;
        public const float ISO_TILE_HALF_HEIGHT = ISO_TILE_HEIGHT / 2.0f;
        
        // 原本意图是用来描绘 tile 中心
        // 但是时间上, 有可能这个描述的是 tile 的左上角, 和 anchor 配合起来也许是更对的
        // 返回值, 先理解为 "左上角" 吧
        public static void GetCenterPositionAtTile(int tileX, int tileY,out float posX,out float posY)
        {
            float baseX = -tileY * ISO_TILE_HALF_WIDTH;
            float baseY = -tileY * ISO_TILE_HALF_HEIGHT;
            
            posX = baseX + tileX * ISO_TILE_HALF_WIDTH;
            posY = baseY - tileX * ISO_TILE_HALF_HEIGHT;
        }
        
        public static Mesh CreateBaseGrid(int tileCountX,int tileCountY)
        {
            float currentY = 0.0f;
            
            int quadCount = tileCountX * tileCountY;
            
            Vector3[] verts = new Vector3[quadCount * 4];
            Vector2[] uvs = new Vector2[quadCount * 4];
            int[] tris = new int[quadCount * 6];

            int vi = 0;
            int ti = 0;
            for (int tileY = 0; tileY < tileCountY; tileY++)
            {
                for (int tileX = 0; tileX < tileCountX; tileX++)
                {
                    float px, py;
                    JYMapUtils.GetCenterPositionAtTile(tileX,tileY,out px, out py);
                    
                    /*
                     * p0  ---  p1
                     * |        |
                     * |        |
                     * p2  ---  p3 
                     */
                    verts[vi + 0] = new Vector3(px, currentY, py);
                    verts[vi + 1] = new Vector3(px + JYMapUtils.ISO_TILE_WIDTH, currentY, py);
                    verts[vi + 2] = new Vector3(px, currentY, py - JYMapUtils.ISO_TILE_HEIGHT);
                    verts[vi + 3] = new Vector3(px + JYMapUtils.ISO_TILE_WIDTH, currentY, py - JYMapUtils.ISO_TILE_HEIGHT);

                    uvs[vi + 0] = new Vector2(0, 1);
                    uvs[vi + 1] = new Vector2(1, 1);
                    uvs[vi + 2] = new Vector2(0, 0);
                    uvs[vi + 3] = new Vector2(1, 0);

                    
                    tris[ti++] = vi + 0;
                    tris[ti++] = vi + 1;
                    tris[ti++] = vi + 2;
                    tris[ti++] = vi + 1;
                    tris[ti++] = vi + 3;
                    tris[ti++] = vi + 2;
                    
                    vi += 4;
                    
                    currentY += 0.001f;
                }
            }

            Mesh mesh = new Mesh();
            mesh.name = "JYBaseGrid";
            
            mesh.SetVertices(verts);
            mesh.SetUVs(0,uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
    
}