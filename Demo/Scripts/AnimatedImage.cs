using UnityEngine;
using UnityEngine.UI;

namespace Plugins.EasyDiscord.Demo.Scripts {
    public class AnimatedImage : MonoBehaviour {
        [SerializeField] private Sprite[] spriteFrames;
        [SerializeField] private bool loop = true;
        [SerializeField] private int frameTime = 6;

        private Image _image;
        private int _index = 0;
        private int _frame = 0;

        private void Awake() {
            if (!TryGetComponent(out _image)) {
                _image = gameObject.AddComponent<Image>();
            }
            
            // Set the first frame
            _image.sprite = spriteFrames[0];
        }

        private void Update() {
            // If the frame is not the last one, increment the frame
            if (!loop && _index == spriteFrames.Length) return;
            _frame++;
            
            // If the frame is the last one, reset the frame
            if (_frame < frameTime) return;
            _image.sprite = spriteFrames[_index];
            _frame = 0;
            _index++;
            
            // If the index is the last one, reset the index
            if (_index < spriteFrames.Length) {
                return;
            }

            // If the animation is set to loop, reset the index
            if (loop) _index = 0;
        }
    }
}