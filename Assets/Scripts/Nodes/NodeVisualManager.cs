namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.XR;
    using static CT6GAMAI.Constants;

    public class NodeVisualManager : MonoBehaviour
    {
        [SerializeField] private NodeVisualFSM visualFSM;

        public NodeState State;
        public NodeStateVisualData[] VisualDatas;
        public SpriteRenderer VisualSR;

        /// <summary>
        /// A bool indicating whether the node visual is active or not.
        /// This returns true if any of the other bools besides default (Hovered, Pressed, Path) is true.
        /// </summary>
        public bool IsActive => _isActive;

        /// <summary>
        /// A bool indicating whether the visual is in default state. Default = No Visuals.
        /// </summary>
        public bool IsDefault => _isDefault;

        /// <summary>
        /// A bool indicating whether the visual is in a hovered state.
        /// Hovered is when the selection is over a unit and it shows their movement range.
        /// </summary>
        public bool IsHovered => _isHovered;

        /// <summary>
        /// A bool indicating whether the visual is in a pressed state.
        /// Pressed is when a unit has been pressed and it highlights the movement range in bold.
        /// </summary>
        public bool IsPressed => _isPressed;

        /// <summary>
        /// A bool indicating whether the visual is in a path state.
        /// Path is when the unit's path has been selected, and this is a highlight of it.
        /// </summary>
        public bool IsPath => _isPath;

        [SerializeField] private bool _isActive;
        [SerializeField] private bool _isDefault;
        [SerializeField] private bool _isHovered;
        [SerializeField] private bool _isPressed;
        [SerializeField] private bool _isPath;

        private void Start()
        {
            Debug.Log("VisualManager: Start");

            visualFSM = new NodeVisualFSM(gameObject);
            visualFSM.Manager = this;
            visualFSM.Initialize();
        }

        public void SetDefault()
        {
            Debug.Log("VisualManager: SetDefault");

            _isActive = false;
            _isDefault = true;
            _isHovered = false;
            _isPressed = false;
            _isPath = false;

            visualFSM.ChangeState(NodeVisualState.Default);
        }

        public void SetHovered(NodeVisualColorState color)
        {
            Debug.Log("VisualManager: SetHovered");

            _isActive = true;
            _isDefault = false;
            _isHovered = true;
            _isPressed = false;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.HoveredBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.HoveredRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.HoveredGreen);
                    break;
            }
        }

        public void SetPressed(NodeVisualColorState color)
        {
            Debug.Log("VisualManager: SetPressed");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = true;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.SelectedBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.SelectedRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.SelectedGreen);
                    break;
            }
        }

        public void SetEnemyRange(NodeVisualEnemyColorState color)
        {
            Debug.Log("VisualManager: SetEnemyRange");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = true;
            _isPath = false;

            switch (color)
            {
                case NodeVisualEnemyColorState.SingularEnemy:
                    visualFSM.ChangeState(NodeVisualState.AllEnemyRange);
                    break;
                case NodeVisualEnemyColorState.AllEnemy:
                    visualFSM.ChangeState(NodeVisualState.SingularEnemyRange);
                    break;
            }
        }

        public void SetPath()
        {
            Debug.Log("VisualManager: SetPath");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = false;
            _isPath = true;

            visualFSM.ChangeState(NodeVisualState.PointOfInterest);
        }
    }
}