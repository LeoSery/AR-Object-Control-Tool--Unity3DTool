/////////////////////////////////////////////////////////////////////////
// AR-Object-Control-Tool -- MovementManager
// Author: Léo Séry
// Date created: 30/12/2022
// Last updated: 17/02/2023
// Purpose: Allow manipulation of a GameObject in AR with touch input.
// Documentation: https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool
/////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;
using TMPro;

public class MovementManager : MonoBehaviour
{
    #region Fields
    public enum RotationAxis
    {
        All, X, Y,
    }

    public bool Scaling = false;
    public bool Rotating = false;
    public bool Replacement = false;
    public bool Movement = false;

    public GameObject objectContainer;
    public GameObject objectToAffect;
    public Camera targetCamera;
    public Camera targetUICameraPrefab;
    public Camera targetUICamera;

    public bool useExternalScript = false;
    public bool useElevetionManager = false;
    public bool fullAR = false;

    public string targetTag;
    public bool blockingIU;

    public float maxObjectScale = 3f;
    public float minObjectScale = 1f;
    public bool useScaleIndicator = false;
    public bool showRawScale = false;
    public GameObject scaleIndicatorImagePrefab;
    public GameObject scaleIndicatorImage;
    public TextMeshPro scaleText;
    public float baseScale;
    private float currentScale;
    private float scaleFactor;
    private float baseCanvasScale;
    float minCanvasScale;
    float maxCanvasScale;
    public GameObject resetScaleButton;

    public RotationAxis rotationAxis;
    [Range(0, (float)0.5)]
    public float rotationSpeed = 0.01f;
    public GameObject loadingObject;

    [HideInInspector] public UnityEngine.UI.Image imageLoading;
    [HideInInspector] public ElevationManager elevationManager;
    [HideInInspector] public bool dragging = false;
    private bool isCurrentlyReplace = false;
    private readonly float startingTimeToHold = 0.75f;
    private readonly float timeToHold = 1.25f;
    private float currentStartingTime = 0f;
    private float currentTime = 0f;
    private float initialDistance;
    private Vector3 initialScale;
    private Vector3 touchOffset;
    private Transform toDrag;

    #endregion

    #region UnityFunctions
    void Start()
    {
        SetTargetObjectSettings();
    }

    void Update()
    {
        if (objectToAffect != null)
        {
            if ((useElevetionManager && elevationManager.isItLevitate) || !useExternalScript)
            {
                if (IsOnUI() == false)
                {
                    if (Input.touchCount == 0)
                    {
                        if (Replacement)
                        {
                            imageLoading.fillAmount = 0f;
                            loadingObject.SetActive(false);
                        }
                        currentTime = 0f;
                    }
                    if (Input.touchCount == 1)
                    {
                        if (IsOnObject())
                        {
                            Move();
                            return;
                        }
                        else if (IsTimeToReplace())
                        {
                            Replace();
                            return;
                        }
                        else
                        {
                            Rotate();
                            return;
                        }
                    }
                    if (Input.touchCount == 2)
                    {
                        if (useScaleIndicator)
                            scaleIndicatorImage.transform.gameObject.SetActive(true);
                        Rescale();
                    }
                    else
                    {
                        if (useScaleIndicator)
                            scaleIndicatorImage.transform.gameObject.SetActive(false);
                    }
                }
                else
                    return;
            }
            else
                return;
        }
    }

    void LateUpdate()
    {
        if (Scaling && useScaleIndicator)
            UpdateScaleIndicator(currentScale);
    }
    #endregion

