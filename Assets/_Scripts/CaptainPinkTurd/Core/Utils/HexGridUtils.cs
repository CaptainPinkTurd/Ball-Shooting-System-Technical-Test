using UnityEngine;

namespace CaptainPinkTurd.Core.Utils
{
    public static class HexGridUtils
    {
        //q is column, r is row
        public static Vector3Int AxialToCube(int q, int r)
        {
            int x = q;
            int z = r;
            int y = -x - z;

            return new Vector3Int(x, y, z);
        }
        public static int GetCubeHexManhattanDistance(Vector3Int startCoord, Vector3Int endCoord)
        {
            return Mathf.Max(
                Mathf.Abs(startCoord.x - endCoord.x),
                Mathf.Abs(startCoord.y - endCoord.y),
                Mathf.Abs(startCoord.z - endCoord.z)
            );
        }
    }
}
