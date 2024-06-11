using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rotwang
{
    public enum EntryMode
    {
        DO_NOTHING, SLIDE, ZOOM, FADE
    }

    public enum Direction
    {
        NONE, UP, LEFT, RIGHT, DOWN
    }
    public class AnimationHelper : Singleton<AnimationHelper>{

        public static void Animate(GameObject obj, float duration)
        {
            if (obj != null)
            {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(rectTransform.DOScale(Vector3.zero, 0f));
                    sequence.Append(rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutBounce));
                    sequence.Join(rectTransform.DOAnchorPos(Vector2.zero, duration).SetEase(Ease.InOutQuint));
                    sequence.Play();
                }
            }
        }


        public static IEnumerator SlideIn(RectTransform rectTransform, Direction direction, float speed, Vector2? finalPosition, UnityEvent? OnEnd)
        {
            Vector2 startPosition = Vector2.zero, endPosition = Vector2.zero;

            switch (direction)
            {
                case Direction.UP:
                    startPosition = new Vector2(0, -rectTransform.rect.height * (1 - rectTransform.pivot.y));
                    endPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                    break;
                case Direction.RIGHT:
                    startPosition = new Vector2((-rectTransform.rect.width - Screen.width)*1.5f * (1 - rectTransform.pivot.x), 0);
                    endPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
                    break;
                case Direction.DOWN:
                    startPosition = new Vector2(0, Screen.height + rectTransform.rect.height * rectTransform.pivot.y);
                    endPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                    break;
                case Direction.LEFT:
                    startPosition = new Vector2(Screen.width + rectTransform.rect.width * rectTransform.pivot.x, 0);
                    endPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
                    break;
            }
            rectTransform.anchoredPosition = startPosition;

            Vector2 endPos = finalPosition ?? endPosition;

            yield return rectTransform.DOAnchorPos(endPos, speed).SetEase(Ease.InOutQuint).WaitForCompletion();

            rectTransform.anchoredPosition = endPos;

            OnEnd?.Invoke();
        }

        public static IEnumerator SlideOut(RectTransform rectTransform, Direction direction, float speed, Vector2? finalPosition, UnityEvent? OnEnd)
        {
            Vector2 startPosition = Vector2.zero, endPosition = Vector2.zero;

            switch (direction)
            {
                case Direction.UP:
                    startPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                    endPosition = new Vector2(0, Screen.height + rectTransform.rect.height * (1 - rectTransform.pivot.y));
                    break;
                case Direction.RIGHT:
                    startPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                    endPosition = new Vector2(Screen.width + rectTransform.rect.width * (1 - rectTransform.pivot.x), rectTransform.anchoredPosition.y);
                    break;
                case Direction.DOWN:
                    startPosition = new Vector2(0, rectTransform.anchoredPosition.y);
                    endPosition = new Vector2(0, -rectTransform.rect.height * (1 - rectTransform.pivot.y));
                    break;
                case Direction.LEFT:
                    startPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                    endPosition = new Vector2(-rectTransform.rect.width * (1 + rectTransform.pivot.x), rectTransform.anchoredPosition.y);
                    break;
            }

            rectTransform.anchoredPosition = startPosition;
            rectTransform.DOAnchorPos(endPosition, speed).SetEase(Ease.InOutQuint).OnComplete(() => {
                OnEnd?.Invoke();
            });
            yield return new WaitForSeconds(speed);
        }


        public static IEnumerator FadeIn(CanvasGroup canvasGroup, float speed, UnityEvent? OnEnd) {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            float time =0;
            while (time < 1)
            {
                canvasGroup.DOFade(1, time);
                //canvasGroup.alpha=Mathf.Lerp(0, 1, time);
                yield return null;
                time += Time.deltaTime * speed;
            }

            canvasGroup.alpha = 1;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeOut(CanvasGroup canvasGroup, float speed, UnityEvent? OnEnd)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            float time = 0;
            while (time < 1)
            {
                canvasGroup.DOFade(0f, time).OnComplete(() => canvasGroup.interactable = false);
                //canvasGroup.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
                time += Time.deltaTime * speed;
            }
            canvasGroup.alpha = 0;
            
            OnEnd?.Invoke();
        }

        public static IEnumerator ZoomIn(RectTransform rectTransform, float animationSpeed, UnityEvent? OnEnd)
        {
      
            float time = 0;

            while (time < 1)
            {
                rectTransform.DOScale(Vector2.one, animationSpeed);
                //rectTransform.localScale = Vector3.Lerp(Vector2.zero,Vector2.one, time);
                yield return null;
                time += Time.deltaTime * animationSpeed;
            }
            rectTransform.localScale = Vector2.one;
            OnEnd?.Invoke();

        }

        public static IEnumerator ZoomOut(RectTransform rectTransform, float animationSpeed, UnityEvent? OnEnd)
        {
            float time = 0;

            while (time < 1)
            {
                rectTransform.DOScale(Vector2.zero, animationSpeed);
                //rectTransform.localScale = Vector3.Lerp(Vector2.zero,Vector2.one, time);
                yield return null;
                time += Time.deltaTime * animationSpeed;
            }
            rectTransform.localScale = Vector2.zero;
            OnEnd?.Invoke();
        }
    }

}
