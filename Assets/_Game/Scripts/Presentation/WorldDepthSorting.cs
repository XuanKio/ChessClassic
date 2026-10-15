using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SortingGroup))]
public sealed class WorldDepthSorting : MonoBehaviour
{
    [SerializeField] private float feetOffset = -0.26f;
    [SerializeField, Min(1)] private int orderPerUnit = 100;
    private SortingGroup sortingGroup;

    private void OnEnable()
    {
        sortingGroup = GetComponent<SortingGroup>();
        RefreshOrder();
    }

    private void LateUpdate()
    {
        RefreshOrder();
    }

    public void RefreshOrder()
    {
        if (sortingGroup == null)
            sortingGroup = GetComponent<SortingGroup>();
        int order = Mathf.RoundToInt(-(transform.position.y + feetOffset) * orderPerUnit);
        if (sortingGroup.sortingOrder != order)
            sortingGroup.sortingOrder = order;
    }
}
