using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialRecognition
{
    public class Face
    {
        private int _left;
        private int _top;
        private int _width;
        private int _height;
        private Guid _faceId;

        public Guid FaceId
        {
            get { return _faceId; }
            set { _faceId = value; }
        }

        public System.Windows.Int32Rect UIRect
        {
            get
            {
                return new System.Windows.Int32Rect(Left, Top, Width, Height);
            }
        }

        public Face(int x, int y, int width, int height)
        {
            this._left = x;
            this._top = y;
            this._width = width;
            this._height = height;
        }

        public Face(int left, int top, int width, int height, Guid faceId) : this(left, top, width, height)
        {
            _faceId = faceId;
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

    }

}
