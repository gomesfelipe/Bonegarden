using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cutscene1 : MonoBehaviour
{

    public TOD_Sky Sky;

    private float _rayleigh;
    private float _mie;
    private float _contrast;
    private float _directionality;

    // Use this for initialization
    void Awake()
    {
        _rayleigh = Sky.Atmosphere.RayleighMultiplier;
        _mie = Sky.Atmosphere.MieMultiplier;
        _contrast = Sky.Atmosphere.Contrast;
        _directionality = Sky.Atmosphere.Directionality;

        Sky.Atmosphere.RayleighMultiplier = 0;
        Sky.Atmosphere.MieMultiplier = 0.4f;
        Sky.Atmosphere.Contrast = 6f;
        Sky.Atmosphere.Directionality = 1f;
    }

    void Start()
    {
        StartCoroutine(Routine());
    }

    float GetDirection()
    {
        return Sky.Atmosphere.Directionality;
    }

    void SetDirection(float value)
    {
        Sky.Atmosphere.Directionality = value;
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(5f);
        DOTween.To(GetDirection, SetDirection, 0.72f, 8f);
    }
}
