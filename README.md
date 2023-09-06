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
