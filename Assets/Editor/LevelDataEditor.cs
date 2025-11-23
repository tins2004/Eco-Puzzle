using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private TileType selectedType = TileType.T00_Null;

    public override void OnInspectorGUI()
    {
        LevelData level = (LevelData)target;

        // ===== MAP SETTINGS =====
        EditorGUILayout.LabelField("Map Settings", EditorStyles.boldLabel);
        if (GUILayout.Button("Tạo lại bản đồ"))
        {
            level.Init();
        }

        selectedType = (TileType)EditorGUILayout.EnumPopup("Paint Tile Type", selectedType);

        if (level.tiles == null || level.tiles.Length == 0)
        {
            EditorGUILayout.HelpBox("Nhấn nút tạo/tạo lại để làm mới hoặc tạo bản đồ.", MessageType.Info);
        }
        else
        {
            float cellWidth = 40f;
            float cellHeight = cellWidth / 2f;

            for (int x = 0; x < level.height; x++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space((x % 2 != 0 ? 0 : cellWidth / 2f));

                for (int y = 0; y < level.width; y++)
                {
                    int index = x * level.width + y;
                    TileData tile = level.tiles[index];

                    string shortName = tile.type.ToString();
                    shortName = shortName.Length > 3 ? shortName.Substring(0, 3) : shortName;

                    bool isNullTile = tile.type == TileType.T00_Null;
                    if (isNullTile) tile.active = false;

                    string displayText = (!tile.active || isNullTile) ? " " : shortName;

                    if (GUILayout.Button(displayText, GUILayout.Width(cellWidth), GUILayout.Height(cellHeight)))
                    {
                        tile.type = selectedType;
                        tile.active = (selectedType != TileType.T00_Null);
                        EditorUtility.SetDirty(level);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        level.gridSize = EditorGUILayout.FloatField("Grid Size", level.gridSize);

        EditorGUILayout.Space();

        // ===== LEVEL LIMIT SETTINGS =====
        EditorGUILayout.LabelField("Level Limit Settings", EditorStyles.boldLabel);
        level.limitType = (LimitType)EditorGUILayout.EnumPopup("Limit Type", level.limitType);

        if (level.limitType == LimitType.TimeLimit)
        {
            level.limitValue = EditorGUILayout.IntField("Time Limit (seconds)", level.limitValue);
        }
        else if (level.limitType == LimitType.MoveLimit)
        {
            level.limitValue = EditorGUILayout.IntField("Move Limit", level.limitValue);
        }

        EditorGUILayout.Space();

        // ===== LEVEL SCORING =====
        EditorGUILayout.LabelField("Level Scoring", EditorStyles.boldLabel);
        level.maxScore = EditorGUILayout.IntField("Max Score", level.maxScore);

        EditorGUILayout.Space();

        // ===== MISSIONS =====
        EditorGUILayout.LabelField("Missions", EditorStyles.boldLabel);

        // Tile Missions
        EditorGUILayout.LabelField("Tile Missions", EditorStyles.miniBoldLabel);
        for (int i = 0; i < level.tileMissions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            level.tileMissions[i].tileType = (TileType)EditorGUILayout.EnumPopup(level.tileMissions[i].tileType);
            level.tileMissions[i].requiredCount = EditorGUILayout.IntField(level.tileMissions[i].requiredCount);
            if (GUILayout.Button("X", GUILayout.Width(20))) level.tileMissions.RemoveAt(i);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        if (GUILayout.Button("Thêm Tile Mission")) level.tileMissions.Add(new TileMission());

        if (GUILayout.Button("Tạo Tile Mission từ Map"))
        {
            GenerateTileMissions(level);
        }

        // Animal Missions
        EditorGUILayout.LabelField("Animal Missions", EditorStyles.miniBoldLabel);
        for (int i = 0; i < level.animalMissions.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            level.animalMissions[i].animalType = (AnimalType)EditorGUILayout.EnumPopup(level.animalMissions[i].animalType);
            level.animalMissions[i].requiredCount = EditorGUILayout.IntField(level.animalMissions[i].requiredCount);
            if (GUILayout.Button("X", GUILayout.Width(20))) level.animalMissions.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Thêm Animal Mission")) level.animalMissions.Add(new AnimalMission());

        // tile lock
        EditorGUILayout.LabelField("Tile lock", EditorStyles.miniBoldLabel);
        for (int i = 0; i < level.tileLocks.Count; i++)
        {
            level.tileLocks[i].SetPosition(EditorGUILayout.Vector2IntField("Tile Pos", level.tileLocks[i].position));
            EditorGUILayout.BeginHorizontal();
            level.tileLocks[i].SetRequiredCount(EditorGUILayout.IntField(level.tileLocks[i].requiredCount));
            if (GUILayout.Button("X", GUILayout.Width(20))) level.tileLocks.RemoveAt(i);
            EditorGUILayout.EndHorizontal();

        }
        if (GUILayout.Button("Thêm Tile lock")) level.tileLocks.Add(new TileLock());

        if (GUI.changed) EditorUtility.SetDirty(level);
    }

    private void GenerateTileMissions(LevelData level)
    {
        // Gom dữ liệu theo TileType
        Dictionary<TileType, TileMission> missionMap = new Dictionary<TileType, TileMission>();

        for (int x = 0; x < level.height; x++)
        {
            for (int y = 0; y < level.width; y++)
            {
                int index = x * level.width + y;
                TileData tile = level.tiles[index];

                if (tile.active && tile.type != TileType.T00_Null)
                {
                    // Nếu chưa có mission cho tile này → tạo mới
                    if (!missionMap.ContainsKey(tile.type))
                    {
                        missionMap[tile.type] = new TileMission
                        {
                            tileType = tile.type,
                            requiredCount = 0,
                            positions = new List<Vector2Int>()
                        };
                    }

                    // Tăng số lượng
                    missionMap[tile.type].requiredCount++;

                    // Ghi tọa độ
                    missionMap[tile.type].positions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Ghi vào LevelData
        level.tileMissions = missionMap.Values.ToList();

        EditorUtility.SetDirty(level);
        Debug.Log("Đã tạo Tile Mission từ Map (có thêm vị trí từng tile)!");
    }

}
