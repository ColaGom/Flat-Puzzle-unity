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
using cbs.pang.basic.color;

namespace cbs.pang.game.model
{
    public class PangObject : MonoBehaviour
    {

        #region Field

        public Colors CurrentColor;
        SpriteRenderer mRenderer;
        SpriteFader mFader;

        #endregion

        #region Method

        void Awake()
        {
            mRenderer = GetComponent<SpriteRenderer>();
            mFader = gameObject.AddComponent<SpriteFader>();
        }

        #endregion

        #region Property

        public SpriteRenderer _Render
        {
            get { return mRenderer; }
            set { mRenderer = value; }
        }

        public SpriteFader _Fader
        {
            get { return mFader; }
            set { mFader = value; }
        }

        #endregion
    }
}
