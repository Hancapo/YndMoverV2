using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls.Ribbon;
using CodeWalker.GameFiles;
using SharpDX;

namespace YND_Mover;

public class PathUtils
{
    public static int GetAreaIDFromNodePosition(Vector3 position)
    {
        int AreaID = 0;
        int Result = 0;
        float Xpos = -8192f;
        float Ypos = -8192f;

        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                float xmin = Xpos;
                float xmax = Xpos + 512f;
                float ymin = Ypos;
                float ymax = Ypos + 512f;

                float node_x = position.X;
                float node_y = position.Y;
                bool v = ymin < node_y && node_y < ymax;
                bool v1 = xmin < node_x && node_x < xmax;

                if (v1 && v)
                {
                    Result = AreaID;
                }
                AreaID += 1;
                Xpos += 512f;
            }
            Ypos += 512f;
            Xpos = -8192f;
        }

        return Result;
    }

    public static List<YndNode> GetNodesFromYNDs(List<string> yndfiles)
    {
        List<YndNode> nodes = new();    
        foreach (var yndfile in yndfiles)
        {
            YndFile yf = new();
            yf.Load(File.ReadAllBytes(yndfile));
            nodes.AddRange(yf.Nodes.ToList());
        }
        return nodes;
    }
    
    public static void MoveAllNodes(List<YndNode> nodes, Vector3 position)
    {
        foreach (var node in nodes)
        {
            node.Position += position;
        }
    }
    
    public static void UpdateAreaIDfromNodes(List<YndNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.AreaID = Convert.ToUInt16(GetAreaIDFromNodePosition(node.Position));
        }
    }
    
    public static void GenerateYNDperArea(List<YndNode> nodes, string outputdir)
    {
        var groupedNodes = nodes.GroupBy(node => node.AreaID);
        
        foreach (var group in groupedNodes)
        {
            YndFile yf = new()
            {
                Nodes = group.ToArray(),
                AreaID = group.Key,
                CellX = group.Key % 32,
                CellY = group.Key / 32,
                HasChanged = true,
                Name = $"nodes{group.Key}"
                
            };
            yf.UpdateAllNodePositions();
            yf.BuildBVH();
            var filename = $"nodes{yf.AreaID}.ynd";
            byte[] newBytes = yf.Save();
            File.WriteAllBytes(Path.Combine(outputdir, $"{yf.Name}.ynd"), newBytes);
        }
    }

}
