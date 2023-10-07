# UnityUtility
Adds a few base classes to Unity to improve existing Behaviours.

**Dependencies** \
Since Unity cannot handle package-level Git package dependencies, this has to be added manually:
- Editor Utility: https://github.com/xZenvin/UnityEditorUtility.git


### MonoBehavior
- Class name spelling changed from BE to AE to fit Unity's other naming schemes
- Added caching for `Transform` component
- Added implicit conversion from `Transform` and `GameObject` to `MonoBehavior`

### MonoBehaviorAutoInit
A version of the `MonoBehavior`, which can automatically get components for any contained fields decorated with the `[AutoInit]` attribute.
Getting components can happen on child or parent objects, or on the object the script is attached to.
Arrays are supported as well.
If a component reference is not found, and the target field is denoted as critical, the `MonoBehaviorAutoInit` will disable itself.

### UnityGuid
A `struct` inspired by [`System.Guid`](https://learn.microsoft.com/en-us/dotnet/api/system.guid?view=net-7.0), which Unity can actually serialize. \
For the sake of displaying it in the inspector more easily (and because I suck at writing bitshift code), the struct encapsulates a byte array, rather than a set of integral numbers.

### UnityNullable
A `struct` inspired by [`System.Nullable<T>`](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1?view=net-7.0), which Unity can actually serialize. \
Uses a custom property drawer to set values to "null". Note that drawing nested properties is not currently supported.

### Point
A `struct` representing a point in a scene. Other than a normal `Vector3`, a `Point` may reference a `Transform` to move about instead of merely being a fixed position. \
A custom property drawer allows switching modes between "Transform" and "Vector3".

### StateQueue and EventBasedStateQueue
`StateQueue`s can be used declare a value in a central place and have it influenced by multiple parties, without those parties interfering with each other. \
The normal `StateQueue<T>` uses an `interface`-based approach, while the `EventBasedStateQueue` uses callbacks to manipulate its base value. \
A queue's base value is serialized (so long as it does not have nested properties). **Note that editor-changes to the base value need to be applied explicitly with a call to the containing queue's `Update()` method**. \
Both types of queue implement `IStateQueue<T>`.

### SerializedTable
**Not stable. Use at own risk.** \
A `class` to associate a value with two arbitrarily typed keys. \
Can be serialized and accessed via a custom editor window.
