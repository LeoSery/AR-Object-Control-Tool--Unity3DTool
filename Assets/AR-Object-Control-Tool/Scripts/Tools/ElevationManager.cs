/////////////////////////////////////////////////////////////////////////
// AR-Object-Control-Tool -- ElevationManager
// Author: Léo Séry
// Date created: 30/12/2022
// Last updated: 16/02/2023
// Purpose: Allow an object to levitate to a target point and play a floating animation.
// Documentation: https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool
/////////////////////////////////////////////////////////////////////////

using UnityEngine.UI;
using UnityEngine;

public class ElevationManager : MonoBehaviour
{
    #region Fields
    public bool isItLevitate = false;
    public float groundPosition;

    public GameObject objectContainer;
    public GameObject objectToAffect;
    public Animator objectAnimator;

    public bool playAnimation;
    [Range(0.1f, 30f)]
    public float objectFloatHeight;
    [Range(300f, 1f)]
    public float Speed;

    public bool objectDefinesHeightGround;
    public GameObject heightReferenceObject;
    public GameObject stopButton;

    public MovementManager movementManager;
    public bool useExternalScript = false;
    public bool useMovementManager = false;
    public bool useVuforia = false;
    public bool useButton = false;

    private Vector3 Velocity = Vector3.zero;
    public Vector3 Target;
    public float saveGroundPos;
    #endregion

    #region UnityFunctions
    void Awake()
    {
        SetUpManager();
    }

    void Start()
    {
        Target = new Vector3(objectContainer.transform.position.x, saveGroundPos, objectContainer.transform.position.z);
    }

    void Update()
    {
        if (objectContainer != null)
            if (Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Mouse0))
                if (IsOnObject())
                    if (!isItLevitate)
                        Levitate();
    }
    #endregion

    #region Methods
    #region Methods.SetUpManager();
    public void SetUpManager()
    {
        if (stopButton != null)
            stopButton.GetComponent<Button>().interactable = false;
        else
            throw new System.NullReferenceException("ElevationManager > No stop button has been referenced to stop the animation and put the 3D model on the ground.");

        if (objectToAffect != null)
        {
            if (objectToAffect.transform.parent.gameObject != null)
                objectContainer = objectToAffect.transform.parent.gameObject;
            else
                throw new System.NullReferenceException("ElevationManager > Place 'objectToAffect' in a parent to avoid coordinate problems during the animation.");

            if (playAnimation)
                if (objectToAffect.transform.TryGetComponent(out Animator Animator))
                    objectAnimator = Animator;
                else
                    throw new System.NullReferenceException("ElevationManager > unable to retrieve the animation to play on the <Animation> component of object 'objectToAffect'");
        }
        else
            throw new System.NullReferenceException("ElevationManager > 'objectToAffect' has no reference set to an instance of an object.");

        if (objectDefinesHeightGround)
        {
            if (heightReferenceObject != null)
            {
                groundPosition = heightReferenceObject.transform.localPosition.y;
                saveGroundPos = groundPosition;
            }
            else
                throw new System.NullReferenceException("ElevationManager > The option for the ground plane to define the height has no reference set to an instance of an object.");
        }
        else
        {
            groundPosition = objectContainer.transform.localPosition.y;
            saveGroundPos = groundPosition;
        }

        if (movementManager == null)
        {
            objectToAffect.TryGetComponent<MovementManager>(out MovementManager manager);
            { movementManager = manager; };
        }
    }
    #endregion

    #region Methods.GoToTarget();
    void GoToTarget()
    {
        Vector3 travelDistance = Target - objectContainer.transform.localPosition;
        if (!movementManager.dragging)
            objectContainer.transform.localPosition = Vector3.SmoothDamp(objectContainer.transform.localPosition, Target, ref Velocity, Speed * Time.deltaTime);

        if (travelDistance.magnitude < 0.1f)
        {
            if (!isItLevitate)
                isItLevitate = true;
            else
                isItLevitate = false;
            CancelInvoke("GoToTarget");
        }
    }
    #endregion

    #region Methods.Levitate();
    void Levitate()
    {
        Target = new Vector3(objectContainer.transform.localPosition.x, saveGroundPos, objectContainer.transform.localPosition.z) + new Vector3(0f, objectFloatHeight, 0f);
        stopButton.GetComponent<Button>().interactable = true;
        InvokeRepeating("GoToTarget", 0f, Time.deltaTime);
        PlayAnimation();
    }
    #endregion

    #region StopLevitate();
    public void StopLevitate()
    {
        Target = new Vector3(objectContainer.transform.localPosition.x, saveGroundPos, objectContainer.transform.localPosition.z);
        stopButton.GetComponent<Button>().interactable = false;
        InvokeRepeating("GoToTarget", 0f, Time.deltaTime);
        StopAnimation();
    }
    #endregion

    #region Methods.IsOnObject();
    bool IsOnObject()
    {
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("ObjectToAffect"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        else
            return false;
    }
    #endregion

    #region Methods.PlayAnimation();
    public void PlayAnimation()
    {
        if (objectAnimator != null)
        {
            objectAnimator.enabled = true;
            objectAnimator.Play("Floating");
        }
        else
            Debug.LogWarning("No Animator to play");
    }
    #endregion

    #region Methods.StopAnimation();
    public void StopAnimation()
    {
        if (objectAnimator != null)
            objectAnimator.enabled = false;
        else
            Debug.LogWarning("No Animator to play");
    }
    #endregion
    #endregion
}
