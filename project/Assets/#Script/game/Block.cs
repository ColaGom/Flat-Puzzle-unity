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
using System.Collections.Generic;
using cbs.pang.basic.color;
using cbs.pang.game.model;

namespace cbs.pang.game
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class Block : PangObject
    {

        #region -------Public Field-------

        public enum State : int
        {
            EMPTY, // before Spawn  
            IDLE, // after Spawn
            MOVE,  
            SELECT,
            MATCH_WAIT,
            MATCH_END, // Crash Event -> WAIT
            HINT
        }

        #endregion

        #region -------Private Field-------

     //   SpriteRenderer mRenderer;
       // Colors mColor;
        BlockData mData;
      //  SpriteFader mFader;
        State mState;

        static Vector2 blockScale = new Vector2(Values.Block_Scale, Values.Block_Scale);
        static Vector2 selectedScale = blockScale * .8f;
        static MatchContainer container;

        #endregion

        #region -------Default Method-------
        /*
        void OnGUI()
        {
            Vector2 vec = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(vec.x, Screen.height - vec.y,50,80), string.Format("{0}\n{1}:{2}", mState.ToString(),_X,_Y));
        }*/

        void Start()
        {
            _Fader.Set(_Render, Values.Block_ChangeColorDuration);
            mState = State.EMPTY;
            if (!container) container = transform.parent.GetComponent<MatchContainer>();
        }
        

        #endregion

        #region -------Public Method-------

        public void Move(int x, int y)
        {
            if (mState == State.MOVE) return;
            StopCoroutine("scaleCoroutine");
            StartCoroutine(moveCoroutine(x, y, mState));
        }

        public void On()
        {
            StopAllCoroutines();
            mState = State.IDLE;
            transform.localScale = blockScale;
        }

        public void Spawn()
        {
            StopCoroutine("scaleCoroutine");
            transform.localPosition = new Vector2(_X, _Y);
            StartCoroutine("scaleCoroutine", blockScale);
            mState = State.IDLE;
        }

        public void Hide()
        {
            StopCoroutine("scaleCoroutine");
            StartCoroutine("scaleCoroutine", Vector2.zero);
        }

        public void OnHint()
        {
            if (mState == State.HINT) return;
            mState = State.HINT;

            StopCoroutine("hineCoroutine");
            StartCoroutine("hineCoroutine");
        }

        public void OffHint()
        {
            mState = State.IDLE;
            StopCoroutine("hineCoroutine");
            SetColor(CurrentColor);
        }

        public void Match()
        {
            if (mState == State.MATCH_WAIT) return;
            mState = State.MATCH_WAIT;

            StartCoroutine(matchCoroutine());
            //transform.localScale = Vector2.zero;
        }

        public void SetData(int x, int y)
        {
            mData = new BlockData(x, y);
        }

        public void SetData(BlockData data)
        {
            mData = data;
        }

        public void SetPosition(int x, int y)
        {
            _LocalPosition = new Vector2(x, y);
            mData.x = x;
            mData.y = y;
        }

        public void SetData(){
            mData.x = (int)transform.localPosition.x;
            mData.y = (int)transform.localPosition.y;
        }

        public void SetColor(Colors color)
        {
            CurrentColor = color;
            _Render.color = Palette.GetInstance.GetColor(color);
        }

        public void SetRandomColor()
        {
            _Fader.FadeColor(Palette.GetInstance.GetRandomColor(out CurrentColor));
        }

        public void SetRandomColor(bool fade)
        {
            if (!fade)
                _Render.color = Palette.GetInstance.GetRandomColor(out CurrentColor);
            else
                _Fader.FadeColor(Palette.GetInstance.GetRandomColor(out CurrentColor));
        }

        public void Select(out Block block)
        {
            if (mState == State.SELECT)
            {
                block = null;
                return;
            }
            block = this;
            mState = State.SELECT;
            StartCoroutine(scaleCoroutine(selectedScale, .1f));
        }

        public void DeSelect(out Block block)
        {
            block = null;
            mState = State.IDLE;
            StartCoroutine(scaleCoroutine(blockScale, .1f));
        }

        public void Wait()
        {
            transform.localScale = blockScale;
        }

        #endregion

        //static float speed = 10f;
        #region -------Private Method-------

        IEnumerator hineCoroutine()
        {
            while(mState == State.HINT)
            {
                yield return StartCoroutine(_Fader.PingPongFade(Color.white, .1f));
            }
            SetColor(CurrentColor);
        }

        IEnumerator matchCoroutine()
        {
            _Fader.FadeColor(Color.white);
            yield return StartCoroutine(scaleCoroutine(Vector2.zero));
            mState = State.MATCH_END;
        }
        IEnumerator moveCoroutine(int x,int y, State pre)
        {
            Vector2 startPos = _LocalPosition;
            Vector2 endPos = new Vector2(x, y);
            float startTime = Time.time;
            //float duration = Vector2.Distance(startPos, endPos) / speed;
            float duration = .2f;
            mState = State.MOVE;

            while(Time.time - startTime <= duration)
            {
                _LocalPosition = Vector2.Lerp(startPos, endPos, (Time.time - startTime) / duration);
                yield return null;
            }

            mState = pre;
            _LocalPosition = endPos;
            SetData();
            container.FinishedMoveEvent(this);
        }

        IEnumerator scaleCoroutine(Vector2 endVec)
        {
            float startTime = Time.time;
            Vector2 orgVec = transform.localScale;

            while (Time.time - startTime <= Values.Block_SpawnDuration)
            {
                transform.localScale = Vector2.Lerp(orgVec, endVec, (Time.time - startTime) / Values.Block_SpawnDuration);
                yield return null;
            }

            transform.localScale = endVec;
        }
        IEnumerator scaleCoroutine(Vector2 endVec, float duration)
        {
            float startTime = Time.time;
            Vector2 orgVec = transform.localScale;

            while (Time.time - startTime <= Values.Block_SpawnDuration)
            {
                transform.localScale = Vector2.Lerp(orgVec, endVec, (Time.time - startTime) / Values.Block_SpawnDuration);
                yield return null;
            }

            transform.localScale = endVec;
        }


        #endregion

        #region -------Property-------

        public BlockData _BlockData
        {
            get { return mData; }
        }

        public State _State
        {
            get { return mState; }
        }

        public int _X
        {
            get { return mData.x; }
        }

        public int _Y
        {
            get { return mData.y; }
        }

        public Vector2 _LocalPosition
        {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }

        #endregion
    }
}