    #region Methods
    #region Methods.SetTargetObjectSettings();
    void SetTargetObjectSettings()
    {
        if (Replacement)
        {
            if (loadingObject == null)
                throw new NullReferenceException("MovementManager > Option to allow moving is TRUE but 'loadingObject' has no reference set to an instance of an object.");
            if (!loadingObject.TryGetComponent<UnityEngine.UI.Image>(out imageLoading))
                throw new NullReferenceException("MovementManager > Option to allow moving is TRUE but the component 'Image' of 'loadingObject' is null.");
        }

        if (objectToAffect == null)
            objectToAffect = transform.gameObject;
        else
        {
            if (objectContainer == null)
                objectContainer = objectToAffect.transform.parent.gameObject;

            if (Movement)
            {
                if (!objectToAffect.CompareTag("ObjectToAffect"))
                    throw new NullReferenceException("MovementManager > Option to allow moving is TRUE but 'objectToAffect' does not have the necessary 'objectToAffect' tag to use the motion option.");
            }
        }

        if (Scaling && useScaleIndicator)
        {
            scaleIndicatorImage = Instantiate(scaleIndicatorImagePrefab, objectToAffect.transform);
            targetUICamera = Instantiate(targetUICameraPrefab);

            if (scaleText == null)
            {
                scaleText = scaleIndicatorImage.GetComponentInChildren<TextMeshPro>(true);
                if (scaleText == null)
                    throw new NullReferenceException("MovementManager > The scaling option is TRUE but the 'scaleText' field is empty and the object could not be retrieved via the instance of `scaleIndicatorImagePrefab` object.");
                // todo rework error message
            }

            baseScale = objectToAffect.transform.localScale.x;
            currentScale = baseScale;
            ChangeTextValue(FormatValue(currentScale));
            scaleIndicatorImage.SetActive(false);

            if (targetUICameraPrefab != null)
            {
                targetUICamera.transform.SetParent(targetCamera.transform);
                targetUICamera.transform.gameObject.SetActive(true);
                targetUICamera.transform.localPosition = Vector3.zero;
                targetUICamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
            else
                throw new NullReferenceException("MovementManager > Option to show `Scale Indicator` is TRUE but 'targetUICameraPrefab' has no reference set to an instance of an object.");

            if (scaleIndicatorImage != null)
            {
                scaleIndicatorImage.transform.SetParent(objectContainer.transform);
                baseCanvasScale = scaleIndicatorImage.transform.localScale.x;
                minCanvasScale = baseCanvasScale / 2;
                maxCanvasScale = baseCanvasScale * 2;
            }
            else
                throw new NullReferenceException("MovementManager > Option to show `Scale Indicator` is TRUE but 'scaleIndicatorCanvas' has no reference set to an instance of an object.");
        }
    }
    #endregion

    #region Methods.ISOnObject();
    bool IsOnObject()
    {
        if (Input.touchCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("ObjectToAffect"))
                    return true;
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

    #region Methods.IsOnUI();
    bool IsOnUI()
    {
        if (blockingIU)
        {
            if (Input.touchCount > 0)
            {
                PointerEventData pointerEventData = new(EventSystem.current) { position = Input.GetTouch(0).position };

                List<RaycastResult> raycastResultList = new();
                EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
                for (int i = 0; i < raycastResultList.Count; i++)
                {
                    if (raycastResultList[i].gameObject.CompareTag(targetTag.ToString()))
                        return true;
                }
                return false;
            }
            else
                return false;
        }
        else
            return false;
    }
    #endregion

    #region Methods.IsTimeToReplace();
    bool IsTimeToReplace()
    {
        if (Replacement)
        {
            if (Input.touchCount > 0)
            {
                if (!isCurrentlyReplace)
                {
                    imageLoading.fillAmount = 0f;
                    loadingObject.SetActive(true);
                    imageLoading.transform.position = Input.GetTouch(0).position;

                    if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                    {
                        if (currentStartingTime < startingTimeToHold)
                        {
                            currentStartingTime += Time.deltaTime;
                            return false;
                        }
                        else
                        {
                            if (Input.GetTouch(0).phase != TouchPhase.Ended)
                            {
                                currentTime += Time.deltaTime;
                                imageLoading.fillAmount = currentTime / timeToHold;

                                if (currentTime < timeToHold)
                                    return false;
                                else if (currentTime == timeToHold || currentTime > timeToHold)
                                {
                                    currentTime = 0f;
                                    imageLoading.fillAmount = 0f;
                                    return true;
                                }
                                else
                                    return false;
                            }
                            else if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled)
                            {
                                imageLoading.fillAmount = 0f;
                                loadingObject.SetActive(false);
                                currentTime = 0f;
                                return false;
                            }
                            else
                                return false;
                        }
                    }
                    else
                    {
                        currentStartingTime = 0f;
                        return false;
                    }
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

    #region Methods.Rotate();
    void Rotate()
    {
        if (Rotating)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                if (Replacement)
                    loadingObject.SetActive(false);
                currentTime = 0f;
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        objectToAffect.transform.Rotate(touch.deltaPosition.y * rotationSpeed, 0f, 0f, Space.World);
                        break;
                    case RotationAxis.Y:
                        objectToAffect.transform.Rotate(0f, -touch.deltaPosition.x * rotationSpeed, 0f, Space.World);
                        break;
                    case RotationAxis.All:
                        objectToAffect.transform.Rotate(touch.deltaPosition.y * rotationSpeed, -touch.deltaPosition.x * rotationSpeed, 0f, Space.World);
                        break;
                }
            }
        }
        else
            return;
    }
    #endregion

    #region Methods.Rescale();
    void Rescale()
    {
        if (Scaling)
        {
            if (Input.touchCount > 0)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                    touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                    return;

                if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                    initialScale = objectToAffect.transform.localScale;
                }
                else
                {
                    float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                    if (Mathf.Approximately(initialDistance, 0))
                        return;

                    float factor = currentDistance / initialDistance;
                    float newScale = initialScale.x * factor;
                    newScale = Mathf.Clamp(newScale, minObjectScale, maxObjectScale);
                    objectToAffect.transform.localScale = new Vector3(newScale, newScale, newScale);
                }
                currentScale = objectToAffect.transform.localScale.x;
                if (useScaleIndicator)
                    ChangeTextValue(FormatValue(currentScale));
            }
        }
        else
            return;
    }

