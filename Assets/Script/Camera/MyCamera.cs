using Assets.Script.Extension;
using Assets.Script.HUD;
using Assets.Script.InventoryFolder.CraftFolder;
using Assets.Script.Menu;
using Assets.Script.QuestFolder;
using Assets.Script.SpellFolder;
using Assets.Scripts.InventoryFolder.CraftFolder;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Script.Camera
{
    public class MyCamera : MonoBehaviour
    {
        private const float OFFSET_FROM_WALL = 0.1f;                       // Bring camera away from any colliding objects
        private const float MAX_DISTANCE = 7f;                       // Maximum zoom Distance
        private const float MIN_DISTANCE = 0.6f;                      // Minimum zoom Distance
        private const float X_ORBIT_SPEED = 200.0f;                             // Orbit speed (Left/Right)
        private const float Y_ORBIT_SPEED = 200.0f;                             // Orbit speed (Up/Down)
        private const float Y_MIN_LOOK = -80f;                            // Looking up limit
        private const float Y_MAX_LOOK = 80f;                             // Looking down limit
        private const float ZOOMRATE = 40f;                          // Zoom Speed
        private const float ROTATION_DAMPENING = 3.0f;                // Auto Rotation speed (higher = faster)
        private const float ZOOM_DAMPENING = 5.0f;                    // Auto Zoom speed (Higher = faster)
        private const float TARGET_HEIGHT = 1.7f;                         // Vertical offset adjustment

        private GameObject target;                           // Target to follow
        private float distance = 7.0f;                            // Default Distance      
        private LayerMask collisionLayers = -1;     // What the camera will collide with
        private bool lockToRearOfTarget = false;             // Lock camera to rear of target
        private bool allowMouseInputX = true;                // Allow player to control camera angle on the X axis (Left/Right)
        private bool allowMouseInputY = true;                // Allow player to control camera angle on the Y axis (Up/Down)
        private float xDeg = 0.0f;
        private float yDeg = 0.0f;
        private float currentDistance;
        private float desiredDistance;
        private float correctedDistance;
        private bool rotateBehind = false;
        private bool mouseSideButton = false;
        private float pbuffer = 0.0f;       //Cooldownpuffer for SideButtons
        private float coolDown = 0.5f;      //Cooldowntime for SideButtons 

        public static bool Enabled;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            xDeg = angles.x;
            yDeg = angles.y;
            currentDistance = distance;
            desiredDistance = distance;
            correctedDistance = distance;
            if (lockToRearOfTarget)
                rotateBehind = true;
            Enabled = true;
        }

        void Update()
        {
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player");
            }

        }

        //Only Move camera after everything else has been updated
        void LateUpdate()
        {
            if (ComponentCraftMenu.Visible)
                return;
            // Don't do anything if target is not defined
            if (target == null)
                return;
            //pushbuffer
            if (pbuffer > 0)
                pbuffer -= Time.deltaTime;
            if (pbuffer < 0) pbuffer = 0;

            //Sidebuttonmovement
            if ((Input.GetAxis("Toggle Move") != 0) && (pbuffer == 0))
            {
                pbuffer = coolDown;
                mouseSideButton = !mouseSideButton;
            }
            if (mouseSideButton && Input.GetAxis("Vertical") != 0)
                mouseSideButton = false;

            Vector3 vTargetOffset;

            // If either mouse buttons are down, let the mouse govern camera position
            if (GUIUtility.hotControl == 0)
            {
                if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && Enabled && !DragAndDropWindow.WindowDrag && SpellDragAndDrop.SpellDraged == null && !EventSystem.current.IsPointerOverGameObject() && InventoryMouseHandler.itemBeingDragged == null)
                {
                    //Check to see if mouse input is allowed on the axis
                    if (allowMouseInputX)
                        xDeg += Input.GetAxis("Mouse X") * X_ORBIT_SPEED * 0.02f;
                    else
                        RotateBehindTarget();
                    if (allowMouseInputY)
                        yDeg -= Input.GetAxis("Mouse Y") * Y_ORBIT_SPEED * 0.02f;

                    //Interrupt rotating behind if mouse wants to control rotation
                    /* if (!lockToRearOfTarget)
                         rotateBehind = false;*/
                }
                // otherwise, ease behind the target if any of the directional keys are pressed
                else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || rotateBehind || mouseSideButton)
                {
                    RotateBehindTarget();
                }
            }
            yDeg = ClampAngle(yDeg, Y_MIN_LOOK, Y_MAX_LOOK);


            // Set camera rotation
            Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);
            // Calculate the desired distance
            //Debug.Log(Utilities.IsRayCastHit(GameObject.Find("Terrain").transform,"Player","SalesMan","ThisQuestMaster","Enemy"));
            if (!EventSystem.current.IsPointerOverGameObject())
                desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZOOMRATE * Mathf.Abs(desiredDistance);
            desiredDistance = Mathf.Clamp(desiredDistance, MIN_DISTANCE, MAX_DISTANCE);
            correctedDistance = desiredDistance;

            // Calculate desired camera position
            vTargetOffset = new Vector3(0, -TARGET_HEIGHT, 0);
            Vector3 position = target.transform.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

            // Check for collision using the true target's desired registration point as set by user using height
            RaycastHit collisionHit;
            Vector3 trueTargetPosition = new Vector3(target.transform.position.x, target.transform.position.y + TARGET_HEIGHT, target.transform.position.z);

            // If there was a collision, correct the camera position and calculate the corrected distance
            var isCorrected = false;
            if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers))
            {
                // Calculate the distance from the original estimated position to the collision location,
                // subtracting out a safety "offset" distance from the object we hit.  The offset will help
                // keep the camera from being right on top of the surface we hit, which usually shows up as
                // the surface geometry getting partially clipped by the camera's front clipping plane.
                if (collisionHit.transform.tag != "Respawn")
                {
                    correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - OFFSET_FROM_WALL;
                    isCorrected = true;
                }
            }

            // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
            currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * ZOOM_DAMPENING) : correctedDistance;

            // Keep within limits
            currentDistance = Mathf.Clamp(currentDistance, MIN_DISTANCE, MAX_DISTANCE);

            // Recalculate position based on the new currentDistance
            position = target.transform.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

            //Finally Set rotation and position of camera
            transform.rotation = rotation;
            transform.position = position;
        }

        private void RotateBehindTarget()
        {
            float targetRotationAngle = target.transform.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, ROTATION_DAMPENING * Time.deltaTime);

            // Stop rotating behind if not completed
            if (targetRotationAngle == currentRotationAngle)
            {
                if (!lockToRearOfTarget)
                    rotateBehind = false;
            }
            else
                rotateBehind = true;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f)
                angle += 360f;
            if (angle > 360f)
                angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }
}