using UnityEngine;
using NTC.Global.Cache;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;

public class CentipedeFragmentSpawner : MonoCache
{
    [SerializeField] private CentipedeFragment baseFragmentPrefab, fragmentPrefab, headPrefab;
    [SerializeField] private Rigidbody _headTarget;
    [SerializeField] private int fragmentCount;
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private float minUpForce, maxUpForce;

    private float _startSpring;
    private int _lastFragmentHitIndex;
    private bool _dead;

    private CentipedeFragment[] _fragments;
    private CentipedeFragment _head;
    private SpringJoint _headSpring;

    private void Awake()
    {
        SpawnCentipedeFragments();
    }

    protected override void OnEnabled()
    {
        var stunner = GetComponent<IInStun>();
        if (stunner != null)
        {
            stunner.OnStun += HandleStun;
            stunner.OnRecover += HandleStunRecover;
        }
        var healthHandler = GetComponent<IHealthHandler>();
        if (healthHandler != null)
        {
            healthHandler.OnDied += HandleDeath;
        }
    }

    protected override void OnDisabled()
    {
        var stunner = GetComponent<IInStun>();
        if (stunner != null)
        {
            stunner.OnStun -= HandleStun;
            stunner.OnRecover -= HandleStunRecover;
        }
        var healthHandler = GetComponent<IHealthHandler>();
        if (healthHandler != null)
        {
            healthHandler.OnDied -= HandleDeath;
        }
    }

    protected override void FixedRun()
    {
        if (_dead) return;
        for (var i = 1; i < _fragments.Length; i++)
        {
            if (!_fragments[i])
                continue;
            var force = Mathf.Lerp(maxUpForce, minUpForce, (float)i / (float)_fragments.Length);
            _fragments[i].GetRb().AddForce(Vector3.up * force, ForceMode.Acceleration);
        }
    }

    [ContextMenu("Spawn fragments")]
    public void SpawnCentipedeFragments()
    {
        _fragments = new CentipedeFragment[fragmentCount];

        _fragments[0] = Instantiate(baseFragmentPrefab, transform);
        _fragments[0].transform.position = transform.position;
        _fragments[0].transform.localScale *= scaleMultiplier;
        _fragments[0].GetRb().mass *= scaleMultiplier;
        _fragments[0].SetIndex(0);
        _fragments[0].OnFragmentHit += SetLastFragmentHitIndex;

        _fragments[0].SetParentJoint(GetComponent<Rigidbody>());

        GetComponent<FixedJoint>().connectedBody = _fragments[0].GetRb();

        for (var i = 1; i < fragmentCount; i++)
        {
            _fragments[i] = Instantiate(fragmentPrefab, transform);
            _fragments[i].transform.position = _fragments[i-1].transform.position + Vector3.up * _fragments[i-1].transform.localScale.y * (i == 1 ? 1 : 1.5f);

            var currentScaleMultiplier = Mathf.Lerp(scaleMultiplier, 1, (float)i / (float)fragmentCount);
            _fragments[i].transform.localScale *= currentScaleMultiplier;
            _fragments[i].GetRb().mass *= currentScaleMultiplier;
            _fragments[i].SetIndex(i);

            _fragments[i].SetParentJoint(_fragments[i-1].GetRb());

            _fragments[i].OnFragmentHit += SetLastFragmentHitIndex;
        }

        _head = Instantiate(headPrefab, transform);
        _head.transform.position = _fragments[_fragments.Length-1].transform.position + Vector3.up * _fragments[_fragments.Length-1].transform.localScale.y * 1.5f;
        _head.SetParentJoint(_fragments[_fragments.Length-1].GetRb());
        _head.SetIndex(_fragments.Length);
        _head.OnFragmentHit += SetLastFragmentHitIndex;

        _headTarget.transform.position = _fragments[_fragments.Length - 2].transform.position;

        _headSpring = _head.GetComponent<SpringJoint>();
        _headSpring.connectedBody = _headTarget;
        _startSpring = _headSpring.spring;


        var healthHandler = GetComponent<CentipedeHealthSystem>();
        if (healthHandler != null)
        {
            healthHandler.SubscribeToHitTakers();
        }
    }

    private void HandleStun()
    {
        _headSpring.spring = 0;
    }

    private async void HandleStunRecover()
    {
        var t = 0f;
        while (t < 1)
        {
            if (!gameObject)
                return;
            t += Time.deltaTime;
            _headSpring.spring = Mathf.Lerp(0, _startSpring, t);
            await Task.Yield();
        }
    }

    private void HandleDeath()
    {
        if (_dead)
            return;
        _dead = true;
        StartCoroutine(MakeDeathAnimation());
    }

    private IEnumerator MakeDeathAnimation()
    {
        var dissolveDuration = 5f;
        _headSpring.spring = 0;
        if (_lastFragmentHitIndex == _fragments.Length) _head.gameObject.AddComponent<Death>();
        else _fragments[_lastFragmentHitIndex].gameObject.AddComponent<Death>();
        yield return new WaitForSeconds(0.2f);
        for (var i = 1; i <= _fragments.Length; i++)
        {
            var upIndex = _lastFragmentHitIndex + i;
            var downIndex = _lastFragmentHitIndex - i;
            if (upIndex < _fragments.Length)
            {
                var d = _fragments[upIndex].gameObject.AddComponent<Death>();
                d.SetDissolveDuration(dissolveDuration);
            }
            if (upIndex == _fragments.Length)
            {
                var d = _head.gameObject.AddComponent<Death>();
                d.SetDissolveDuration(dissolveDuration);
            }
            if (downIndex >= 0)
            {
                var d = _fragments[downIndex].gameObject.AddComponent<Death>();
                d.SetDissolveDuration(dissolveDuration);
            }

            yield return new WaitForSeconds(0.3f);
        }
        Destroy(gameObject);
    }

    private void SetLastFragmentHitIndex(int i)
    {
        if (_dead) return;
        _lastFragmentHitIndex = i;
    }
}
