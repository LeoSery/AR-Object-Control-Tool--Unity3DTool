/////////////////////////////////////////////////////////////////////////
// AR-Object-Control-Tool -- MovementManager
// Author: Léo Séry
// Date created: 30/12/2022
// Last updated: 15/02/2023
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

    public GameObject ObjectContainer;
    public GameObject ObjectToAffect;

    public bool useExternalScript = false;
    public bool useElevetionManager = false;
    public bool fullAR = false;

    public string targetTag;
    public bool blockingIU;

    public float maxObjectScale = 3f;
    public float minObjectScale = 1f;
    public bool useScaleIndicator = false;
    public bool showRawScale = false;
    public TextMeshProUGUI scaleText;
    public float baseScale;
    public float currentScale;
    public float scaleFactor;

    public RotationAxis rotationAxis;
    [Range(0, (float)0.1)]
    public float rotationSpeed = 0.5f;
    public GameObject loadingObject;

    [HideInInspector] public UnityEngine.UI.Image imageLoading;
    [HideInInspector] public ElevationManager elevationManager;
    [HideInInspector] public bool dragging = false;
    private bool IsCurrentlyReplace = false;
    private readonly float startingTimeToHold = 0.75f;
    private readonly float timeToHold = 1.25f;
    private float currentStartingTime = 0f;
    private float currentTime = 0f;
    private float initialDistance;
    private UnityEngine.Vector3 initialScale;
    private UnityEngine.Vector3 touchOffset;
    private Transform toDrag;
    #endregion

    #region UnityFunctions
    void Start()
    {
        SetTargetObjectSettings();
    }

    void Update()
    {
        if (ObjectToAffect != null)
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
                        Rescale();
                }
                else
                    return;
            }
            else
                return;
        }
    }
    #endregion

    #region Methods
    #region Methods.SetTargetObjectSettings();
    void SetTargetObjectSettings()
    {
        if (Replacement)
        {
            if (loadingObject == null)
                throw new System.NullReferenceException("MovementManager > Option to allow moving is TRUE but 'loadingObject' has no reference set to an instance of an object.");
            if (!loadingObject.TryGetComponent<UnityEngine.UI.Image>(out imageLoading))
                throw new System.NullReferenceException("MovementManager > Option to allow moving is TRUE but the component 'Image' of 'loadingObject' is null.");
        }

        if (ObjectToAffect == null)
            ObjectToAffect = transform.gameObject;
        else
        {
            if (ObjectContainer == null)
                ObjectContainer = ObjectToAffect.transform.parent.gameObject;

            if (Movement)
            {
                if (!ObjectToAffect.CompareTag("ObjectToAffect"))
                    throw new System.NullReferenceException("MovementManager > Option to allow moving is TRUE but 'ObjectToAffect' does not have the necessary 'ObjectToAffect' tag to use the motion option.");
            }
        }

        if (useScaleIndicator)
        {
            if (scaleText == null)
                throw new System.NullReferenceException("MovementManager > Option to allow scaling is TRUE but 'scaleText' has no reference set to an instance of an object.");
            baseScale = ObjectToAffect.transform.localScale.x;
            currentScale = baseScale;
            ChangeTextValue(FormatValue(currentScale));
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
                if (!IsCurrentlyReplace)
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
                        ObjectToAffect.transform.Rotate(touch.deltaPosition.y * rotationSpeed, 0f, 0f, Space.World);
                        break;
                    case RotationAxis.Y:
                        ObjectToAffect.transform.Rotate(0f, -touch.deltaPosition.x * rotationSpeed, 0f, Space.World);
                        break;
                    case RotationAxis.All:
                        ObjectToAffect.transform.Rotate(touch.deltaPosition.y * rotationSpeed, -touch.deltaPosition.x * rotationSpeed, 0f, Space.World);
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
                    initialDistance = UnityEngine.Vector2.Distance(touchZero.position, touchOne.position);
                    initialScale = ObjectToAffect.transform.localScale;
                }
                else
                {
                    if (ObjectToAffect.transform.localScale.x <= minObjectScale || ObjectToAffect.transform.localScale.y <= minObjectScale || ObjectToAffect.transform.localScale.z <= minObjectScale)
                        ObjectToAffect.transform.localScale = new UnityEngine.Vector3(minObjectScale, minObjectScale, minObjectScale);
                    else if (ObjectToAffect.transform.localScale.x >= maxObjectScale || ObjectToAffect.transform.localScale.y >= maxObjectScale || ObjectToAffect.transform.localScale.z >= maxObjectScale)
                        ObjectToAffect.transform.localScale = new UnityEngine.Vector3(maxObjectScale, maxObjectScale, maxObjectScale);

                    float currentDistance = UnityEngine.Vector2.Distance(touchZero.position, touchOne.position);

                    if (Mathf.Approximately(initialDistance, 0))
                        return;

                    float factor = currentDistance / initialDistance;
                    if (ObjectToAffect.transform.localScale.x >= (1 - minObjectScale) && ObjectToAffect.transform.localScale.x <= (1 + maxObjectScale))
                        ObjectToAffect.transform.localScale = initialScale * factor;
                    if (ObjectToAffect.transform.localScale.x > (1 + maxObjectScale))
                        ObjectToAffect.transform.localScale = new UnityEngine.Vector3(1 + maxObjectScale, 1 + maxObjectScale, 1 + maxObjectScale);
                    else if (ObjectToAffect.transform.localScale.x < (1 - minObjectScale))
                        ObjectToAffect.transform.localScale = new UnityEngine.Vector3(1 - minObjectScale, 1 - minObjectScale, 1 - minObjectScale);
                }
                currentScale = ObjectToAffect.transform.localScale.x;
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
    #endregion

    #region Methods.Move();
    public void Move()
    {
        if (Movement)
        {
            Touch touch = Input.GetTouch(0);
            UnityEngine.Vector3 pos = touch.position;

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

                        touchOffset = toDrag.position - Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, Camera.main.WorldToScreenPoint(toDrag.position).z));
                    }
                }
            }

            if (dragging && touch.phase == TouchPhase.Moved)
            {
                UnityEngine.Vector3 position = new UnityEngine.Vector3(pos.x, pos.y, Camera.main.WorldToScreenPoint(toDrag.position).z);
                toDrag.position = Camera.main.ScreenToWorldPoint(position) + touchOffset;
            }

            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                toDrag.position = new UnityEngine.Vector3(toDrag.position.x, toDrag.position.y, toDrag.position.z);
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