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

namespace cbs.pang.game.model
{

    public class SpriteFader : MonoBehaviour
    {

#region Field

        SpriteRenderer mRenderer;
        float mDelay;

#endregion


#region Public Method
        public void Set(SpriteRenderer render, float delay)
        {
            mRenderer = render;
            mDelay = delay;
        }

        public void FadeColor(Color color)
        {
            StopCoroutine("fadeCoroutine");
            StartCoroutine("fadeCoroutine", color);
        }

        public IEnumerator PingPongFade(Color color, float delay)
        {
            Color orgColor = mRenderer.color;
            float startTime = Time.time;
            while (Time.time - startTime <= mDelay)
            {
                mRenderer.color = Color.Lerp(orgColor, color, (Time.time - startTime) / mDelay);
                yield return null;
            }

            startTime = Time.time;

            while (Time.time - startTime <= mDelay)
            {
                mRenderer.color = Color.Lerp(color, orgColor, (Time.time - startTime) / mDelay);
                yield return null;
            }

            mRenderer.color = orgColor;
        }
#endregion

#region Private Method

        IEnumerator fadeCoroutine(Color color)
        {
            Color orgColor = mRenderer.color;
            float startTime = Time.time;

            while (Time.time - startTime <= mDelay)
            {
                mRenderer.color = Color.Lerp(orgColor, color, (Time.time - startTime) / mDelay);
                yield return null;
            }
            mRenderer.color = color;
        }

#endregion
    }
}
