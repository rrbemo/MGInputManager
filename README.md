# MGInputManager
A Unity 3D project demonstrating a custom input manager with in game remapping and controller support. Initally developed for an indie game I'm working on, Convicted Galaxy. This project is free for anyone to use. If you make improvements, please keep a version free and open to the community.

If you want to show your support, let @rrbemo know on twitter, or check out my game, Convicted Galaxy: 
http://www.convictedgalaxy.com
http://store.steampowered.com/app/545840

# Requires
- Unity 3D (Built with 5.3.4 Personal).

# Install
- Download and unzip.
- Open with Unity 3D.

If you want to use this code in another project, make sure that you copy:

"ControllerTesting/Assets/MGInputManager" folder into the associated folder on your new project.

**WARNING: This will delete any previous Unity Input Manager setup you may have done in the new project**

"ControllerTesting/Project Settings/InputManager.asset" into the associated folder on your new project.

# Modifications

"ControllerTesting/Assets/MGInputManager/Scripts/InputManager.cs" (InputManager) contains most of the information you'll want to change or customize via code (beside the UI stuff). With any changes, you may need to modify your menu or UI to accommodate. The UI is auto-built from the ActionName enum in my scene, but you can do something different if you'd like.

- Add new Actions and apply their default Axis

Modify the PrepareActions() method of InputManager by adding additional default actions to each InputInterfaceType of interfaceInputActions. You will also want to add the new action the the ActionName enum. If there is an inverse action, make sure you specify that as well.

- Add additional Axis or Interface Types

Modify the AxisName or InputInterfaceType enums as desired. If you're adding a new InputInterfaceType, you will likely have to modify the PrepareActions() method of InputManager too. I haven't done either of these too much as of late, so there may be additional steps.
