using MachineRepair.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MachineRepair.Grid.GridManager;


namespace MachineRepair.Grid
{
    public class GridManager : MonoBehaviour
    {
        [Header("Map Size")]
        public int width = 64;
        public int height = 48;

        [Header("Cells")]
        private cellDef nullCell;
        private cellDef normalCell;
        private cellDef connectorCell;
        private cellDef displayCell;
        public cellDef[] cellByIndex;
        public cellDef[] cellSubGrid;


        [Header("Spills (0..n liters depth proxy)")]
        private float[] spillByIndex;

        [Header("Overlays")]
        private bool[] powerByIndex; // true if this cell has power service
        private bool[] waterByIndex; // true if this cell has water service

        [Header("Things")]
        private List<ThingDef>[] bucketByIndex;  // 0..many per cell

        [Header("Tilemap to query")]
        [SerializeField] private Tilemap tilemap;

        public int CellCount => width * height;
        public bool setup = false;

        private void Start()
        {
            
        }
        void Awake()
        {
            cellDefByType();
            InitGrids();
            setup = true;
            
        }

        #region API: GRID
        // --- Grid Setup
        public void InitGrids()
        {
            int n = CellCount;
            cellByIndex = new cellDef [n];

            for (int i = 0; i < cellByIndex.Length; i++)
            {
                cellByIndex[i] = new cellDef();

                //Set all cells to empty to start
                cellByIndex[i].pipe = false;
                cellByIndex[i].component = ComponentType.None;
                cellByIndex[i].wire = WireType.None;


                var (x, y) = FromIndex(i);
                Vector3Int cellPos = new Vector3Int(x, y, 0);  // z=0 for tilemap

                // grab the tile (TileBase covers Tile/RuleTile/etc.)
                TileBase t = tilemap.GetTile(cellPos);
               
                if (t != null)
                {
                    string tileName = t.name;
                        Debug.Log($"Comparing titleName: {tileName} to a name value ");
                        cellByIndex[i] = tileName switch
                        {
                            var s when s == "normalCell" => normalCell,
                            var s when s == "connectorCell" => connectorCell,
                            var s when s == "displayCell" => displayCell,
                            _ => nullCell
                        }; Debug.Log(cellByIndex[i].placeability);
                }
                else Debug.Log("placeabiliy defaulted");
            }
            UpdateSubGrid();
        }

        public void UpdateSubGrid()
        {
            int n = width;
            cellSubGrid = new cellDef [n];

            for (int i = 0; i < cellSubGrid.Length; i++)
            {
                Vector2Int m = new Vector2Int(i, 0);
                cellSubGrid[i] = new cellDef();
                cellSubGrid[i] = cellByIndex[(ToIndex(m))];

                Vector3Int cellPos = new Vector3Int(i, 0, 0);
                TileBase t = tilemap.GetTile(cellPos);
                if (t != null)
                {
                    Vector3Int subcellPos = new Vector3Int(i, -50, 0);
                    tilemap.SetTile(subcellPos, t);
                }
            }


        }

        private void cellDefByType()
        {
            nullCell.placeability = CellPlaceability.Blocked;
            normalCell.placeability = CellPlaceability.Placeable;
            connectorCell.placeability = CellPlaceability.ConnectorsOnly;
            displayCell.placeability = CellPlaceability.Display;
        }



        // --- Bounds & Indexing ---
        public bool InBounds(int x, int y) => (uint)x < (uint)width && (uint)y < (uint)height;
        public int ToIndex(Vector2Int c) => CellIndex.ToIndex(c.x, c.y, width);
        public (int x, int y) FromIndex(int index) => CellIndex.FromIndex(index, width);

        // --- Cells ---
        public cellDef GetCell(Vector2Int c) => cellByIndex[ToIndex(c)];
        public void SetCell(Vector2Int c, cellDef f) => cellByIndex[ToIndex(c)] = f;

        public Vector3 CellToWorld(Vector2Int c) //provide cells positioning to the game space
        {
            return new Vector3(c.x + 0.5f, c.y + 0.5f, 0f);
        }
        public Vector2Int WorldToCell(Vector3 worldPos) //provide cell from game space position
        {
            int x = Mathf.FloorToInt(worldPos.x);
            int z = Mathf.FloorToInt(worldPos.y);
            return new Vector2Int(x, z);
        }
        public bool TryGetCell(Vector2Int c, out cellDef cell) //checking if cell is real and in bounds
        {
            cell = default;
            if (!InBounds(c.x, c.y)) return false;
            cell = GetCell(c);
            return true;
        }

