using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

public class Cutscene1 : MonoBehaviour
{

    public TextMeshPro GodText;
    public Transform CorridorTransform;
    public Transform PlayerTransform;
    public Transform StairsTransform;

    private bool _hasTriggered;
    private Transform _textLookatTransform;

    void Awake()
    {
        _textLookatTransform = GodText.transform.parent;
    }

    IEnumerator Routine()
    {
        yield return new WaitForSeconds(1f);
        GodText.text = "º _ º\n\noh hi";
        yield return new WaitForSeconds(3.5f);
        GodText.text = "º _ º\n\nyou've arrived.";
        yield return new WaitForSeconds(2.5f);
        GodText.text = "º _ º\n\n";
        yield return new WaitForSeconds(1f);
        GodText.text = "º _ º\n\nthis way.";
        yield return new WaitForSeconds(1f);
        CorridorTransform.DOLocalMoveY(30f, 2f).SetEase(Ease.Linear).OnComplete(delegate
        {
            CorridorTransform.DOLocalMoveY(34f, 2f).SetEase(Ease.OutElastic).OnComplete(delegate
            {
                GodText.text = "º _ º\n\n";
                StairsTransform.DOLocalRotate(new Vector3(90, 0, 0), 2f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine);
            });
        });
    }

    void Update()
    {
        if (GodText.renderer.isVisible && !_hasTriggered)
        {
            _hasTriggered = true;
            StartCoroutine(Routine());
        }

        _textLookatTransform.LookAt(PlayerTransform);
    }
}
