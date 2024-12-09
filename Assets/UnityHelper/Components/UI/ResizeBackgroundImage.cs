using System;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

namespace UnityHelper
{
    public class ResizeBackgroundImage : MonoBehaviour
    {
        [Foldout("Info")]
        [SerializeField,Variants(0,1)]
        protected int _match = 1;
        [SerializeField]
        protected Vector2 _resolutionReference = new (1080,1920);
        [Foldout("Reference")]
        [SerializeField]
        protected Image _image;
        [Foldout("Data")]
        [SerializeField]
        protected ImageInfo _imageInfo;
        [EndFoldout]
        
        private Vector2 _screenCalculate;
        
        //
        protected RectTransform _imageRtf;

        protected virtual void Awake()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
            
            if(!_image)
                return;
            _imageRtf = _image.rectTransform;
            CalculateScreenSize();
            ApplySpriteResolution(_imageInfo);
            
        }
        
        private void CalculateScreenSize()
        {
            var screenHeightCalculate = 0f;
            var screenWidthCalculate  = 0f;
            
            if (_match - 1 == 0)
            {
                screenHeightCalculate = _resolutionReference.y;
                var ratio = screenHeightCalculate / Screen.height;
                screenWidthCalculate = Screen.width * ratio;
            }
            else
            {
                screenWidthCalculate = _resolutionReference.x;
                var ratio = screenWidthCalculate / Screen.width;
                screenHeightCalculate = Screen.height * ratio;
            }
            _screenCalculate = new Vector2(screenWidthCalculate, screenHeightCalculate);
        }

        public virtual void ApplySpriteResolution(ImageInfo imageInfo)
        {
            if(!_image)
                return;
            var ratioX = imageInfo.resolution.x / _screenCalculate.x;
            var ratioY = imageInfo.resolution.y / _screenCalculate.y;
            float width = 0;
            float height = 0;
            if (ratioX < ratioY)
            {
                width = _screenCalculate.x;
                height = imageInfo.resolution.y / ratioX;
            }
            else
            {
                height = _screenCalculate.y;
                width = imageInfo.resolution.x / ratioY;
            }
            _imageRtf.sizeDelta = new Vector2(width, height);
            _image.sprite = imageInfo.sprite;
        }
        
    }

    [Serializable]
    public struct ImageInfo
    {
        public Vector2 resolution;
        public Sprite sprite;
    }
    
}