        // --- Spills ---
        public float GetSpill(Vector2Int c) => spillByIndex[ToIndex(c)];
        public void AddSpill(Vector2Int c, float amount)
        {
            int i = ToIndex(c);
            spillByIndex[i] = Mathf.Max(0f, spillByIndex[i] + amount);
        }
        public void SetSpill(Vector2Int c, float value)
        {
            spillByIndex[ToIndex(c)] = Mathf.Max(0f, value);
        }

        // --- Overlays ---
        public bool HasPower(Vector2Int c) => powerByIndex[ToIndex(c)];
        public bool HasWater(Vector2Int c) => waterByIndex[ToIndex(c)];
        public void SetPower(Vector2Int c, bool on) => powerByIndex[ToIndex(c)] = on;
        public void SetWater(Vector2Int c, bool on) => waterByIndex[ToIndex(c)] = on;

        // --- Things ---
        public IReadOnlyList<ThingDef> ThingsAt(Vector2Int c) => bucketByIndex[ToIndex(c)];
        //public Machine EdificeAt(Vector2Int c) => edificeByIndex[ToIndex(c)];

        /*
        public void AddThing(ThingDef t, Vector2Int c)
        {
            if (!InBounds(c.x, c.y)) return;
            int i = ToIndex(c);
            bucketByIndex[i].Add(t);
            t.cell = c;
        }
        

        public void RemoveThing(ThingDef t)
        {
            int i = ToIndex(t.cell);
            bucketByIndex[i].Remove(t);
        }
         */

        // --- Queries ---
        public bool IsPlaceable(Vector2Int c)
        {
            if (!InBounds(c.x, c.y)) return false;
            var cell = GetCell(c);
            if (cell.placeability == CellPlaceability.Blocked) return false;

            return true;
        }

        public int GetFillState(Vector2Int c)
        {
            if (!InBounds(c.x, c.y)) return 0;
            var cell = GetCell(c);
            if (cell.placeability == CellPlaceability.Blocked) return 0;

            return (int)cell.placeability;
        }

        /// <summary>
        /// Creates sprite-based highlights for every occupied cell. Cells with components,
        /// wires, or pipes will receive a SpriteRenderer colored by the contents. Returns the
        /// created renderers so callers can manage their lifecycle (e.g., destroy or pool).
        /// </summary>
        /// <param name="highlightSprite">Sprite to render for each occupied cell.</param>
        /// <param name="parent">Transform used as the parent for all created highlights.</param>
        /// <param name="componentColor">Tint for cells containing a component.</param>
        /// <param name="wireColor">Tint for cells containing only wire.</param>
        /// <param name="pipeColor">Tint for cells containing only pipe.</param>
        /// <param name="mixedColor">Tint when multiple content types share the cell.</param>
        /// <param name="sortingLayer">Sorting layer used for the highlight renderers.</param>
        /// <param name="sortingOrder">Sorting order used for the highlight renderers.</param>
        public List<SpriteRenderer> CreateOccupancyHighlights(
            Sprite highlightSprite,
            Transform parent,
            Color componentColor,
            Color wireColor,
            Color pipeColor,
            Color mixedColor,
            string sortingLayer = "Default",
            int sortingOrder = 0)
        {
            var highlights = new List<SpriteRenderer>();
            if (highlightSprite == null || parent == null) return highlights;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = new Vector2Int(x, y);
                    var cell = GetCell(c);

                    bool hasComponent = cell.HasComponent;
                    bool hasWire = cell.HasWire;
                    bool hasPipe = cell.HasPipe;

                    if (!hasComponent && !hasWire && !hasPipe) continue;

                    Color color = hasComponent && hasWire || hasComponent && hasPipe || hasWire && hasPipe
                        ? mixedColor
                        : hasComponent ? componentColor
                        : hasWire ? wireColor
                        : pipeColor;

                    var go = new GameObject($"occupancyHighlight_{x}_{y}");
                    go.transform.SetParent(parent, worldPositionStays: false);
                    go.transform.position = CellToWorld(c);

                    var renderer = go.AddComponent<SpriteRenderer>();
                    renderer.sprite = highlightSprite;
                    renderer.color = color;
                    renderer.sortingLayerName = sortingLayer;
                    renderer.sortingOrder = sortingOrder;
                    highlights.Add(renderer);
                }
            }

            return highlights;
        }

        /*
        public bool IsWalkable(Vector2Int c) {
            if (!InBounds(c.x, c.z)) return false;
            var e = EdificeAt(c);
            if (e != null && e.def != null && e.def.passability == Passability.Impassable) return false;
            return true;
        }
        */

      
        #endregion

        
    }
}
        


