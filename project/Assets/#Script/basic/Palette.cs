/*
    Copyright 2014 ColaBearStudio. Choi Ethan.

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using UnityEngine;
using System.Collections;

namespace cbs.pang.basic.color
{
    public enum Colors : int
    {
        TURQUOISE,
        EMERALD,
        PETERRIVER,
        MIDNIGHTBLUE,
        CARROT,
        ALIZARIN,
        ASBESTOS,
        WISTERIA,
        BLACK
    }
    public class Palette : MonoBehaviour
    {
        #region Singleton
        
        static Palette sInstance;

        public static Palette GetInstance
        {
            get
            {
                if (sInstance == null) sInstance = FindObjectOfType<Palette>();
                return sInstance;
            }
        }


        #endregion

        #region -------Public Field-------

        public Color[] colors;

        #endregion

        #region -------Private Field-------

        #endregion

        #region -------Default Method-------

        void Start()
        {
            sInstance = this;
        }

        #endregion

        #region -------Public Method-------
        public Color GetColor(Colors color)
        {
            return colors[(int)color];
        }

        public Color GetColor(int colorNum)
        {
            return colors[colorNum];
        }

        public Color GetExclusionRandomColor(Colors excColor, out Colors color)
        {
            int index = getRandomColorIndex();
            while ((Colors)index == excColor)
            {
                index = getRandomColorIndex();
            }
            color = (Colors)index;
            return colors[index];
        }

        public Color GetRandomColor(out Colors color)
        {
            int index = getRandomColorIndex();
            color = (Colors)index;
            return colors[index];
        }
        public Color GetRandomColor()
        {
            int index = getRandomColorIndex();
            return colors[index];
        }
        #endregion

        #region -------Private Method-------

        int getRandomColorIndex()
        {
            return Random.Range(0, colors.Length);
        }

        #endregion

        #region -------Property-------

        #endregion

    }
}
