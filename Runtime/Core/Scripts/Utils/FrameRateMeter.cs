using System;
using TMPro;
using UnityEngine;

namespace OneDay.Core.Utils
{
    public class FrameRateMeter : MonoBehaviour
    {
        public enum Mode
        {
            Short,
            Detailed,
            Hidden
        }
        
        [Header("Display Settings")] [SerializeField]
        private TextMeshProUGUI fpsText;

        [Tooltip("How often to update the display")] [SerializeField]
        private float updateInterval = 0.5f;

        [SerializeField] private Mode mode; // Show more detailed performance info
        [SerializeField] private Color goodFpsColor = Color.green;
        [SerializeField] private Color warningFpsColor = Color.yellow;
        [SerializeField] private Color badFpsColor = Color.red;
        [SerializeField] private int goodFpsThreshold = 60;
        [SerializeField] private int warningFpsThreshold = 30;

        private float accumulatedFrameTime = 0f;
        private int framesAccumulated = 0;
        private float timeLeft;

        private int numberOfModes;
        private void Start()
        {
            timeLeft = updateInterval;
            numberOfModes = Enum.GetValues(typeof(Mode)).Length;
        }

        public void ToggleMode() => mode = (Mode)(((int)mode + 1) % numberOfModes);

        private void Update()
        {
            timeLeft -= Time.deltaTime;
            accumulatedFrameTime += Time.unscaledDeltaTime;
            framesAccumulated++;

            // Update display when the interval has passed
            if (timeLeft <= 0.0f)
            {
                // Calculate average FPS over the interval
                float fps = framesAccumulated / accumulatedFrameTime;
                float msPerFrame = 1000.0f * accumulatedFrameTime / framesAccumulated;

                switch (mode)
                {
                    case Mode.Short:
                        fpsText.enabled = true;
                        fpsText.text =  $"{fps:0.} <size=50%>fps</size>";
                        UpdateColor(fps);
                        break;
                    case Mode.Detailed:
                        fpsText.enabled = true;
                        fpsText.text = $"{fps:0.} <size=50%>fps</size> ({msPerFrame:0.0} <size=50%>ms</size>)";
                        UpdateColor(fps);
                        break;
                    
                    case Mode.Hidden:
                        fpsText.enabled = false;
                        break;
                }

                // Reset for next interval
                timeLeft = updateInterval;
                accumulatedFrameTime = 0f;
                framesAccumulated = 0;
            }
        }

        private void UpdateColor(float fps)
        {
            // Update color based on performance thresholds
            if (fps >= goodFpsThreshold)
            {
                fpsText.color = goodFpsColor;
            }
            else if (fps >= warningFpsThreshold)
            {
                fpsText.color = warningFpsColor;
            }
            else
            {
                fpsText.color = badFpsColor;
            }
        }
    }
}
