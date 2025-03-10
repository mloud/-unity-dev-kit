using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneDay.Core.Extensions
{
    public static class GameObjectExtensions
    {
        public static Transform SetScaleX(this Transform transform, float scaleX)
        {
            var scale = transform.localScale;
            scale.x = scaleX;
            transform.transform.localScale = scale;
            return transform;
        }
        
        public static Transform SetScaleY(this Transform transform, float scaleY)
        {
            var scale = transform.localScale;
            scale.y = scaleY;
            transform.transform.localScale = scale;
            return transform;
        }

        public static GameObject SetScaleX(this GameObject gameObject, float scaleX) =>
            SetScaleX(gameObject.transform, scaleX).gameObject;
        public static GameObject SetScaleY(this GameObject gameObject, float scaleY) =>
            SetScaleY(gameObject.transform, scaleY).gameObject;
        public static GameObject SetScaleZ(this GameObject gameObject, float scaleZ) =>
            SetScaleZ(gameObject.transform, scaleZ).gameObject;

        public static Transform SetScaleZ(this Transform transform, float scaleZ)
        {
            var scale = transform.localScale;
            scale.z = scaleZ;
            transform.transform.localScale = scale;
            return transform;
        }
        
        public static async UniTask SetVisibleWithFade(this Component component, bool isVisible, float duration,
            bool includeChildren,  CancellationToken token = default)
        {
            await SetVisibleWithFade(component.gameObject, isVisible, duration, includeChildren, token);
        }
        
        public static async UniTask SetVisibleWithFade(this GameObject go, bool isVisible, float duration,
            bool includeChildren, CancellationToken token = default)
        {
            const Ease ease = Ease.InSine;
            CanvasGroup canvasGroup = null;
            if (isVisible)
                go.SetActive(true);
            if (includeChildren)
            { 
                canvasGroup = go.GetComponent<CanvasGroup>();

                if (go.transform.childCount > 0)
                {
                    if (canvasGroup == null)
                    {
                        Debug.LogWarning(
                            $"GameObject {go.name} does not have CanvasGroup to fade its children - adding one");
                        canvasGroup = go.AddComponent<CanvasGroup>();
                    }
                }
            }

            float targetAlpha = isVisible ? 1 : 0;
            if (canvasGroup != null)
            {
                if (duration > 0)
                {
                    await canvasGroup.DOFade(targetAlpha, duration)
                        .SetEase(ease)
                        .ToUniTask(cancellationToken: token);
                }
                else
                {
                    canvasGroup.alpha = targetAlpha;
                }
            }
            else
            {
                var image = go.GetComponent<Image>();
                if (image != null)
                {
                    if (duration > 0)
                    {
                        await image.DOFade(targetAlpha, duration)
                            .SetEase(ease)
                            .ToUniTask(cancellationToken: token);
                    }
                    else
                    {
                        image.SetAlpha(targetAlpha);
                    }
                }
                else
                {
                    var spriteRenderer = go.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        if (duration > 0)
                        {
                            await spriteRenderer.DOFade(targetAlpha, duration)
                                .SetEase(ease)
                                .ToUniTask(cancellationToken: token);
                        }
                        else
                        {
                            spriteRenderer.SetAlpha(targetAlpha);
                        }
                    }
                    else
                    {
                        var textUi = go.GetComponent<TextMeshProUGUI>();
                        if (textUi != null)
                        {
                            if (duration > 0)
                            {
                                await textUi.DOFade(targetAlpha, duration)
                                    .SetEase(ease)
                                    .ToUniTask(cancellationToken: token);
                            }
                            else
                            {
                                textUi.SetAlpha(targetAlpha);
                            }
                        }
                        else
                        {
                            var text = go.GetComponent<TextMeshPro>();
                            if (text != null)
                            {
                                if (duration > 0)
                                {
                                    await text.DOFade(targetAlpha, duration)
                                        .SetEase(ease)
                                        .ToUniTask(cancellationToken: token);
                                }
                                else
                                {
                                    text.SetAlpha(targetAlpha);
                                }
                            }
                        }
                    }
                }
            }

            if (!isVisible)
                go.SetActive(false);
        }
    }
}