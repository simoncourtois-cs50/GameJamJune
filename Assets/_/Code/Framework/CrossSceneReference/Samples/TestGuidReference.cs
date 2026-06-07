using UnityEngine;

public class TestGuidReference : MonoBehaviour
{
    public GuidReference target = new GuidReference();

    private void Awake()
    {
        target.OnGuidRemoved += OnTargetLost;
        target.OnGuidAdded += OnTargetFound;
    }

    private void OnTargetFound(GameObject go)
    {
        Debug.Log($"[GUID REF]: Référence résolue → {go.name}");
    }

    private void OnTargetLost()
    {
        Debug.Log("[GUID REF]: Référence perdue");
    }
}