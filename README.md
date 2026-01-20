# unity-save-system

OVERVIEW :-

This project is a lightweight save/load framework for Unity that removes the need to manually write serialization code for every game object.

Instead of explicitly saving fields one by one, this system uses C# reflection to automatically discover and persist marked data at runtime.



THE PROBLEM :-

In most games, saving state involves:

-> writing repetitive serialization code

-> updating save logic every time a field is added or renamed

-> tightly coupling gameplay code with persistence logic

This quickly becomes error-prone and hard to iterate over.

I found this especially frustrating when I wanted a save system for a small Game Jam game since time is the most important resource in Game Jams.



THE CORE IDEA :-

First you have to decide which variables you want to be saved from the components on a game object.

Once you do that... that's it, you're done. Save Manager and Save Handler will handle all the hardwork from there.



SETUP :-

SaveableObject.cs : Must be on every object that holds a variable that you want to save. Add the component that holds that variable and then write the name of the variable (it's case sensitive so be careful)

SaveManager.cs : There should be exactly one instance of it in EVERY scene. This holds references to every single SaveableObject.cs in that scene.

SaveHandler.cs : This should be at the start scene(s) of your game as it's a singleton and handles the reading and writing part.



PROS :-

-> Easy to setup...



CONS :-

-> May be slow and harder to manage in bigger projects

-> Uses .txt files

-> Save data may get corrupted during development phase due to code refactors (Little to no chance of corruption in builds)

-> Only supports a handful of data types



CONCLUSION :-

Just don't use it for anything other than Game Jam projects...

I'm proud of it though, this was my first innovative project.
