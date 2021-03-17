# References Inspector Extension for Unity
An Editor extension that shows the user all objects in the Scene that reference the currently selected objects.

## Purpose
Sometimes you find yorself in other people's project or an old project of your own. It can be hard to see how a specific object in the scene tree is used.
For instance, a button without an onClick assigned in the properties, but referenced in a View Controller somewhere in the scene, and the onClick handler assigned in OnStart.
With this extension, you can find that View Controller, because it has a reference to that button.

Another great use is as an alternative way to quickly jump between objects without having to scroll through the tree.
