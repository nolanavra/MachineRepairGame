using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;     // For UI-hit checks
using UnityEngine.InputSystem;      // New Input System

/// Centralizes input handling, keys, mouseclick  (left/right) and routes to per-mode handlers.
/// Uses GameModeManager + GridManager. Uses New Input System (Mouse.current).
namespace MachineRepair.Grid
{
    public class InputRouter : MonoBehaviour, IGameModeListener
    {
        [Header("References")]
        [Tooltip("Auto-found at runtime if left unassigned.")]
        [SerializeField] private GridManager grid;
        [SerializeField] private GameObject currentComponentPrefab;
        private Camera cam;

        [Header("Behavior")]
        [Tooltip("Ignore clicks when the pointer is over UI (recommended).")]
        [SerializeField] private bool blockWhenPointerOverUI = true;

        [Header("Cell Highlighter")]
        [SerializeField] private Sprite highlightSprite;
        [Tooltip("Enable)")]
        [SerializeField] private bool highlightEnable = true;
        [Tooltip("Tint (alpha controls transparency).")]
        [SerializeField] private Color highlightTint = new Color(1f, 1f, 0f, 0.25f); // soft yellow, 25% alpha
        [Tooltip("Optional Scaling (1,1 fits a 1x1 cell).")]
        [SerializeField] private Vector2 highlightScale = new Vector2(1f, 1f);
        [SerializeField] private string highlightSortingLayer = "Default";
        [SerializeField] private int highlightSortingOrder = 1000;

        private GameObject highlightObject;
        private SpriteRenderer highlightRenderer;
        private Vector2Int highlightLastPosition;
        

        private void Awake()
        {
            cam = Camera.main;
            if (grid == null) grid = Object.FindFirstObjectByType<GridManager>();
            SetupHighlightVisual();
        }

        private void OnEnable()
        {
            if (GameModeManager.Instance != null)
                GameModeManager.Instance.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (GameModeManager.Instance != null)
                GameModeManager.Instance.UnregisterListener(this);
        }

        private void Update()
        {
            var mouse = Mouse.current;
            if (mouse == null || cam == null || grid == null) return;

            if (blockWhenPointerOverUI && IsPointerOverUI()) return;

            if(highlightEnable)UpdateCellHighlight();

            // LEFT CLICK
            if (mouse.leftButton.wasPressedThisFrame)
            {
                cellDef cell = GetMouseCell();
                Vector2Int pos = GetMousePos();
                if (grid.InBounds(pos.x, pos.y))
                    RouteLeftClick(cell, pos);
            }

            // RIGHT CLICK
            if (mouse.rightButton.wasPressedThisFrame)
            {
                cellDef cell = GetMouseCell();
                Vector2Int pos = GetMousePos();
                if (grid.InBounds(pos.x, pos.y))
                    RouteRightClick(cell, pos);
            }
        }

       
        // -------------- Core Routing ----------------

        private void RouteLeftClick(cellDef cell, Vector2Int cellPos)
        {
            /*
            switch (GameModeManager.Instance.CurrentMode)
            {
                case GameMode.Selection:
                    OnLeftClick_Selection(cell, cellPos);
                    break;

                case GameMode.ComponentPlacement:
                    OnLeftClick_ComponentPlacement(cell, cellPos);
                    break;

                case GameMode.WirePlacement:
                    OnLeftClick_WirePlacement(cell, cellPos);
                    break;

                case GameMode.PipePlacement:
                    OnLeftClick_PipePlacement(cell, cellPos);
                    break;

                case GameMode.Simulation:
                    OnLeftClick_Simulation(cell, cellPos);
                    break;
            }
            */
            
        }

        private void RouteRightClick(cellDef cell, Vector2Int cellPos)
        {
            /*
            switch (GameModeManager.Instance.CurrentMode)
            {
                case GameMode.Selection:
                    OnRightClick_Selection(cell, cellPos);
                    break;

                case GameMode.ComponentPlacement:
                    OnRightClick_ComponentPlacement(cell, cellPos);
                    break;

                case GameMode.WirePlacement:
                    OnRightClick_WirePlacement(cell, cellPos);
                    break;

                case GameMode.PipePlacement:
                    OnRightClick_PipePlacement(cell, cellPos);
                    break;

                case GameMode.Simulation:
                    OnRightClick_Simulation(cell, cellPos);
                    break;
            }
            */
            
        }

        
        // -------------- Per-Mode Handlers --------------
        // Put your actual calls where the comments indicate.

        #region Selection

