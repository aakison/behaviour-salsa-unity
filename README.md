# behaviour-salsa-unity
Behaviours, helpers, utilities that help with Unity development.

## Installation
This repo is designed to be used as a submodule of an existing Unity3D repo.  E.g.

```
    git submodule add https://github.com/aakison/behaviour-salsa-unity.git Assets/Modules/BehaviourSalsa
```

There are no demo scenes or resources in this repo, just the required files so that your solution is not polluted with unnecessary files.

## Feature: WaitFor helper

In Unity Coroutines, creating new objects in yield statements causes unnecessary heap allocations.  Use `WaitFor.Seconds(s)` to use a cached version.

