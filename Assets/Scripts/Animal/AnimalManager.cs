using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AnimalManager : MonoBehaviour
{
    private LevelData levelData;
    private LevelManager levelManager;

    [Header("Dữ liệu động vật")]
    public List<AnimalData> animals;   // Danh sách animal có sẵn
    public AnimalType selectedAnimal;

    [Header("Đối chiếu")]
    [SerializeField] private GameObject animalIconPrefab;
    [SerializeField] private Transform iconParent;

    // Vùng đất
    [SerializeField] private GameObject regionInfoPrefab;
    [SerializeField] private Transform regionInfoParent;
    [SerializeField] private Sprite[] numberTileForAnimalSprite;
    private List<GameObject> currentRegionInfoBox = new List<GameObject>();

    // key = Region.Id, value = list AnimalType trong vùng đó
    [HideInInspector] public Dictionary<int, List<AnimalType>> currentAnimal = new Dictionary<int, List<AnimalType>>();

    // Icon trên Tile
    [SerializeField] private GameObject markerIconPrefab;
    [SerializeField] private Transform markerIconParent;
    // Quản lý icon spawn trực tiếp trên tile (theo Region.Id)
    private Dictionary<int, List<GameObject>> regionAnimalIcons = new Dictionary<int, List<GameObject>>();


    public void SetLevelData(LevelData data, LevelManager levelManager)
    {
        levelData = data;
        this.levelManager = levelManager;
    }

    public void StartAnimal()
    {
        SpawnAnimalIcons();
    }

    /// <summary>
    /// Sinh icon animal ra màn hình dựa trên mission hiện tại
    /// </summary>
    private void SpawnAnimalIcons()
    {
        // Lấy dictionary nhiệm vụ animal từ MissionManager
        var animalMissions = levelManager.missionManager.GetCurrentMission().Item2;

        foreach (var mission in animalMissions)
        {
            var animal = animals.Find(a => a.type == mission.Key);
            if (animal != null)
            {
                // Icon
                var icon = Instantiate(animalIconPrefab, iconParent);
                icon.name = animal.type.ToString();

                var images = icon.GetComponentsInChildren<Image>();

                if (Enum.TryParse(animal.type.ToString(), out AnimalSize sizeAnimal))
                {
                    images[0].sprite = numberTileForAnimalSprite[(int)sizeAnimal - 1];
                }

                images[1].sprite = animal.icon;

                // Cho con Image full size theo cha - một nửa
                var rt = images[1].GetComponent<RectTransform>();
                rt.sizeDelta = rt.sizeDelta / 2f;
                rt.localPosition = rt.localPosition / 2f;

                // Gắn sự kiện chọn animal
                var btn = icon.GetComponent<Button>();
                btn.onClick.AddListener(() => SelectAnimal(animal));
            }
        }
    }

    /// <summary>
    /// Cập nhật icon animal, nếu nhiệm vụ xong thì xoá icon luôn
    /// </summary>
    private void UpdateAnimalIcons()
    {
        var animalMissions = levelManager.missionManager.GetCurrentMission().Item2;

        foreach (var mission in animalMissions)
        {
            var icon = iconParent.Find(mission.Key.ToString());
            if (icon != null)
            {
                var text = icon.GetComponentInChildren<TMP_Text>();

                if (mission.Value <= 0)
                {
                    // Nhiệm vụ xong thì xoá icon
                    Destroy(icon.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Khi click chọn 1 animal
    /// </summary>
    public void SelectAnimal(AnimalData animal)
    {
        if (selectedAnimal != AnimalType.A00_Null)
        {
            // Nếu đã chọn animal khác, bỏ chọn
            ResetSelection();
            return;
        }


        selectedAnimal = animal.type;
        // Debug.Log($"Chọn: {selectedAnimal}");

        var inappropriateRegions = ListOfInappropriateRegions(selectedAnimal);

        if (levelManager.gridMapManager != null)
            levelManager.gridMapManager.HighlightRegion(selectedAnimal, ListCoordinatesOfInappropriateTiles(inappropriateRegions));

        if (Enum.TryParse(selectedAnimal.ToString(), out AnimalRegion tile))
            ShowRegionInfo((TileType)tile, inappropriateRegions);

        ShowMarkerIcon(false);
    }

    /// <summary>
    /// Đặt lại lựa chọn động vật
    /// </summary>
    public void ResetSelection()
    {
        selectedAnimal = AnimalType.A00_Null;
        if (levelManager.gridMapManager != null)
            levelManager.gridMapManager.ClearHighlight();

        if (currentRegionInfoBox.Count > 0)
        {
            currentRegionInfoBox.ForEach(info => Destroy(info));
            currentRegionInfoBox.Clear();
        }

        ShowMarkerIcon(true);
    }

    /// <summary>
    /// Lấy danh sách những vùng không phù hợp với animalType
    /// </summary>
    private List<Region> ListOfInappropriateRegions(AnimalType animalType)
    {
        var result = new List<Region>();

        if (!Enum.TryParse(animalType.ToString(), out AnimalRegion regionType))
            return result;

        if (!Enum.TryParse(animalType.ToString(), out AnimalSize sizeAnimal))
            return result;

        // Debug.Log("Bắt đầu lấy danh sách các tile không phù hợp");

        foreach (var region in levelManager.gridMapManager.GetRegionsByTileType((TileType)regionType))
        {
            if (region.Tiles.Count < (int)sizeAnimal)
            {
                result.Add(region);
            }
            else
            {
                int currentTotalSize = 0;
                var animalsInRegion = GetAnimalsInRegion(region);
                // Debug.Log($"Region có {animalsInRegion.Count} động vật: {string.Join(", ", animalsInRegion)}");
                foreach (var animal in animalsInRegion)
                {
                    if (Enum.TryParse(animal.ToString(), out AnimalSize aSize))
                    {
                        currentTotalSize += (int)aSize;
                    }
                }

                // Nếu region và các animal trong đó nhỏ hơn kích thước cần thiết
                if ((region.Tiles.Count - currentTotalSize) < (int)sizeAnimal)
                {
                    result.Add(region);
                }
            }
        }

        // Debug.Log($"Danh sách các vùng không phù hợp: {string.Join(", ", result)}");

        return result;
    }

    /// <summary>
    /// Lấy chính xác các vị trí các Tile của vùng không phù hợp
    /// </summary>
    private List<Vector3Int> ListCoordinatesOfInappropriateTiles(List<Region> inappropriateRegions)
    {
        var result = new List<Vector3Int>();

        foreach (var region in inappropriateRegions)
        {
            foreach (var tile in region.Tiles)
            {
                result.Add(tile.Coordinates);
            }
        }

        // Debug.Log($"Danh sách các tile không phù hợp: {string.Join(", ", result)}");

        return result;
    }

    /// <summary>
    /// Hiển thị thông tin các vùng cho loại tile từ startTile.
    /// </summary>
    /// <param name="tileType"></param>
    /// <param name="inappropriateRegions"></param>
    public void ShowRegionInfo(TileType tileType, List<Region> inappropriateRegions)
    {
        if (regionInfoPrefab == null || regionInfoParent == null) return;

        // Lấy toàn bộ các vùng ứng với loại tile này
        var regions = levelManager.gridMapManager.GetRegionsByTileType(tileType);

        for (int i = 0; i < regions.Count; i++)
        {
            if (inappropriateRegions.Contains(regions[i]))
                continue; // Bỏ qua vùng không phù hợp

            var localRegion = regions[i];
            var animalsInRegion = GetAnimalsInRegion(localRegion); // Lấy danh sách động vật hiện tại trong vùng (nếu có)

            // Tính vị trí trung bình của cả vùng để đặt box
            Vector3 avgPos = Vector3.zero;
            foreach (var tile in localRegion.Tiles)
            {
                avgPos += tile.transform.position;
            }
            avgPos /= localRegion.Tiles.Count;

            // --Spawn UI box--
            GameObject newBox = Instantiate(regionInfoPrefab, regionInfoParent);

            // --Text--
            TMP_Text text = newBox.GetComponentInChildren<TMP_Text>();
            int totalAnimalSize = 0;
            foreach (var animal in animalsInRegion)
            {
                if (Enum.TryParse(animal.ToString(), out AnimalSize sizeAnimal))
                {
                    totalAnimalSize += (int)sizeAnimal;
                }
            }
            text.text = $"{totalAnimalSize}/{localRegion.Tiles.Count}";

            // --Icons--
            var icons = newBox.GetComponentInChildren<GridLayoutGroup>().GetComponentsInChildren<Image>(true);

            // Ẩn tất cả trước
            foreach (var img in icons)
                img.gameObject.SetActive(false);

            // Bật số icon = số con thú, gán sprite
            for (int j = 0; j < animalsInRegion.Count; j++)
            {
                if (j < icons.Length)
                {
                    icons[j].sprite = animalsInRegion[j] != AnimalType.A00_Null ? animals.Find(a => a.type == animalsInRegion[j])?.icon : null;
                    icons[j].gameObject.SetActive(true);
                }
            }

            // --Button--
            var btn = newBox.GetComponentInChildren<Button>();
            btn.onClick.AddListener(() => SelectRegion(localRegion));

            // Đặt vị trí box trên màn hình (theo trung tâm vùng)
            newBox.GetComponent<RectTransform>().position = avgPos;

            currentRegionInfoBox.Add(newBox);
        }
    }

    /// <summary>
    /// Chọn một vùng (danh sách tile) để đặt động vật vào
    /// </summary>
    /// <param name="region"></param>
    private void SelectRegion(Region region)
    {
        if (selectedAnimal == AnimalType.A00_Null) return;
        if (!levelManager.missionManager.GetCurrentMission().Item2.ContainsKey(selectedAnimal)) return; // Nếu không có trong nhiệm vụ

        // Nếu region chưa có thì khởi tạo
        if (!currentAnimal.ContainsKey(region.Id))
            currentAnimal[region.Id] = new List<AnimalType>();


        // Nếu đủ 4 con rồi thì không thêm nữa
        if (currentAnimal[region.Id].Count >= 4)
        {
            Debug.Log("Region này đã đủ 4 động vật, không thể thêm!");
            return;
        }

        // Thêm con thú vào list
        currentAnimal[region.Id].Add(selectedAnimal);
        SpawnAnimalOnRegion(region, selectedAnimal);

        // Đánh dấu nhiệm vụ
        levelManager.missionManager.CollectAnimal(selectedAnimal);
        UpdateAnimalIcons();

        // Reset selection
        ResetSelection();

        // Debug log
        // var testAnimalsInRegion = GetAnimalsInRegion(region);
        // string log = $"Region có {testAnimalsInRegion.Count} động vật:\n";
        // foreach (var a in testAnimalsInRegion)
        //     log += $"- {a}\n";
        // Debug.Log(log);
    }

    public List<AnimalType> GetAnimalsInRegion(Region region)
    {
        if (currentAnimal.TryGetValue(region.Id, out var animals))
        {
            // Debug.Log($"{region} có {animals.Count} động vật.");
            // Debug.Log($"{region} có {string.Join(", ", animals)} động vật.");
            return animals;
        }

        return new List<AnimalType>();
    }

    /// <summary>
    /// Spawn 1 icon animal lên 1 tile bất kỳ trong region.
    /// Nếu có nhiều animal, sẽ phân bổ ra nhiều tile.
    /// </summary>
    private void SpawnAnimalOnRegion(Region region, AnimalType animalType)
    {
        if (animalIconPrefab == null) return;

        // Nếu region chưa có entry trong dictionary thì tạo mới
        if (!regionAnimalIcons.ContainsKey(region.Id))
            regionAnimalIcons[region.Id] = new List<GameObject>();

        var existingIcons = regionAnimalIcons[region.Id];

        // Lấy list tile khả dụng (chưa có icon nào đứng trên)
        var usedTiles = new HashSet<GridTileBase>(
            existingIcons.Select(icon => icon.GetComponent<AnimalIconMarker>()?.Tile)
                        .Where(t => t != null)
        );

        var availableTiles = region.Tiles.Where(t => !usedTiles.Contains(t)).ToList();

        // Nếu còn tile trống thì lấy random
        GridTileBase targetTile;
        if (availableTiles.Count > 0)
        {
            targetTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
        }
        else
        {
            // Nếu hết tile trống, thì cứ chọn random trong tất cả region
            // targetTile = region.Tiles[UnityEngine.Random.Range(0, region.Tiles.Count)];`
            targetTile = null;
            Debug.LogWarning($"Region {region.Id} đã đầy icon, không thể spawn thêm!");
        }

        // Spawn icon trên tile
        var iconObj = Instantiate(markerIconPrefab, markerIconParent); // hoặc có thể đặt icon trực tiếp trong world
        iconObj.transform.position = targetTile.transform.position + Vector3.up * 0.2f; // dịch lên chút để nhìn rõ
        iconObj.name = $"{animalType}_{region.Id}_{targetTile.Coordinates}";

        // Set sprite icon
        var animalData = animals.Find(a => a.type == animalType);
        if (animalData != null)
        {
            var images = iconObj.GetComponentsInChildren<SpriteRenderer>();
            if (images.Length > 1)
            {
                if (Enum.TryParse(animalType.ToString(), out AnimalSize sizeAnimal))
                {
                    images[0].sprite = numberTileForAnimalSprite[(int)sizeAnimal - 1];
                }

                images[1].sprite = animalData.icon;
            }
        }

        // Gắn marker để biết icon này thuộc tile nào
        var marker = iconObj.AddComponent<AnimalIconMarker>();
        marker.Tile = targetTile;

        // Lưu vào dictionary
        existingIcons.Add(iconObj);
    }


    public void ShowMarkerIcon(bool show)
    {
        markerIconParent.gameObject.SetActive(show);
    }
    
    /// <summary>
    /// Tìm marker theo vị trí tile (coordinates).
    /// </summary>
    /// <param name="coordinates">Tọa độ tile (Vector3Int).</param>
    /// <returns>GameObject marker nếu có, ngược lại null.</returns>
    public GameObject GetMarkerByTile(Vector3Int coordinates)
    {
        string coordStr = coordinates.ToString(); // ví dụ "(3, 2, 0)"
        
        foreach (Transform child in markerIconParent.transform)
        {
            if (child.name.Contains(coordStr))
            {
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Di chuyển marker trong region nếu tile cũ bị thay.
    /// </summary>
    public void RelocateMarker(Region region, GridTileBase oldTile)
    {
        if (GetMarkerByTile(oldTile.Coordinates) is GameObject marker)
        {
            // var neighbors = oldTile.GetNeighbors(); // đã tính đúng 6 hướng hex
            var connectedOldTiles = levelManager.gridMapManager.GetConnectedTiles(oldTile, t => t.GetTileType() == oldTile.GetTileType());

            // Giữ lại phần tên cũ nhưng bỏ đoạn tọa độ ở cuối
            string oldName = marker.name;
            int lastUnderscore = oldName.LastIndexOf("_");
            string baseName = (lastUnderscore >= 0) ? oldName.Substring(0, lastUnderscore) : oldName;

            foreach (var connectedTile in connectedOldTiles)
            {
                // Nếu có neighbor thuộc region
                if (connectedTile != null && region.Tiles.Contains(connectedTile))
                {
                    if (connectedTile == oldTile)
                        continue; // bỏ qua tile cũ
                    
                    if (GetMarkerByTile(connectedTile.Coordinates) is not null)
                        continue; // tile này đã có marker rồi, không thể di chuyển

                    // Cập nhật vị trí và tên mới với tọa độ mới
                    marker.transform.position = connectedTile.transform.position + Vector3.up * 0.2f;
                    marker.name = $"{baseName}_{connectedTile.Coordinates}";
                    marker.GetComponent<AnimalIconMarker>().Tile = connectedTile;

                    Debug.Log($"Di chuyển marker từ {oldTile.Coordinates} sang {connectedTile.Coordinates} trong region {region.Id}.");
                    break;
                }
            }
        }
    }
}
