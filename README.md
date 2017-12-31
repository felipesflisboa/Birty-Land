# Birty-Land
This is a top-down shooter game with some concepts of RPGs and MOBAs where you fight against hordes of creatures and bosses. Made with Unity game engine.

8-bit project who uses 3D components (Collider/Rigibody) because it was made initially to be a 3D project. WebGL version works on mobile browsers (or at least Firefox) responsively changing the controls.

In each WebGL build. Build/UnityLoader.js file need to have line `e.popup("Please note that Unity WebGL is not currently supported on mobiles. Press OK if you wish to continue anyway.",[{text:"OK",callback:t}])` changed to `t()`. This way, the alert about Unity WebGL is not supported on mobiles won't be show.