    #region Methods.Rescale.ChangeTextValue();
    public void ChangeTextValue(string value)
    {
        if (scaleText != null)
            scaleText.text = value;
    }
    #endregion

    #region Methods.Rescale.FormatValue();
    public string FormatValue(float currentScaleValue)
    {
        scaleFactor = currentScaleValue / baseScale;

        int integerPart = (int)Math.Truncate(scaleFactor);
        int integerPartLength = integerPart.ToString().Length;

        string floatingPart = "";
        string res = "";

        if (currentScaleValue >= baseScale)
        {
            if ((scaleFactor - integerPart) > 0)
                floatingPart = scaleFactor.ToString().Substring(integerPartLength + 1);

            if (integerPartLength >= 2)
            {
                res = "x" + integerPart.ToString();
            }
            else if (integerPartLength == 1)
            {
                if (floatingPart.Length > 1)
                    res = "x" + integerPart + "." + floatingPart[0];
                else
                    res = "x" + integerPart;
            }
            return res;
        }
        else
        {
            int fractionDenominator = (int)(1 / scaleFactor);
            if (showRawScale)
            {
                // Version with all denominator values.
                res = "1/" + fractionDenominator.ToString();
            }
            else
            {
                // Version with logarithmically managed steps.
                float step = GetStep(scaleFactor);
                fractionDenominator = (int)(fractionDenominator / step) * (int)step;
                res = "1/" + fractionDenominator.ToString();
            }

        }
        return res;
    }
    #endregion

    #region Methods.Rescale.GetStep();
    private float GetStep(float scaleFactor)
    {
        float denominator = 1 / scaleFactor;
        float step = (float)Math.Pow(10, Math.Floor(Math.Log10(denominator)));
        return step;
    }
    #endregion

    #region Methods.Rescale.ResetScale();
    public void ResetScale()
    {
        objectToAffect.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
        currentScale = baseScale;
        ChangeTextValue(FormatValue(currentScale));
    }
    #endregion

    #region Methods.Rescale.UpdateScaleIndicator();
    void UpdateScaleIndicator(float currentScaleValue)
    {
        float currentCanvasScale = currentScaleValue * baseCanvasScale;
        float newCanvasScale = Mathf.Clamp(currentCanvasScale, minCanvasScale, maxCanvasScale);

        scaleIndicatorImage.transform.LookAt(scaleIndicatorImage.transform.position + targetCamera.transform.forward);

        scaleIndicatorImage.transform.localScale = new Vector3(newCanvasScale, newCanvasScale, newCanvasScale);
    }
    #endregion

    #endregion

    #region Methods.Move();
    public void Move()
    {
        if (Movement)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("ObjectToAffect"))
                    {
                        if (useElevetionManager)
                            elevationManager.StopAnimation();
                        toDrag = hit.transform.parent;
                        dragging = true;

                        touchOffset = toDrag.position - Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, Camera.main.WorldToScreenPoint(toDrag.position).z));
                    }
                }
            }

            if (dragging && touch.phase == TouchPhase.Moved)
            {
                Vector3 position = new Vector3(pos.x, pos.y, Camera.main.WorldToScreenPoint(toDrag.position).z);
                toDrag.position = Camera.main.ScreenToWorldPoint(position) + touchOffset;
            }

            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                toDrag.position = new Vector3(toDrag.position.x, toDrag.position.y, toDrag.position.z);
                dragging = false;
                if (useElevetionManager)
                    elevationManager.PlayAnimation();
            }
        }
        else
            return;
    }
    #endregion

    #region Methods.Replace();
    public void Replace()
    {
        //call your AR replacement function here
        Debug.LogWarning("Replacement of the object...");
    }
    #endregion
    #endregion
}