        /// <summary>
        /// LEFT CLICK in Selection: pick/select an object in the cell.
        /// CALL: SelectionSystem.SelectAt(cellPos) or Raycast for entity under cursor.
        /// </summary>
        private void OnLeftClick_Selection(cellDef cell, Vector2Int cellPos)
        {
            if (!CellUsable(cell)) return;
            //SelectionManager.Instance.SelectCell(cellPos);
            //Debug.Log($"[Selection] Select at {cellPos}");
        }

        /// <summary>
        /// RIGHT CLICK in Selection: context or move command.
        /// CALL: SelectionSystem.ContextMenu(cellPos) or IssueMoveCommand().
        /// </summary>
        private void OnRightClick_Selection(cellDef cell, Vector2Int cellPos)
        {
            if (!CellUsable(cell)) return;
            //SelectionManager.Instance.ClearSelection();
            //Debug.Log($"[Selection] Context/Move at {cellPos}");
        }

        #endregion

        #region Component Placement

        /// <summary>
        /// LEFT CLICK in ComponentPlacement: place a component if the cell is enabled and not occupied.
        /// CALL: BuildSystem.PlaceComponentAt(cellPos);
        /// </summary>
        private void OnLeftClick_ComponentPlacement(cellDef cell, Vector2Int cellPos)
        {
          
        }

        /// <summary>
        /// RIGHT CLICK in ComponentPlacement: rotate/cancel current ghost.
        /// CALL: BuildSystem.RotateCurrentGhost() or BuildSystem.CancelPlacement();
        /// </summary>
        private void OnRightClick_ComponentPlacement(cellDef cell, Vector2Int cellPos)
        {
            // TODO: Replace with your rotate/cancel logic.
            // BuildSystem.RotateCurrentGhost();
            Debug.Log($"[ComponentPlacement] Rotate/Cancel at {cellPos}");
        }

        #endregion

        #region Wire Placement

        /// <summary>
        /// LEFT CLICK in WirePlacement: start/continue wire at this cell.
        /// CALL: WireTool.AddPoint(cellPos) or WireTool.StartAt(cellPos) if not started.
        /// </summary>
        private void OnLeftClick_WirePlacement(cellDef cell, Vector2Int cellPos)
        {
            if (!CellUsable(cell)) return;

            // TODO: Replace with your wire placement logic.
            // if (!WireTool.HasActivePath) WireTool.StartAt(cellPos); else WireTool.AddPoint(cellPos);
            Debug.Log($"[WirePlacement] Point at {cellPos}");
        }

        /// <summary>
        /// RIGHT CLICK in WirePlacement: undo last point or cancel path.
        /// CALL: WireTool.UndoLastPoint() or WireTool.CancelPath();
        /// </summary>
        private void OnRightClick_WirePlacement(cellDef cell, Vector2Int cellPos)
        {
            // TODO: Replace with your wire cancel/undo logic.
            // WireTool.UndoLastPoint();
            Debug.Log($"[WirePlacement] Undo/Cancel at {cellPos}");
        }

        #endregion

        #region Pipe Placement

        /// <summary>
        /// LEFT CLICK in PipePlacement: start/continue pipe run.
        /// CALL: PipeTool.AddPoint(cellPos) or PipeTool.StartAt(cellPos).
        /// </summary>
        private void OnLeftClick_PipePlacement(cellDef cell, Vector2Int cellPos)
        {
            if (!CellUsable(cell)) return;

            // TODO: Replace with your pipe placement logic.
            // if (!PipeTool.HasActiveRun) PipeTool.StartAt(cellPos); else PipeTool.AddPoint(cellPos);
            Debug.Log($"[PipePlacement] Point at {cellPos}");
        }

        /// <summary>
        /// RIGHT CLICK in PipePlacement: undo last point or cancel run.
        /// CALL: PipeTool.UndoLastPoint() or PipeTool.CancelRun();
        /// </summary>
        private void OnRightClick_PipePlacement(cellDef cell, Vector2Int cellPos)
        {
            // TODO: Replace with your pipe cancel/undo logic.
            // PipeTool.UndoLastPoint();
            Debug.Log($"[PipePlacement] Undo/Cancel at {cellPos}");
        }

        #endregion

        #region Simulation

        /// <summary>
        /// LEFT CLICK in Simulation: probe/inspect.
        /// CALL: SimSystem.InspectCell(cellPos) or open inspector panel.
        /// </summary>
        private void OnLeftClick_Simulation(cellDef cell, Vector2Int cellPos)
        {
            // Probing usually allowed on disabled cells too; gate if you want:
            // if (!CellUsable(cell)) return;

            // TODO: Replace with your simulation inspect call.
            // SimSystem.InspectCell(cellPos);
            Debug.Log($"[Simulation] Inspect {cellPos}");
        }

