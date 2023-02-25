# AR-Object-Control-Tool

The **_AR-Object-Control-Tool_** for **_Unity3D_** allows you to easily control your objects in your Unity3D projects. Just add the necessary scripts to your code, configure them following the documentation below and you will be able to **_move_**, **_rotate_** and **_rescale_** of your objects in real time. This tool is easy to use and **_modulable_**, supports **_touch devices_** and **_augmented reality_**.

Watch the video presentation of the tool :

[![Watch the video](https://i.imgur.com/Jg8Nao6.png)](https://youtu.be/bI6uYFKny5s)

## Contents:

- **Presentation** of the tool.
- The different **modules**.
- How to **install the tool** in your project.
- How to **configure the tool** for your project.

## Tool Overview:

The **_AR-Object-Control-Tool_** is divided into two main parts. The first part is the **`MovementManager.cs`** script which is the basis of the tool and allows you to use the main functions such as :

- **Move** the object by touching it and moving it on the screen.
- **Rotate** the object by using one or more fingers to rotate the screen.
- **Rescale** the size of the object by using a pinching motion on the screen with two fingers.
- **Move** the object to its original position.

The second part of the tool, located in the **`ElevationManager.cs`** script, is a **complementary module** to the first script. It can **also be used independently** if you do not want to manipulate your object. This part of the tool will allow you to **float an object** and play an animation on it.

It is important to note that for now, the module only supports **_touch inputs_**. This tool requires a version of Unity higher or equal to [`Unity 2021.X`](https://unity.com/releases/editor/archive).

## The different modules:

### **_MovementManager_**:

The object manipulation module allows to manipulate any GameObject that has been defined as a target. To define a GameObject as a target, you just have to add the tag _`ObjectToAffect`_. Once your object is defined as a target, you can use the following functions:

- ### Move your object:

  This function allows you to move an object that has been set as a target. Simply place your finger on the object and move it where you want.

![](https://i.imgur.com/72Gcjoy.gif)

- ### Rotate your object:

  This function allows your target object to rotate (locally) based on the movement of your Input_0 (Input.GetTouch(0) which is the first finger to touch the screen).

![](https://i.imgur.com/rbcjUIP.gif)

There are two parameters available for this function:

1. The axis of rotation:
   - `All`
   - `X`
   - `Y`
2. The speed of rotation.

![](https://i.imgur.com/0Yyarsp.gif)

- ### Change the size of your object:

  This function allows you to change the size of the object that has been set as the target. To do this, use two fingers and make a pinching motion on the screen to reduce the size of your object, and make the opposite motion to increase its size. You can set a minimum and maximum size by changing the `maxObjectScale` and `minObjectScale` parameters in the _`Scale`_ category of the _`Selected options settings`_ block.

![](https://i.imgur.com/C9Auu3e.gif)

- ### Reposition your object:

  The tool supports augmented reality and allows you to reposition an object using the placement function of your AR solution by adding a line of code in the `MovementManager.cs` script.

### **_ElevationManager_** :

The object floatation module allows you to lift an object to a target position and then play an animation. The animation is by default an animation allowing the object to float but, it can be changed later for a more advanced use of the tool.

## How to install the tool in your project:

Download the tool via this [**UnityPackage**](https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool/releases).

Once downloaded, open unity and drag it into your `project` window. Once the import window is open, select the scripts you want to import between `MovementManager.cs` and `ElevationManager.cs`. For each of the two scripts make sure that its Editor script (`...CustomEditor`) is also selected and click `import`.

In the demo project you can find the scripts in :

```
../Assets/AR-Object-Control-Tool/Scripts
```

In the UnityPackage you can find the scripts in :

```
../AR-Object-Control-Tool/Scripts
```

To use the tool you can use the prefab `AR-Object-Control-Tool` which is located in `../Assets/Prefabs` and drag and drop it into your scene. Then you only have to set up the tool following the documentation below.

Or you can choose to use the scripts independently by choosing where you want to place them. To do this drag and drop the script of your choice on an object of your scene so that it is active. It is recommended to add it on the object you want to set as target because if you don't reference any object in the `ObjectToAffect` field, by default the script will take the object it is present on.

You can put both the `MovementManager` and `ElevationManager` components on the same object but it is not recommended to put the same component on the same object several times.

## How to configure the tool for your project:

### **_MovementManager_** :

By default when the component is added on an object you have 4 categories:

- **Allowed Options**

  This category will define with which type of manipulation your object will be able to be manipulated. There are 4 types of manipulation possible:

  1. _Scaling_
  2. _Rotating_
  3. _Replacement_
  4. _Movement_

  Each of these options has a checkbox to enable / disable the functionality. They can be used independently or simultaneously.

- **General Settings**

  Will allow you to manage the use of external scripts such as the `ElevationManager` that comes with the tool or for more advanced use, another script can be configured to interact with this script.

- **Script Target**

  Main category of the component that will allow to define which object will be the target of the manipulation done by the script. To define an object as a target you need to reference this object in the `Object To Affect` field and give it the tag: `ObjectToAffect`.
  It is recommended to place the target object as a child of an empty and to reference the empty in the `Object Container` field. This may avoid some problems with position calculations or animation problems if you use the `ElevationManager` component.

- **UI**

  By default the tool does not take into account the UI display and will calculate the movement of your finger even if you are on a button or an image displayed by your canvas. By checking the `Throught UI` option you can define a tag for which the script should stop manipulating the target object when your finger is currently on a UI with that tag.

By enabling some of the options listed above, other options may appear in the component, you will see how to configure them in the rest of this documentation.

By checking one or more `Allowed Options` a new large category called `Selected Options Settings` appears and gives you access to new configuration options such as :

- **Scale**

  This category appears when you check the _`Scaling`_ field.
  By unfolding this menu you have 2 new fields:

  `Min Object Scale` > will allow you to define the minimum size to which the object can shrink.

  `Max Object Scale` > will allow you to define the maximum size to which the object can grow.

  `Use Scale Indicator` > will let you know the size factor of the target object in relation to its original size. And if you check this option 3 new parameters will appear:

  - `Scale Indicator Image Prefab` > This field allows you to set a canvas that will be the receptacle for the UI that displays the scale factor of the object. The target canvas must be configured correctly to be used with the _Use Scale Indicator_ option. See below for steps on how to properly configure the target canvas.
  - `target UI Camera Prefab` > So that the scale indicator is always visible the tool uses a second camera which is responsible for rendering the indicator, you must reference it here. More information about the UI Camera below.
  - `Scale Text` > is the target text on which the current scale factor will be displayed.
  - `Base Scale` > is the reference value for the calculation of the scale of the target object. It corresponds to the size of the object when it is scaled 1:1.
  - `Show Raw Scale` > by default the negative values are displayed with logarithmic steps but by checking this option you can display the raw values and thus have all the different scale values.

  **Setting up the UI for the Scale Indicator :**

  It is recommended to use the `ScaleFactorCanvas` and the `UICamera` included in the [UnityPackage](https://github.com/LeoSery/AR-Object-Control-Tool--Unity3DTool/releases) prefab to use the `Scale Indicator Canvas` option.

  But you can use your own by configuring it with the following steps:

  Before setting up your UI, once your object and the `MovementManager` script are present in your scene make sure that the fields listed in the `General Settings` and `Script Target` categories are filled in. Then, follow these steps :

  ```
  1. Add a canvas to your scene, set its `Render Mode` to `World Space` and then place it where you want it on your target object.
  2. Drag your Canvas into the `Scale Indicator Canvas` field of the tool.
  3. Enter the target text of your UI canvas in the `Scale Text` field of the tool.
  4. Add a new Camera to your scene. Set its `Clear Flags` parameter to `Depth Only` and then `Depth` to 10.
  5. Remove the audio listener from the camera you just added.
  6. Add a Layer to your Canvas. Then in the `Culling Mask` field of your main camera uncheck this same layer so that the category changes to `Mixed`.
  7. Conversely, in the UI camera, in the `Culling Mask` field uncheck all layers except the one you just added.
  8. Enter the new UI camera in the `Target UI Camera` field of the tool.
  ```

---

The scale indicator uses a text object from the Unity package named `TextMeshPro`, if it is not already installed in your project you can see how to install it [here](https://learn.unity.com/tutorial/working-with-textmesh-pro).

- **Rotation**

  This category appears when you check the _`Rotating`_ field.
  In this foldout menu you can manage on which axis you want your target to rotate. Among the 3 following possibilities:

  `X` > the object will rotate on the horizontal axis, which corresponds to the **X** axis in Unity.

  `Y` > the object will rotate on the vertical axis, which corresponds to the **Y** axis in Unity.

  `All` > the object will rotate on all its axes (**X**,**Y**,**Z**) at the same time.

- **Move**

  This category appears when you check the _`Replacement`_ field.
  In this part you will be able to reference an image in the `Loading Object` field, it will be used to display the progress bar for your target replacement. The image is displayed when your finger remains static on the screen for a certain period of time.

  In order to use the replacement function you must call the replacement function of your augmented reality solution. You must call this function in the `MovementManager.cs` script, region `Methods.Replace();`.

  It looks like this:

  ```csharp
  public class MovementManager : MonoBehaviour
  {
      ...

      #region Methods.Replace();
      public void Replace()
      {
          //call your AR replacement function here
      }
      #endregion
  }
  ```

- **Additional Scripts**

  This category appears when you check the _`Use External Script`_ field located in the `General Settings` category. It will allow you to reference scripts so that they can interact with the `MovementManager`.

  Basically the tool is configured to receive an `ElevationManager`. If you fill in the script, by default you can only manipulate the object once it is floating, i.e. when the `isItLevitate` variable of the `ElevationManager` is set.

### **_ElevationManager_** :

At the top of this component you will find a field indicating the current state of the object, to indicate if it is currently floating or not.

- **General Settings**

  As with the `MovementManager` you will find a list of options that can be activated, which when at least one of them is activated, brings up a new category called `Selected Options Settings`.

  List of options you can enable/disable:

  1. _Play Animation_
  2. _Use External Script_
  3. _Use Vuforia_

- **GameObjects**

  As for the `MovementManager` this category is the main one of the component, it allows to define which object will be the target of the manipulation made by the script. To define an object as a target you have to reference this object in the `Object To Affect` field and assign it the tag: `ObjectToAffect`.
  It is recommended to place the target object as a child of an empty and to reference the empty in the `Object Container` field. This may prevent some animation problems when the object is levitated.

- **UI**

  To trigger the levitation, just touch the object that has been defined as the target and it will automatically move to the target position that you have defined through the `Object Float Height` field located in the `Animation` category once you have checked the `Play Animation` option above.
  As far as stopping the levitation is concerned, at the moment the tool does not support a second press on the target object, you need to reference a button that should call the function `ElevationManager.StopLevitate()`.

By checking one or more of the options located in `General Settings` a new large category named `Selected Options Settings` appears and gives you access to new configuration options such as :

- **Play Animation**

  By activating this option, you will see an `Animation` category that contains two fields:

  `Object Float Height` > which allows you to set the height at which the target object should rise when you click on it.

  `Speed` > which allows you to set the speed at which the target object should reach the height you set.

- **Use External Script**

  This category allows you to reference an external script such as the `MovementManager` to apply conditions and for example make the external script function only available when the `isItLevitate` variable is set to `True`. By default you can reference a `MovementManager`. But you can modify the script and add your own scritp for a more advanced use of the tool.

- **Use Vuforia**

  Just like the other module, this one supports augmented reality. Basically it is configured to be used with the "Vuforia" solution, so you can enable this option to have the return height of the target object defined by vuforia by enabling the `Object Defines Height` option and filling in the `Height Reference Object` field.
