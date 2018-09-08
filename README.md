# behaviour-salsa-unity
Behaviours, helpers, utilities that help with Unity development.

## Installation
This repo is designed to be used as a submodule of an existing Unity3D repo.  E.g.

```
    git submodule add https://github.com/aakison/behaviour-salsa-unity.git Assets/Modules/BehaviourSalsa
```

There are no demo scenes or resources in this repo, just the required files so that your solution is not polluted with unnecessary files.

## Feature: WaitFor helper

In Unity Coroutines, creating new objects in yield statements causes unnecessary heap allocations.  Use `WaitFor.Seconds(s)` to use a cached version.  For consistency, you can also use `WaitFor.NextFrame` and `WaitFor.EndOfFrame`.

## Feature: Destroy on Timeout

Add this component to an object, e.g. a particle system, to automatically destroy the object after a set number of seconds.

## Feature: Spawn on Timeout

Add this component to an object that will act as a placeholder for the position of a prefab to be added later.  E.g. to re-spawn a pickup after it's been picked up once.

## Feature: Quaternion Extensions

Adds the ability for a quaternion to be normalized so that massive rotations don't add up errors over time.

## Feature: Random Extensions

The `UnityEngine.Random` class has some handy functions like random on a unit sphere that don't exist on `System.Random`, but is static instead of an instance.  This adds some of those handy methods as extension methods to `System.Random`.

