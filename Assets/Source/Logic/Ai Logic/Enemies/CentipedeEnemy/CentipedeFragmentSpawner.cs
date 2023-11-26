using UnityEngine;
using NTC.Global.Cache;

public class CentipedeFragmentSpawner : MonoCache
{
    [SerializeField] private CentipedeFragment baseFragmentPrefab, fragmentPrefab;
    [SerializeField] private int fragmentCount;
    [SerializeField] private float scaleMultiplier;

    private void Start()
    {
        SpawnCentipedeFragments();
    }

    [ContextMenu("Spawn fragments")]
    public void SpawnCentipedeFragments()
    {
        var fragments = new CentipedeFragment[fragmentCount];

        fragments[0] = Instantiate(baseFragmentPrefab, transform.position, Quaternion.identity);
        fragments[0].transform.localScale *= scaleMultiplier;
        fragments[0].GetRb().mass *= scaleMultiplier;

        fragments[0].transform.SetParent(transform);
        fragments[0].SetParentJoint(GetComponent<Rigidbody>());

        GetComponent<FixedJoint>().connectedBody = fragments[0].GetRb();

        for (var i = 1; i < fragmentCount; i++)
        {
            fragments[i] = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
            fragments[i].transform.SetParent(transform, true);
            fragments[i].transform.position = fragments[i-1].transform.position + Vector3.up * fragments[i-1].transform.localScale.y * (i == 1 ? 1 : 1.5f);

            var currentScaleMultiplier = Mathf.Lerp(scaleMultiplier, 1, (float)i / (float)fragmentCount);
            fragments[i].transform.localScale *= currentScaleMultiplier;
            fragments[i].GetRb().mass *= currentScaleMultiplier;

            fragments[i].SetParentJoint(fragments[i-1].GetRb());


        }
    }
}
