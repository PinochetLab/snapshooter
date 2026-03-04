using Players;
using Snap;
using UnityEngine;
using Zenject;

namespace View
{
    public class SnapViewer : Body
    {
        [SerializeField] private Camera snapCamera;
        
        [SerializeField] private Tolerance tolerance;

        [Inject] private Player _playerMc;

        private ViewTransform _startVt, _endVt;

        public bool IsMatched()
        {
            return ViewTransform.AreMatched(ViewTransform, _playerMc.ViewTransform, tolerance);
        }

        public float GetTime()
        {
            return ViewTransform.TimeDifference(_playerMc.ViewTransform, ViewTransform, ViewTransformSpeed.Snap);
        }

        private Texture2D TakePictureTexture()
        {
            var renderTexture = snapCamera.targetTexture;
            snapCamera.targetTexture = renderTexture;
            snapCamera.Render();
            RenderTexture.active = renderTexture;
            var size = SnapMaster.SnapshotSize;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false, true);
            var srcX = (Screen.width - size) / 2f;
            var srcY = (Screen.height - size) / 2f;
            texture.ReadPixels(new Rect(srcX, srcY, size, size), 
                0, 0);
            texture.Apply();
            return texture;
        }

        public Sprite TakePicture()
        {
            var texture = TakePictureTexture();
            return Sprite.Create(
                texture,
                new Rect(0.0f, 0.0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100.0f
            );
        }

        public void BeforeAlign()
        {
            _playerMc.DisableMotion();
            _startVt = _playerMc.ViewTransform;
            _endVt = ViewTransform;
        }

        public void AfterAlign()
        {
            _playerMc.ViewTransform = _endVt;
            _playerMc.EnableMotion();
        }

        public void ProgressAlign(float t)
        {
            _playerMc.ViewTransform = ViewTransform.Lerp(_startVt, _endVt, t);
        }
    }
}