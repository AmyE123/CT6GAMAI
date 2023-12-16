namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the selection and interaction of nodes within the grid.
    /// This includes handling node selection, grid navigation, unit selection, and pathing.
    /// </summary>
    public class GridCursor : MonoBehaviour
    {
        [SerializeField] private AudioSource _cursorAudioSource;
        [SerializeField] private CursorAudioClips _cursorAudioClips;

        private GameManager _gameManager;
        private GridManager _gridManager;
        private AudioManager _audioManager;
        private UnitManager _lastSelectedUnit;
        private bool _pathing = false;

        /// <summary>
        /// The currently selected node.
        /// </summary>
        public NodeManager SelectedNode;

        /// <summary>
        /// The state of the currently selected node.
        /// </summary>
        public NodeState SelectedNodeState;

        /// <summary>
        /// Indicates whether a unit is currently pressed (selected).
        /// </summary>
        public bool UnitPressed = false;

        /// <summary>
        /// Indicates whether pathing mode is active.
        /// </summary>
        public bool Pathing => _pathing;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;
            _audioManager = _gameManager.AudioManager;

            SelectedNode.NodeState.CursorStateManager.SetDefaultSelected();
        }

        private void Update()
        {
            _pathing = UnitPressed;

            UpdateSelectedNode();
            HandleNodeUnitInteraction();
            HandleGridNavigation();
            HandleUnitSelection();
            HandleUnitPathing();
        }

        private void UpdateUnitReferences()
        {
            var unitManager = _gameManager.UnitsManager;
            _lastSelectedUnit = unitManager.ActiveUnit;
        }

        private void UpdateTerrainTypeUI()
        {
            _gameManager.UIManager.TileInfoManager.SetTerrainType(SelectedNode.NodeData.TerrainType);
        }

        private void UpdateSelectedNode()
        {
            if (SelectedNodeState == null)
            {
                SelectedNodeState = SelectedNode.NodeState;
            }

            if (SelectedNode == null || !SelectedNodeState.CursorStateManager.IsActiveSelection)
            {
                UpdateActiveNodeSelection();
            }
        }

        private void UpdateActiveNodeSelection()
        {
            var nodes = _gridManager.AllNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeState.CursorStateManager.IsActiveSelection)
                {
                    SelectedNode = nodes[i];
                    SelectedNodeState = SelectedNode.NodeState;
                }
            }
        }

        private void HandleNodeUnitInteraction()
        {
            _gameManager.UnitsManager.SetCursorUnit(SelectedNode.StoodUnit);

            if (SelectedNode.StoodUnit != null)
            {
                _gameManager.UIManager.UnitInfoManager.SetUnitType(SelectedNode.StoodUnit.UnitData);

                if (!_pathing || _gameManager.UnitsManager.ActiveUnit == null)
                {
                    _gameManager.UnitsManager.SetActiveUnit(SelectedNode.StoodUnit);

                    ResetHighlightedNodes();
                    SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.VisualStateManager.IsPressed);
                }

                if (_pathing)
                {
                    if (_gameManager.UnitsManager.CursorUnit.UnitData.UnitTeam == Team.Enemy)
                    {
                        Debug.Log("[GAME]: UI Here for 'Enemy selected!'");
                        SelectedNode.NodeState.CursorStateManager.SetEnemySelected();
                    }
                    else if (_gameManager.UnitsManager.CursorUnit.UnitData.UnitTeam == Team.Player)
                    {
                        Debug.Log("[GAME]: UI Here for 'Player selected!'");
                        SelectedNode.NodeState.CursorStateManager.SetPlayerSelected();
                    }
                    else
                    {
                        SelectedNode.NodeState.CursorStateManager.SetDefaultSelected();
                    }
                }
            }
            else
            {
                _gameManager.UIManager.UnitInfoManager.SetUnitInfoUIInactive();

                if (!UnitPressed)
                {
                    _gameManager.UnitsManager.SetActiveUnit(null);
                    ResetHighlightedNodes();
                }
            }
        }

        private void HandleGridNavigation()
        {
            UpdateUnitReferences();
            UpdateTerrainTypeUI();

            if (!_gameManager.UnitsManager.IsAnyUnitMoving())
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    MoveCursor(Direction.North);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    MoveCursor(Direction.West);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    MoveCursor(Direction.South);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    MoveCursor(Direction.East);
                }
            }
        }

        private void MoveCursor(Direction direction)
        {
            _audioManager.PlayCursorSound(UnitPressed);

            NodeState adjacentNodeState = GetAdjacentNodeState(direction);
            if (adjacentNodeState != null)
            {
                adjacentNodeState.CursorStateManager.SetDefaultSelected();
                SelectedNodeState.CursorStateManager.SetInactive();
            }
        }

        private NodeState GetAdjacentNodeState(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return SelectedNode.NorthNode.NodeState;
                case Direction.South:
                    return SelectedNode.SouthNode.NodeState;
                case Direction.West:
                    return SelectedNode.WestNode.NodeState;
                case Direction.East:
                    return SelectedNode.EastNode.NodeState;
                default:
                    Debug.Assert(false, "Invalid direction passed to GetAdjacentNodeState.");
                    return null;
            }
        }

        private void HandleUnitSelection()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var valid = _pathing && _gameManager.UnitsManager.CursorUnit != null && _gameManager.UnitsManager.CursorUnit != _gameManager.UnitsManager.ActiveUnit;

                if (!valid)
                {
                    ToggleUnitSelection();
                }
            }
        }

        private void ToggleUnitSelection()
        {
            if (SelectedNode.StoodUnit != null)
            {
                UnitPressed = !UnitPressed;               

                _audioManager.PlayToggleUnitSound(UnitPressed);

                UpdateNodeVisualState();
            }
        }

        private void UpdateNodeVisualState()
        {
            if (UnitPressed)
            {
                _gameManager.UnitsManager.ActiveUnit.IsSelected = true;
                SelectedNodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
                }
            }

            if (!UnitPressed)
            {
                _gameManager.UnitsManager.ActiveUnit.IsSelected = false;
                SelectedNodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);
                }
            }
        }

        private void HandleUnitPathing()
        {
            if (!UnitPressed)
            {
                return;
            }

            _gridManager.HandleUnitPathing();
        }

        /// <summary>
        /// Resets the visual state of all highlighted nodes to default.
        /// </summary>
        public void ResetHighlightedNodes()
        {
            var nodes = _gridManager.AllNodes;

            if (_lastSelectedUnit != null)
            {
                _lastSelectedUnit.MovementRange.ResetNodes();
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].NodeState.VisualStateManager.IsActive)
                {
                    nodes[i].NodeState.VisualStateManager.SetDefault();
                }

                SelectedNodeState.VisualStateManager.SetDefault();
            }
        }
    }
}