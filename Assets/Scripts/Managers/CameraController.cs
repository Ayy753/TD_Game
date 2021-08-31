namespace DefaultNamespace {

    using DefaultNamespace.IO;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using Zenject;

    public class CameraController : MonoBehaviour {
        [Inject] TargetManager targetManager;

        private const float panSpeed = 30f;
        private const float scrollSpeed = 5f;
        private const int minZoom = 5;
        private const int maxZoom = 75;

        private float horizontalPan;
        private float verticalPan;

        private Transform focusedTarget;

        private void OnEnable() {
            InputHandler.OnCommandEntered += HandleCommandEntered;
            MouseManager.OnRightMouseUp += HandleRightButtonUp;
        }

        private void OnDisable() {
            InputHandler.OnCommandEntered -= HandleCommandEntered;
            MouseManager.OnRightMouseUp -= HandleRightButtonUp;
        }

        void Update() {
            horizontalPan = Input.GetAxisRaw("Horizontal");
            verticalPan = Input.GetAxisRaw("Vertical");

            //  If user is holding down a pan key
            if (horizontalPan != 0 || verticalPan != 0) {
                if (horizontalPan < 0) {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, panSpeed * Time.unscaledDeltaTime);
                }
                else if (horizontalPan > 0) {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, panSpeed * Time.unscaledDeltaTime);
                }

                if (verticalPan < 0) {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down, panSpeed * Time.unscaledDeltaTime);
                }
                else if (verticalPan > 0) {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up, panSpeed * Time.unscaledDeltaTime);
                }

                //  Clear focus target
                focusedTarget = null;
            }
            //  Otherwise if user is focusing on a target
            else if (focusedTarget != null) {
                //  If focused target becomes inactive, clear it to prevent reused enemies from being focused when respawned
                if (!focusedTarget.gameObject.activeInHierarchy) {
                    focusedTarget = null;
                }
                else {
                    Vector3 position = focusedTarget.position;
                    transform.position = new Vector3(focusedTarget.position.x, focusedTarget.position.y, transform.position.z);
                }
            }

            if (!EventSystem.current.IsPointerOverGameObject() && Input.mouseScrollDelta.y != 0) {
                HandleScroll();
            }
        }

        private void HandleScroll() {
            float newCameraDistance = Mathf.Clamp(Camera.main.orthographicSize + -Input.mouseScrollDelta.y * scrollSpeed, minZoom, maxZoom);
            Camera.main.orthographicSize = newCameraDistance;

            //  We need to move the camera's Z position in order for spacial sound to work properly
            Vector3 cameraPosition = transform.position;
            cameraPosition.z = -newCameraDistance;
            transform.position = cameraPosition;
        }

        /// <summary>
        /// Follows target if one exists
        /// </summary>
        /// <param name="command"></param>
        private void HandleCommandEntered(Command command) {
            if (command == Command.FollowTarget) {
                Itargetable target = targetManager.GetTarget();

                if (target != null) {
                    focusedTarget = target.GetTransform();
                }
            }
        }

        /// <summary>
        /// Clears focused target
        /// </summary>
        private void HandleRightButtonUp() {
            focusedTarget = null;
        }
    }
}
