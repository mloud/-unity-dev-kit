using System;
using Cysharp.Threading.Tasks;
using OneDay.Core;
using OneDay.Core.Modules.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Modules.Ui
{
    public class LoadingLayer : UiElement, ILoading, IService
    {
        public enum ProgressInfoStyle
        {
            None,
            Percentage,
            Fraction
        }

        [SerializeField] private TextMeshProUGUI defaultLoadingText;
        [SerializeField] private TextMeshProUGUI customLoadingText;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI counterLabel;
        [SerializeField] private ProgressInfoStyle defaultProgressInfoStyle;

        public void SetProgress(int current, int total, string text)
        {
            SetLoadingStepText(text);
            SetProgressBar(current, total);
            SetProgressInfo(current, total);
        }

        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public async UniTask Show() => await Show(true);
        public async UniTask Hide() => await Hide(true);

        private void SetProgressBar(int current, int total)
        {
            fillImage.fillAmount = (float)current / total;
        }

        private void SetLoadingStepText(string text)
        {
            if (text != null)
            {
                customLoadingText.text = text;
                customLoadingText.enabled = true;
                defaultLoadingText.enabled = false;
            }
            else
            {
                customLoadingText.enabled = false;
                defaultLoadingText.enabled = true;
            }
        }
        
        protected virtual void SetProgressInfo(int current, int total)
        {
            switch (defaultProgressInfoStyle)
            {
                case ProgressInfoStyle.None:
                    counterLabel.enabled = false;
                    break;
                case ProgressInfoStyle.Fraction:
                    counterLabel.text = $"{current} / {total}";
                    break;
                case ProgressInfoStyle.Percentage:
                    counterLabel.text = $"{(int)Mathf.Ceil(current*100.0f/total)}%";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}