using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceWall=1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionStructure2 = CheckPositionStructure2Structure1();
        switch (relativePositionStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomUpDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomUpDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomRightLeft(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomRightLeft(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }

    private void ProcessRoomRightLeft(Node structure2, Node structure1)
    {
        Node leftStructure = null;
        List<Node> leftStructureChildren = StructureHelper.TraverseGraphLowestLeaves(structure1);
        Node rightStructure = null;
        List<Node> rightStructureChildren = StructureHelper.TraverseGraphLowestLeaves(structure2);

        var sortedleftStructure = leftStructureChildren.OrderByDescending(child => child.TopRightCorner.x).ToList();
        if (sortedleftStructure.Count==1)
        {
            leftStructure = sortedleftStructure[0];

        }
        else
        {
            int maxX = sortedleftStructure[0].TopRightCorner.x;
            sortedleftStructure = sortedleftStructure.Where(children => Math.Abs(maxX - children.TopRightCorner.x) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedleftStructure.Count);
            leftStructure = sortedleftStructure[index];

        }
        var possibleNeighboursInRightStructureList = rightStructureChildren.Where(
            child => GetValidYForNeighborLeftRight(
                leftStructure.TopRightCorner,
                leftStructure.BottomRightCorner,
                child.TopLeftCorner,
                child.BottomLeftCorner
            ) != -1
            ).OrderBy(child=>child.BottomRightCorner.x).ToList();
        if(possibleNeighboursInRightStructureList.Count<=0)
        {
            rightStructure = structure2;

        }
        else
        {
            rightStructure = possibleNeighboursInRightStructureList[0];

        }
        int y = GetValidYForNeighborLeftRight(leftStructure.TopLeftCorner, leftStructure.BottomRightCorner, rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        while(y==-1 && sortedleftStructure.Count>0)
        {
            sortedleftStructure = sortedleftStructure.Where(child => child.TopLeftCorner.y != leftStructure.TopLeftCorner.y).ToList();
            leftStructure = sortedleftStructure[0];
            y = GetValidYForNeighborLeftRight(leftStructure.TopLeftCorner, leftStructure.BottomRightCorner, rightStructure.TopLeftCorner, rightStructure.BottomLeftCorner);
        }
        BottomLeftCorner = new Vector2Int(leftStructure.BottomRightCorner.x, y);
        TopRightCorner = new Vector2Int(rightStructure.TopLeftCorner.x, y + this.corridorWidth);
    }

    private int GetValidYForNeighborLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown, Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if(rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMidpoint(
                leftNodeDown + new Vector2Int(0, modifierDistanceWall),
                leftNodeUp - new Vector2Int(0, modifierDistanceWall + this.corridorWidth)
                ).y;
        }
        if(rightNodeUp.y <= leftNodeUp.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMidpoint(
               rightNodeDown+new Vector2Int(0,modifierDistanceWall),
               rightNodeUp - new Vector2Int(0,modifierDistanceWall+this.corridorWidth)
               ).y;
        }
        if(leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMidpoint(
             rightNodeDown + new Vector2Int(0, modifierDistanceWall),
             leftNodeUp - new Vector2Int(0, modifierDistanceWall)
             ).y;
        }
        if(leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMidpoint(
             leftNodeDown+new Vector2Int(0,modifierDistanceWall),
             rightNodeUp-new Vector2Int(0,modifierDistanceWall+this.corridorWidth)
             ).y;
        }
        return -1;
    }

    private void ProcessRoomUpDown(Node structure1, Node structure2)
    {
        Node bottomStructure = null;
        List<Node> structureBottmChildren = StructureHelper.TraverseGraphLowestLeaves(structure1);
        Node topStructure = null;
        List<Node> structureAboveChildren = StructureHelper.TraverseGraphLowestLeaves(structure2);

        var sortedBottomStructure = structureBottmChildren.OrderByDescending(child => child.TopRightCorner.y).ToList();

        if (sortedBottomStructure.Count == 1)
        {
            bottomStructure = structureBottmChildren[0];
        }
        else
        {
            int maxY = sortedBottomStructure[0].TopLeftCorner.y;
            sortedBottomStructure = sortedBottomStructure.Where(child => Mathf.Abs(maxY - child.TopLeftCorner.y) < 10).ToList();
            int index = UnityEngine.Random.Range(0, sortedBottomStructure.Count);
            bottomStructure = sortedBottomStructure[index];
        }

        var possibleNeighboursInTopStructure = structureAboveChildren.Where(
            child => GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                child.BottomLeftCorner,
                child.BottomRightCorner)
            != -1).OrderBy(child => child.BottomRightCorner.y).ToList();
        if (possibleNeighboursInTopStructure.Count == 0)
        {
            topStructure = structure2;
        }
        else
        {
            topStructure = possibleNeighboursInTopStructure[0];
        }
        int x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                topStructure.BottomLeftCorner,
                topStructure.BottomRightCorner);
        while (x == -1 && sortedBottomStructure.Count > 1)
        {
            sortedBottomStructure = sortedBottomStructure.Where(child => child.TopLeftCorner.x != topStructure.TopLeftCorner.x).ToList();
            bottomStructure = sortedBottomStructure[0];
            x = GetValidXForNeighbourUpDown(
                bottomStructure.TopLeftCorner,
                bottomStructure.TopRightCorner,
                topStructure.BottomLeftCorner,
                topStructure.BottomRightCorner);
        }
        BottomLeftCorner = new Vector2Int(x, bottomStructure.TopLeftCorner.y);
        TopRightCorner = new Vector2Int(x + this.corridorWidth, topStructure.BottomLeftCorner.y);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMidpoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMidpoint(
                topNodeLeft + new Vector2Int(modifierDistanceWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceWall, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMidpoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceWall, 0)

                ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMidpoint(
                topNodeLeft + new Vector2Int(modifierDistanceWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceWall, 0)

                ).x;
        }
        return -1;
    }

    private RelativePosition CheckPositionStructure2Structure1()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopRightCorner+structure1.BottomLeftCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopRightCorner + structure2.BottomLeftCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);
        if((angle<45 && angle>=0) || (angle>-45 && angle<0))
        {
            return RelativePosition.Right;
        }
        else if(angle>45 && angle < 135)
        {
            return RelativePosition.Up;

        }
        else if(angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y - middlePointStructure1Temp.y,
            middlePointStructure2Temp.x - middlePointStructure1Temp.x)*Mathf.Rad2Deg;
    }
}