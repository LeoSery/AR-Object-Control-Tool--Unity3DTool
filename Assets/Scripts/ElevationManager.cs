using UnityEngine.UI;
using UnityEngine;

public class ElevationManager : MonoBehaviour
{
    [Header("Infos :")]
    public bool isItLevitate = false;
    public float groundPosition;

    [Header("GamesObjets :")]
    public GameObject objectToAffect;
    public GameObject parentObject;
    public Animator objectAnimator;

    [Header("Settings :")]
    public bool playAnimation;
    [Range(0.1f, 30f)]
    public float objectFloatHeight;
    [Range(300f, 1f)]
    public float Speed;

    [Header("Vuforia :")]
    public bool objectDefinesHeightGround;
    public GameObject heightReferenceObject;

    [Header("UI :")]
    public GameObject stopButton;

    [Header("Additional features scripts :")]
    public MovementManager movementManager;

    private Vector3 Velocity = Vector3.zero;
    public Vector3 Target;
    public float saveGroundPos;

    void Awake()
    {
        SetUpManager();
    }

    public void SetUpManager()
    {
        if (stopButton != null)
            stopButton.GetComponent<Button>().interactable = false;
        else
            throw new System.NullReferenceException("ElevationManager > No stop button has been referenced to stop the animation and put the 3D model on the ground.");

        if (objectToAffect != null)
        {
            if (objectToAffect.transform.parent.gameObject != null)
                parentObject = objectToAffect.transform.parent.gameObject;
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
            groundPosition = parentObject.transform.localPosition.y;
            saveGroundPos = groundPosition;
        }

        if (movementManager == null)
        {
            objectToAffect.TryGetComponent<MovementManager>(out MovementManager manager);
            { movementManager = manager; };
        }
    }

    void Start()
    {
        Target = new Vector3(parentObject.transform.position.x, saveGroundPos, parentObject.transform.position.z);
    }

    void Update()
    {
        if (parentObject != null)
            if (Input.touchCount == 1 || Input.GetKeyDown(KeyCode.Mouse0))
                if (IsOnObject())
                    if (!isItLevitate)
                        Levitate();
    }

    void GoToTarget()
    {
        Vector3 travelDistance = Target - parentObject.transform.localPosition;
        if (!movementManager.dragging)
            parentObject.transform.localPosition = Vector3.SmoothDamp(parentObject.transform.localPosition, Target, ref Velocity, Speed * Time.deltaTime);

        if (travelDistance.magnitude < 0.1f)
        {
            if (!isItLevitate)
                isItLevitate = true;
            else
                isItLevitate = false;
            CancelInvoke("GoToTarget");
        }
    }

    void Levitate()
    {
        Target = new Vector3(parentObject.transform.localPosition.x, saveGroundPos, parentObject.transform.localPosition.z) + new Vector3(0f, objectFloatHeight, 0f);
        stopButton.GetComponent<Button>().interactable = true;
        InvokeRepeating("GoToTarget", 0f, Time.deltaTime);
        PlayAnimation();
    }


    public void StopLevitate()
    {
        Target = new Vector3(parentObject.transform.localPosition.x, saveGroundPos, parentObject.transform.localPosition.z);
        stopButton.GetComponent<Button>().interactable = false;
        InvokeRepeating("GoToTarget", 0f, Time.deltaTime);
        StopAnimation();
    }

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

    public void StopAnimation()
    {
        if (objectAnimator != null)
            objectAnimator.enabled = false;
        else
            Debug.LogWarning("No Animator to play");
    }
}
