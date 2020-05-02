using System;
using System.Collections.Generic;
using System.Linq;

public class CorridorsGenerator
{
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();
        Queue<RoomNode> structuresCheck = new Queue<RoomNode>(
            allNodesCollection.OrderByDescending(Node => Node.TreeLayerIndex).ToList());
        while(structuresCheck.Count > 0)
        {
            var node = structuresCheck.Dequeue();
            if(node.ChildrenNodeList.Count == 0)
            {
                continue;
            }
            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0],node.ChildrenNodeList[1],corridorWidth);
            corridorList.Add(corridor);

        }
        return corridorList;
    }
}