        /// <summary>
        /// RIGHT CLICK in Simulation: set debug marker or clear overlays.
        /// CALL: SimSystem.ToggleMarker(cellPos) or SimOverlay.Clear();
        /// </summary>
        private void OnRightClick_Simulation(cellDef cell, Vector2Int cellPos)
        {
            // TODO: Replace with your simulation right-click action.
            // SimOverlay.ToggleMarker(cellPos);
            Debug.Log($"[Simulation] Marker/Action at {cellPos}");
        }

        #endregion

        // -------------- Helpers ----------------

        /// <summary>
        /// True if the cell exists, is enabled, and usable for build actions.
        /// Modify if you want wires/pipes allowed on occupied cells, etc.
        /// </summary>
        private static bool CellUsable(cellDef cell)
        {
            return true; // add more rules if needed
        }

        /// <summary>
        /// UI block helper for New/Old input systems.
        /// </summary>
        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;
            // Works with mouse (-1) in standalone; for advanced setups, use PointerEventData.
            return EventSystem.current.IsPointerOverGameObject();
        }

        // ----------------- Mouse helpers -----------------

        // Returns mouse position on grid
        public Vector2Int GetMousePos()
        {
            Camera cam = Camera.main;
            if (cam == null) return default;

            // With the new Input System, Mouse.current can be null (e.g., no mouse on device)
            var mouse = Mouse.current;
            if (mouse == null) return default;

            Vector2 screenPos = mouse.position.ReadValue();
            Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            mouseWorld.z = 0f;

            int x = Mathf.FloorToInt(mouseWorld.x);
            int y = Mathf.FloorToInt(mouseWorld.y);

            return new Vector2Int(x, y);
        }

        // Find the cell that the mouse is over
        public cellDef GetMouseCell()
        {
            cellDef foundCell;
            Vector2Int cellPos = GetMousePos();
            if (grid.TryGetCell(cellPos, out foundCell))
                return grid.GetCell(cellPos);
            else return default;
        }

#region Highlights
        /// Creates (once) and configures the hover visual GameObject.
        private void SetupHighlightVisual()
        {
            if (highlightObject != null && highlightRenderer != null)
            {
                // Keep color/scale/sorting in sync if tweaked at runtime
                highlightRenderer.color = highlightTint;
                highlightObject.transform.localScale = new Vector3(highlightScale.x, highlightScale.y, 1f);
                highlightRenderer.sortingLayerName = highlightSortingLayer;
                highlightRenderer.sortingOrder = highlightSortingOrder;
                return;
            }

            if (highlightObject == null)
            {
                highlightObject = new GameObject("cellHighlight");
                highlightObject.transform.SetParent(transform, worldPositionStays: true);
                highlightObject.SetActive(highlightEnable);
            }

            highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
            if (highlightRenderer == null)
                highlightRenderer = highlightObject.AddComponent<SpriteRenderer>();

            highlightRenderer.sprite = highlightSprite; // may be null; user should assign a sprite
            highlightRenderer.color = highlightTint;
            highlightObject.transform.localScale = new Vector3(highlightScale.x, highlightScale.y, 1f);
            highlightRenderer.sortingLayerName = highlightSortingLayer;
            highlightRenderer.sortingOrder = highlightSortingOrder;

            // Optional: prevent the highlight from blocking raycasts/clicks (if you use 2D colliders)
            highlightRenderer.maskInteraction = SpriteMaskInteraction.None;
        }

        private void UpdateCellHighlight()
        {
            if (!highlightEnable)
            {
                if (highlightObject != null) highlightObject.SetActive(false);
                return;
            }

            //SetupHighlightVisual();

            // Get mouse cell and validate
            Vector2Int pos = GetMousePos();

            cellDef cell = default;

            if (grid.InBounds(pos.x, pos.y))
            {
                cell = grid.GetCell(pos);

                if (!highlightObject.activeSelf && cell.placeability != CellPlaceability.Blocked) highlightObject.SetActive(true);
                else if(cell.placeability == CellPlaceability.Blocked )highlightObject.SetActive(false);
                if (pos != highlightLastPosition)
                {

                    // Center of the cell (cell size 1)
                    Vector3 center = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
                    highlightObject.transform.position = center;
                    highlightLastPosition = pos;
                }
            }else highlightObject.SetActive(false);
        }

#endregion

        // -------------- IGameModeListener ----------------

        public void OnEnterMode(GameMode newMode)
        {
            // Optional: per-mode cursor changes, enabling tool GameObjects, etc.
            // Example:
            // CursorManager.SetCursorForMode(newMode);
        }

        public void OnExitMode(GameMode oldMode)
        {
            // Optional: cleanup when leaving a mode (e.g., cancel wire run)
            // Example:
            // if (oldMode == GameMode.WirePlacement) WireTool.CancelIfIncomplete();
        }

    }
}
