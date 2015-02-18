using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MT3
{
    public class Settings
    {
        private string _text;
        private int _number;


        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        //CCD
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _height;
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }


        public Settings()
        {
            _text = "Text";
            _number = 0;

            _width = 640;
            _height = 480;
        }
    }
}
