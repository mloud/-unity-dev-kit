// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace OneDay.Core.Game.Spatial
// {
//     public interface ISpatial2DObject
//     {
//         Vector2 Position { get; }
//     }
//     public class Spatial2DPartitioning<T> where T : ISpatial2DObject
//     {
//         private List<T>[,] Grid { get; }
//         private int Width { get; }
//         private int Height { get; }
//         private float CellSize { get; }
//         private Func<IEnumerable<T>> GridObjectProvider { get; }
//
//         private List<T> TmpResultList { get; }
//         
//         public Spatial2DPartitioning(int width, int height, float cellSize, Func<IEnumerable<T>> gridObjectProvider)
//         {
//             Width = width;
//             Height = height;
//             Grid = new List<T>[height, width];
//             CellSize = cellSize;
//             GridObjectProvider = gridObjectProvider;
//             TmpResultList = new List<T>();
//         }
//
//         public T GetNearestObject(Vector2 worldPosition, Func<T, bool> objectFilter, float maxDistance)
//         {
//             var gridPosition = PositionToGridCoordinates(worldPosition);
//
//             if (Grid[gridPosition.y, gridPosition.x].Count > 0)
//             {
//                 var nearestObject = FindTheNearestObject(
//                     Grid[gridPosition.y, gridPosition.x], 
//                     worldPosition, 
//                     objectFilter);
//                 
//                 if (nearestObject != null)
//                 {
//                     return nearestObject;
//                 }
//             }
//            
//             int distance = 1;
//
//             while (true)
//             {
//                 TmpResultList.Clear();
//
//                 int xMin = Math.Max(0, gridPosition.x - distance);
//                 int xMax = Math.Min(Width - 1, gridPosition.x + distance);
//                 int yMin = Math.Max(0, gridPosition.y - distance);
//                 int yMax = Math.Min(Height - 1, gridPosition.y + distance);
//
//                 bool existsCell = false;
//                 if (gridPosition.y - distance >= 0)
//                 {
//                     for (int x = xMin; x <= xMax; ++x)
//                     {
//                         if ( (gridPosition - new Vector2Int(yMin, x)).sqrMagnitude + 1 < )
//                         existsCell = true; 
//                         if (Grid[yMin, x].Count > 0)
//                             TmpResultList.AddRange(Grid[yMin, x]);
//                     }
//                 }
//
//                 if (gridPosition.y + distance < Height)
//                 {
//                     for (int x = xMin; x <= xMax; ++x)
//                     {
//                         existsCell = true; 
//                         if (Grid[yMax, x].Count > 0)
//                             TmpResultList.AddRange(Grid[yMax, x]);
//                     }
//                 }
//
//                 if (gridPosition.x - distance >= 0)
//                 {
//                     for (int y = yMin + 1; y < yMax; ++y)
//                     {
//                         existsCell = true; 
//                         if (Grid[y, xMin].Count > 0)
//                             TmpResultList.AddRange(Grid[y, xMin]);
//                     }
//                 }
//
//                 if (gridPosition.x + distance < Width)
//                 {
//                     for (int y = yMin + 1; y < yMax; ++y)
//                     {
//                         existsCell = true; 
//                         if (Grid[y, xMax].Count > 0)
//                             TmpResultList.AddRange(Grid[y, xMax]);
//                     }
//                 }
//
//                 if (!existsCell)
//                     break;
//
//                 if (TmpResultList.Count > 0)
//                 {
//                     var nearestObject = FindTheNearestObject(
//                         Grid[gridPosition.y, gridPosition.x], 
//                         worldPosition, 
//                         objectFilter);
//                     return nearestObject;
//                 }
//
//                 distance++;
//             }
//         }
//         
//         public void UpdateSpatialGrid()
//         {
//             for (int y = 0; y < Height; y++)
//             {
//                 for (int x = 0; x < Width; x++)
//                 {
//                     Grid[y, x] ??= new List<T>();
//                     Grid[y, x].Clear();
//                 }
//             }
//         
//
//             foreach (var spatialObject in GridObjectProvider())
//             {
//                 var gridCoordinates = PositionToGridCoordinates(spatialObject.Position);
//                 if (gridCoordinates.x < 0 || gridCoordinates.x >= Width)
//                 {
//                     throw new ArgumentOutOfRangeException(nameof(gridCoordinates.x), 
//                         $"X coordinate {gridCoordinates.x} is out of grid bounds");
//                 }
//         
//                 if (gridCoordinates.y < 0 || gridCoordinates.y >= Height)
//                 {
//                     throw new ArgumentOutOfRangeException(nameof(gridCoordinates.y), 
//                         $"Y coordinate {gridCoordinates.y} is out of grid bounds");
//                 }
//                 
//                 Grid[gridCoordinates.y, gridCoordinates.x].Add(spatialObject);
//             } 
//         }
//
//         private Vector2Int PositionToGridCoordinates(Vector2 position) =>
//             new(
//                 Mathf.FloorToInt(position.x / CellSize + Width / 2.0f), 
//                 Mathf.FloorToInt(position.y / CellSize + Height / 2.0f));
//
//         private static T FindTheNearestObject(IEnumerable<T> objects, Vector2 fromPosition, Func<T, bool> objectFilter)
//         {
//             float minDistance = float.MaxValue;
//             T nearestObject = default;
//
//             foreach (var obj in objects)
//             {
//                 if (!objectFilter(obj))
//                     continue;
//                 
//                 var sqrDistance = (obj.Position - fromPosition).sqrMagnitude;
//                 if (sqrDistance< minDistance)
//                 {
//                     minDistance = sqrDistance;
//                     nearestObject = obj;
//                 }
//             }
//             return nearestObject;
//         }
//     }
// }