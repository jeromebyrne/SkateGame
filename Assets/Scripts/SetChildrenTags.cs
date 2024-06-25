using UnityEngine;

public class SetChildrenTags : MonoBehaviour
{
    [SerializeField] private string tagToSet = "YourTag"; // The tag you want to set for the parent and all children

    void Start()
    {
        // Set the tag for the parent GameObject
        gameObject.tag = tagToSet;

        // Set the tag for all child GameObjects
        SetTagForAllChildren(transform, tagToSet);
    }

    private void SetTagForAllChildren(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            child.tag = tag;

            // If the child has its own children, recursively set their tags as well
            if (child.childCount > 0)
            {
                SetTagForAllChildren(child, tag);
            }
        }
    }
}