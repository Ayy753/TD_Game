namespace DefaultNamespace {

    using DefaultNamespace.StatusSystem;
    using DefaultNamespace.TilemapSystem;
    using UnityEngine;

    public class UnitMovement : MonoBehaviour, IUnitMovement {
        IUnitInput unitInput;
        IMapManager mapManager;
        private static readonly Vector3 TILEMAP_OFFSET = new Vector3(0.5f, 0.5f, 0);
        private Vector3Int lastTile;
        private Vector3Int nextTile;
        Vector3 nextTileWithOffset;
        private Status unitStatus;

        private const int maxTileCost = 15;
        private float cachedUnitSpeed, effectiveSpeed;

        private void Awake() {
            unitInput = transform.GetComponent<IUnitInput>();
            mapManager = GameObject.Find("MapManager").GetComponent<IMapManager>();
            unitStatus = transform.GetComponent<IUnit>().GetStatus();
        }

        private void OnEnable() {
            unitStatus.OnStatusChanged += HandleStatChanged;
            lastTile = Vector3Int.FloorToInt(transform.position);
            nextTile = unitInput.GetNextTile();
            nextTileWithOffset = nextTile + TILEMAP_OFFSET;
            transform.parent.position = nextTile + TILEMAP_OFFSET;
            cachedUnitSpeed = unitStatus.Speed.Value;
            CalculateEffectiveSpeed();
        }

        private void OnDisable() {
            unitStatus.OnStatusChanged -= HandleStatChanged;
        }

        private void Update() {
            Move();
        }

        /// <summary>
        /// Faces next path node
        /// </summary>
        private void FaceNextNode(Vector3Int nextNodePos) {
            //  Rotate unit to face direction of next tile in path
            Vector3 posNoOffset = transform.position - TILEMAP_OFFSET;
            if (nextNodePos.x < posNoOffset.x) {
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else if (nextNodePos.y < posNoOffset.y) {
                transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else if (nextNodePos.y > posNoOffset.y) {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            if (nextNodePos.x > posNoOffset.x) {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        /// <summary>
        /// Calculates and sets the unit's efffective speed based on speed stat and tile cost
        /// </summary>
        private void CalculateEffectiveSpeed() {
            float tileCost = mapManager.GetTileCost(nextTile);
            float tileCostMultiplier = (maxTileCost - tileCost) / maxTileCost;
            effectiveSpeed = cachedUnitSpeed * tileCostMultiplier;
        }

        /// <summary>
        /// Recalculates effective speed if unit speed stat changes
        /// </summary>
        /// <param name="statType"></param>
        private void HandleStatChanged(StatType statType, float amount) {
            if (statType == StatType.Speed) {
                cachedUnitSpeed = unitStatus.Speed.Value;
                CalculateEffectiveSpeed();
            }
        }

        public void Move() {

            //  Check if unit entered a new tile
            Vector3Int thisTile = Vector3Int.FloorToInt(transform.position);
            if (thisTile != lastTile) {
                lastTile = thisTile;
                CalculateEffectiveSpeed();
            }

            if (transform.parent.position != nextTileWithOffset) {
                transform.parent.position = Vector3.MoveTowards(transform.parent.position, nextTileWithOffset, effectiveSpeed * Time.deltaTime);
            }
            else {
                unitInput.ReachedNextTile();
                nextTile = unitInput.GetNextTile();
                nextTileWithOffset = nextTile + TILEMAP_OFFSET;
            }

            FaceNextNode(nextTile);
        }
    }
}
