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
using System;
using System.Collections;
using System.Collections.Generic;
using cbs.pang.basic.color;
using cbs.pang.game.model;

namespace cbs.pang.game
{
    public partial class MatchContainer : MonoBehaviour
    {

        #region -------Public Field-------

        public Block[,] blockList;
        public GameObject blockPrefab;
        public Camera camera; // 매치화면을 보여줄 카메라 컴포넌트.
        public int width; // 가로 블록 개수
        public int height; // 세로 블럭 개수
        public float pad; // 카메라 여백 값.

        #endregion

        #region -------Private Field-------

        Block firBlock, secBlock;
        Block hintBlock;
        Vector2 preMousePos;
        float moveDistance = .7f;
        float preMatchTime;
        float hintDealy = 3f;
        bool enable = false;

        #endregion

        #region -------Default Method-------

        void Start()
        {
            blockList = new Block[width, height];
            float offset = Values.Block_Scale / 2;

            transform.position = new Vector2(-(width / 2f) + offset, -(height / 2f) + offset);

            if (Screen.width > Screen.height)
            {
                float hRatio = 1; // height / height = 1
                float wRatio = (float)Screen.width / Screen.height;

                float hViewSize = height / (hRatio * 2);
                float wViewSize = width / (wRatio * 2);

                Debug.Log("hviewSize :" + hViewSize);
                Debug.Log("wViewSize :" + wViewSize);

                camera.orthographicSize = hViewSize >= wViewSize ? hViewSize : wViewSize;
                camera.orthographicSize += pad;
            }
            else if (Screen.width < Screen.height)
            {
                float hRatio = (float)Screen.height / Screen.width;
                float wRatio = 1;

                float hViewSize = width / (wRatio * 2);
                float wViewSize = height / (hRatio * 2);

                camera.orthographicSize = hViewSize >= wViewSize ? hViewSize : wViewSize;
                camera.orthographicSize += pad;
            }
            else
                camera.orthographicSize = width / 2f + pad;

            InitializeBlock();
        }


        void Update()
        {
            if (enable)
            {
                if (Time.time - preMatchTime >= hintDealy)
                {
                    preMatchTime = Time.time;
                    if (hintBlock && checkMatchAtBlock(hintBlock))
                    {
                        hintBlock.OnHint();
                    }
                    else
                    {
                        setHintBlock();
                        hintBlock.OnHint();
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 pos = ScreenToWorldPoint(Input.mousePosition);

                    Debug.Log(pos);

                    if (pos.x > -(width / 2f) && pos.x < width / 2f && pos.y > -(height / 2f) && pos.y < height / 2f)
                    {
                        int x = (int)(pos.x + width / 2f);
                        int y = (int)(pos.y + height / 2f);
                        if (!checkClickAble(x)) return;
                        clickedEvent(x, y);
                    }
                }

                if (firBlock && !secBlock && Input.GetMouseButton(0))
                {
                    Vector2 pos = ScreenToWorldPoint(Input.mousePosition);

                    if (preMousePos.x - pos.x < -moveDistance && firBlock._X < width - 1 && checkClickAble(firBlock._X + 1))
                    {
                        blockList[firBlock._X + 1, firBlock._Y].Select(out secBlock);
                        completeSelect();
                    }
                    else if (preMousePos.x - pos.x > moveDistance && firBlock._X > 0 && checkClickAble(firBlock._X - 1))
                    {
                        blockList[firBlock._X - 1, firBlock._Y].Select(out secBlock);
                        completeSelect();
                    }
                    else if (preMousePos.y - pos.y < -moveDistance && firBlock._Y < height - 1 && checkClickAble(firBlock._X))
                    {
                        blockList[firBlock._X, firBlock._Y + 1].Select(out secBlock);
                        completeSelect();
                    }
                    else if (preMousePos.y - pos.y > moveDistance && firBlock._Y > 0 && checkClickAble(firBlock._X))
                    {
                        blockList[firBlock._X, firBlock._Y - 1].Select(out secBlock);
                        completeSelect();
                    }
                }

                if (Input.GetMouseButtonUp(0) && firBlock && secBlock)
                {
                    completeSelect();
                }

            }
            // Input Click -> Get Position -> Block Enable Check -> First? Second?
        }


        #endregion

        #region -------Public Method-------

        public void OnContainer()
        {
            enable = true;
            AllBlockPositionReset();
            AllBlockSpawn();
            AllBlockColorSet();
            StartCoroutine(matchDownCoroutine());
            preMatchTime = Time.time;
        }

        public void OffContainer()
        {
            enable = false;
            StopAllCoroutines();
            AllBlockHide();
        }

        public void InitializeBlock()
        {
            for (int x = 0; x < width; ++x)
            {
                //TODO : check match and reset color
                for (int y = 0; y < height; ++y)
                {
                    GameObject go = Instantiate(blockPrefab) as GameObject;
                    go.transform.parent = transform;
                    go.name = "block";
                    go.transform.localPosition = new Vector2(x, y);

                    Block block = go.GetComponent<Block>();
                    blockList[x, y] = block;

                    block.SetData(x, y);
                }
            }
        }

        public void AllBlockHide()
        {
            if (blockList == null) return;

            foreach (Block block in blockList)
            {
                block.StopAllCoroutines();
                block.Hide();
            }
        }

        public void AllBlockSpawn()
        {
            if (blockList == null) return;

            foreach (Block block in blockList)
                block.Spawn();
        }

        public void AllBlockColorSet()
        {
            if (blockList == null) return;

            foreach (Block block in blockList)
            {
                block.SetRandomColor();
                while (checkMatch(block)) block.SetRandomColor();
            }
        }

        public void FinishedMoveEvent(Block block)
        {
            if (findMatchAtBlock(block))
            {
                matchProcess();
                //emptyDown();
            }
        }
        public void AllBlockPositionReset()
        {
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y) blockList[x, y].SetPosition(x, y);
        }

        #endregion

        #region -------Private Method-------

        void completeSelect()
        {
            StartCoroutine(swapCoroutine());
            firBlock = null;
            secBlock = null;
        }

        /// <summary>
        /// checked block click able at line(x)
        /// </summary>
        /// <param name="x">line</param>
        bool checkClickAble(int x)
        {
            for (int y = 0; y < height; ++y)
                if (blockList[x, y]._State == Block.State.MATCH_WAIT || blockList[x, y]._State == Block.State.MOVE) return false;

            return true;
        }

        /// <summary>
        /// All Blocks in MatchContainer Color Set 
        /// </summary>
        /// <param name="delay"> Between Blocks Delay </param>
        /// <returns></returns>
        IEnumerator AllBlockColorSetWithDelay(float delay)
        {
            if (blockList == null) yield break;

            foreach (Block block in blockList)
            {
                block.SetRandomColor();
                while (checkMatch(block)) block.SetRandomColor();
                yield return new WaitForSeconds(delay);
            }
        }

        float swapTime = .1f;
        IEnumerator swapCoroutine()
        {
            Block a = firBlock;
            Block b = secBlock;

            if (!checkClickAble(a._X) || !checkClickAble(b._X)) //before, check able swap
            {
                a.DeSelect(out a);
                b.DeSelect(out b);
                yield break;
            }

            Vector2 firPos = a._LocalPosition;
            Vector2 secPos = b._LocalPosition;

            float startTime = Time.time;


            while (Time.time - startTime <= swapTime)
            {
                a._LocalPosition = Vector2.Lerp(firPos, secPos, (Time.time - startTime) / swapTime);
                b._LocalPosition = Vector2.Lerp(secPos, firPos, (Time.time - startTime) / swapTime);
                yield return null;
            }

            a._LocalPosition = secPos;
            b._LocalPosition = firPos;

            swap(a, b);

            yield return new WaitForSeconds(.1f);

            bool matchedA = findMatchAtBlock(a);
            bool matchedB = findMatchAtBlock(b);

            if (!matchedA && !matchedB) // Don't Matched Block!!
            {
                startTime = Time.time;

                while (Time.time - startTime <= swapTime)
                {
                    a._LocalPosition = Vector2.Lerp(secPos, firPos, (Time.time - startTime) / swapTime);
                    b._LocalPosition = Vector2.Lerp(firPos, secPos, (Time.time - startTime) / swapTime);
                    yield return null;
                }

                a._LocalPosition = firPos;
                b._LocalPosition = secPos;

                swap(a, b);
            }

            if (!matchedA)
                a.DeSelect(out a);
            if (!matchedB)
                b.DeSelect(out b);

            if (matchList.Count != 0)
            {
                matchProcess();
            }
        }


        void matchProcess()
        {
            if (hintBlock && hintBlock._State == Block.State.HINT) hintBlock.OffHint();
            preMatchTime = Time.time;

            foreach (MatchData data in matchList)
            {
                int x = data.x;
                int y = data.y;
                int length = data.length;

                if (blockList[x, y]._State == Block.State.MATCH_END)
                {
                    continue;
                }

                // score ++ 

                switch (data.direction)
                {
                    case MatchData.Direction.LEFT:
                        for (int i = 0; i < length; ++i) blockList[x - i, y].Match();
                        break;
                    case MatchData.Direction.RIGHT:
                        for (int i = 0; i < length; ++i) blockList[x + i, y].Match();
                        break;
                    case MatchData.Direction.TOP:
                        for (int i = 0; i < length; ++i) blockList[x, y + i].Match();
                        break;
                    case MatchData.Direction.BOTTOM:
                        for (int i = 0; i < length; ++i) blockList[x, y - i].Match();
                        break;
                }
            }
            matchList.Clear();
        }

        IEnumerator matchCoroutine()
        {
            foreach (MatchData data in matchList)
            {
                int x = data.x;
                int y = data.y;
                int length = data.length;

                switch (data.direction)
                {
                    case MatchData.Direction.LEFT:
                        for (int i = 0; i < length; ++i) blockList[x - i, y].Hide();
                        break;
                    case MatchData.Direction.RIGHT:
                        for (int i = 0; i < length; ++i) blockList[x + i, y].Hide();
                        break;
                    case MatchData.Direction.TOP:
                        for (int i = 0; i < length; ++i) blockList[x, y + i].Hide();
                        break;
                    case MatchData.Direction.BOTTOM:
                        for (int i = 0; i < length; ++i) blockList[x, y - i].Hide();
                        break;
                }
            }
            yield return null;
        }

        void clickedEvent(int x, int y)
        {
            if (blockList[x, y]._State == Block.State.MOVE || blockList[x, y]._State == Block.State.MATCH_END) return;
            if (!firBlock)
            {
                preMousePos = ScreenToWorldPoint(Input.mousePosition);
                blockList[x, y].Select(out firBlock);
            }
            else if (!secBlock && firBlock != blockList[x, y])
            {
                int disX = firBlock._X - x;
                int disY = firBlock._Y - y;


                if (Mathf.Abs(disX) > 1 || Mathf.Abs(disY) > 1)
                {
                    firBlock.DeSelect(out firBlock);
                    return;
                }

                blockList[x, y].Select(out secBlock);
            }
            else
            {
                if (firBlock) firBlock.DeSelect(out firBlock);
                if (secBlock) secBlock.DeSelect(out secBlock);
            }
        }

        Vector2 ScreenToWorldPoint(Vector2 pos)
        {
            return Camera.main.ScreenToWorldPoint(pos);
        }

        void refresh()
        {

        }

        void swap()
        {
            blockList[firBlock._X, firBlock._Y] = secBlock;
            blockList[secBlock._X, secBlock._Y] = firBlock;

            firBlock.SetData();
            secBlock.SetData();
        }

        void swap(Block a, Block b)
        {
            blockList[a._X, a._Y] = b;
            blockList[b._X, b._Y] = a;

            a.SetData();
            b.SetData();
        }

        void swap(int ax, int ay, int bx, int by)
        {
            Block tmp = blockList[ax, ay];
            blockList[ax, ay] = blockList[bx, by];
            blockList[bx, by] = tmp;

        }

        /// <summary>
        /// match blocks move
        /// </summary>
        void matchDown()
        {
            for (int x = 0; x < width; ++x)
            {
                List<Block> stack = new List<Block>();

                for (int y = 0; y < height; ++y)
                {
                    Block block = blockList[x, y];

                    if (block._State == Block.State.MATCH_END)
                    {
                        if (!stack.Contains(block))
                        {
                            stack.Add(block);
                            block._LocalPosition = new Vector2(x, height);
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        swap(x, y, x, y - stack.Count);
                        block.Move(x, y - stack.Count);
                    }
                }

                for (int i = 0; i < stack.Count; ++i)
                {
                    Block block = blockList[x, height - i - 1];

                    block.On();
                    block.SetRandomColor();
                    block.Move(x, height - i - 1);
                }
            }
        }
        /// <summary>
        /// Is Recursive Method 
        /// Find match end blocks and swap,move 
        /// Call OnContainer() and every frame
        /// </summary>
        /// <returns></returns>
        IEnumerator matchDownCoroutine()
        {
            bool changed = false;
            for (int x = 0; x < width; ++x)
            {
                List<Block> stack = new List<Block>();

                for (int y = 0; y < height; ++y)
                {
                    Block block = blockList[x, y];

                    if (block._State == Block.State.MATCH_END)
                    {
                        if (!stack.Contains(block))
                        {
                            stack.Add(block);
                            block._LocalPosition = new Vector2(x, height);
                        }
                    }

                    else if (stack.Count != 0)
                    {
                        swap(x, y, x, y - stack.Count);
                        block.Move(x, y - stack.Count);
                        changed = true;
                    }
                }

                for (int i = 0; i < stack.Count; ++i)
                {
                    Block block = blockList[x, height - i - 1];

                    block.On();
                    block.SetRandomColor();
                    block.Move(x, height - i - 1);
                    changed = true;
                }
            }

            if (changed) checkCanMatch();

            yield return null;

            StartCoroutine(matchDownCoroutine());

        }

        /// <summary>
        /// call count only one
        /// </summary>
        void checkCanMatch()
        {
            StopCoroutine("checkCanMatchCoroutine");
            StartCoroutine("checkCanMatchCoroutine");
        }

        IEnumerator checkCanMatchCoroutine()
        {
            while (isMoving()) yield return null;

            foreach (Block block in blockList)
            {
                if (checkMatchAtBlock(block))
                {
                    if (hintBlock && hintBlock._State != Block.State.HINT)
                        hintBlock = block;
                    yield break;
                }
            }

            AllBlockColorSet();
        }

        void setHintBlock()
        {
            foreach (Block block in blockList)
            {
                if (checkMatchAtBlock(block))
                {
                    hintBlock = block;
                }
            }
        }

        bool isMoving()
        {
            foreach (Block block in blockList)
            {
                if (block._State == Block.State.MOVE) return true;
            }

            return false;
        }

        List<MatchData> matchList = new List<MatchData>();
        bool findMatchAtIDLE()
        {
            bool match = false;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {

                }
            }
            return match;
        }

        //check order : left -> right -> top -> bottom
        bool checkMatchAtBlock(Block block)
        {
            /*
            //left
            if (x > 0 && y < height - 2 && CompareColor(block, -1, 1, -1, 2)) return true;
            if (x > 0 && y > 1 && CompareColor(block, -1, -1, -1, -2)) return true;
            if (x > 2 && CompareColor(block, -2, 0, -3, 0)) return true;
            //right
            if (x < width - 1 && y < height - 2 && CompareColor(block, 1, 1, 1, 2)) return true;
            if (x < width - 1 && y > 1 && CompareColor(block, 1, -1, 1, -2)) return true;
            if (x < width - 3 && CompareColor(block, 2, 0, 3, 0)) return true;
            //top
            if (x < width - 2 && y < height - 1 && CompareColor(block, 1, 1, 2, 1)) return true;
            if (x > 1 && y < height - 1 && CompareColor(block, -1, 1, -2, 1)) return true;
            if (y < height - 3 && CompareColor(block, 0, 2, 0, 3)) return true;
            //bottom
            if (x < width - 2 && y > 0 && CompareColor(block, 1, -1, 2, -1)) return true;
            if (x > 1 && y > 0 && CompareColor(block, -1, -1, -2, -1)) return true;
            if (y > 2 && CompareColor(block, 0, -2, 0, -3)) return true;
             * */

            //left
            if (CompareColor(block, -1, 1, -1, 2)) return true;
            if (CompareColor(block, -1, -1, -1, -2)) return true;
            if (CompareColor(block, -2, 0, -3, 0)) return true;
            //right
            if (CompareColor(block, 1, 1, 1, 2)) return true;
            if (CompareColor(block, 1, -1, 1, -2)) return true;
            if (CompareColor(block, 2, 0, 3, 0)) return true;
            //top
            if (CompareColor(block, 1, 1, 2, 1)) return true;
            if (CompareColor(block, -1, 1, -2, 1)) return true;
            if (CompareColor(block, 0, 2, 0, 3)) return true;
            //bottom
            if (CompareColor(block, 1, -1, 2, -1)) return true;
            if (CompareColor(block, -1, -1, -2, -1)) return true;
            if (CompareColor(block, 0, -2, 0, -3)) return true;

            return false;
        }

        bool findMatchAtBlock(Block block)
        {
            if (block._State == Block.State.MATCH_END) return false;
            bool match = false;
            //Colors color = block.CurrentColor;

            bool hor, ver, left, top, right, bottom;

            hor = CompareColorInHorizontal(block);
            ver = CompareColorInVertical(block);
            left = CompareColorInPattern(MatchData.Direction.LEFT, block);
            top = CompareColorInPattern(MatchData.Direction.TOP, block);
            right = CompareColorInPattern(MatchData.Direction.RIGHT, block);
            bottom = CompareColorInPattern(MatchData.Direction.BOTTOM, block);

            match = hor || ver || left || top || right || bottom;

            if (match)
            {
                if (hor)
                {
                    if (left && right) matchList.Add(new MatchData(MatchData.Direction.RIGHT, 5, block._X - 2, block._Y));
                    else if (left) matchList.Add(new MatchData(MatchData.Direction.RIGHT, 4, block._X - 2, block._Y));
                    else if (right) matchList.Add(new MatchData(MatchData.Direction.RIGHT, 4, block._X - 1, block._Y));
                    else matchList.Add(new MatchData(MatchData.Direction.RIGHT, 3, block._X - 1, block._Y));
                }
                else
                {
                    if (left) matchList.Add(new MatchData(MatchData.Direction.LEFT, 3, block._X, block._Y));
                    if (right) matchList.Add(new MatchData(MatchData.Direction.RIGHT, 3, block._X, block._Y));
                }
                if (ver)
                {
                    if (top && bottom) matchList.Add(new MatchData(MatchData.Direction.TOP, 5, block._X, block._Y - 2));
                    else if (top) matchList.Add(new MatchData(MatchData.Direction.TOP, 4, block._X, block._Y - 1));
                    else if (bottom) matchList.Add(new MatchData(MatchData.Direction.TOP, 4, block._X, block._Y - 2));
                    else matchList.Add(new MatchData(MatchData.Direction.TOP, 3, block._X, block._Y - 1));
                }
                else
                {
                    if (top) matchList.Add(new MatchData(MatchData.Direction.TOP, 3, block._X, block._Y));
                    if (bottom) matchList.Add(new MatchData(MatchData.Direction.BOTTOM, 3, block._X, block._Y));
                }
            }

            return match;
        }

        bool CompareColor(Block block, int fx, int fy, int sx, int sy)
        {
            Colors color = block.CurrentColor;
            int x = block._X;
            int y = block._Y;

            int x1 = x + fx;
            int y1 = y + fy;
            int x2 = x + sx;
            int y2 = y + sy;

            if (x1 < 0 || x1 > width - 1 || x2 < 0 || x2 > width - 1 || y1 < 0 || y1 > height - 1 || y2 < 0 || y2 > height - 1) return false;

            return color == blockList[x1, y1].CurrentColor && color == blockList[x2, y2].CurrentColor;
        }

        bool CompareColor(Colors color, int fx, int fy, int sx, int sy)
        {
            return color == blockList[fx, fy].CurrentColor && color == blockList[sx, sy].CurrentColor;
        }
        /// <summary>
        /// Call at Blocks swap Vertical 
        /// </summary>
        bool CompareColorInHorizontal(int x, int y, Colors color)
        {
            return (CompareColor(color, x - 1, y) && CompareColor(color, x + 1, y));
        }

        bool CompareColorInHorizontal(Block block)
        {
            int x = block._X;
            int y = block._Y;
            Colors color = block.CurrentColor;

            return (CompareColor(color, x - 1, y) && CompareColor(color, x + 1, y));
        }

        /// <summary>
        /// Call at Blocks swap Horizontal
        /// </summary>
        /// 
        bool CompareColorInVertical(Block block)
        {
            int x = block._X;
            int y = block._Y;
            Colors color = block.CurrentColor;

            return (CompareColor(color, x, y - 1) && CompareColor(color, x, y + 1));
        }
        bool CompareColorInVertical(int x, int y, Colors color)
        {
            return (CompareColor(color, x, y - 1) && CompareColor(color, x, y + 1));
        }


        bool CompareColorInPattern(MatchData.Direction dir, Block block)
        {
            int x = block._X;
            int y = block._Y;
            Colors color = block.CurrentColor;

            switch (dir)
            {
                case MatchData.Direction.LEFT:
                    return (CompareColor(color, x - 2, y) && CompareColor(color, x - 1, y));
                case MatchData.Direction.RIGHT:
                    return (CompareColor(color, x + 2, y) && CompareColor(color, x + 1, y));
                case MatchData.Direction.TOP:
                    return (CompareColor(color, x, y + 2) && CompareColor(color, x, y + 1));
                case MatchData.Direction.BOTTOM:
                    return (CompareColor(color, x, y - 2) && CompareColor(color, x, y - 1));
            }

            return false;
        }
        bool CompareColorInPattern(MatchData.Direction dir, int x, int y, Colors color)
        {
            switch (dir)
            {
                case MatchData.Direction.LEFT:
                    return (CompareColor(color, x - 2, y) && CompareColor(color, x - 1, y));
                case MatchData.Direction.RIGHT:
                    return (CompareColor(color, x + 2, y) && CompareColor(color, x + 1, y));
                case MatchData.Direction.TOP:
                    return (CompareColor(color, x, y + 2) && CompareColor(color, x, y + 1));
                case MatchData.Direction.BOTTOM:
                    return (CompareColor(color, x, y - 2) && CompareColor(color, x, y - 1));
            }

            return false;
        }

        bool CompareColor(Color color, int fx, int fy, int sx, int sy)
        {
            return false;
        }

        bool CompareColor(Colors a, Colors b)
        {
            return a == b;
        }
        bool CompareColor(Colors a, Block b)
        {
            return a == b.CurrentColor;
        }

        bool CompareColor(Colors a, int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return false;
            return a == blockList[x, y].CurrentColor;
        }

        /// <summary>
        /// Only Check Match at Position
        /// </summary>
        /// <param name="x">target index x</param>
        /// <param name="y">target index y</param>
        /// <returns></returns>
        bool checkMatch(int x, int y)
        {
            Colors pColor = blockList[x, y].CurrentColor;

            //left
            if (x > 1 && pColor == blockList[x - 1, y].CurrentColor && pColor == blockList[x - 2, y].CurrentColor) return true;
            //right
            else if (x < width - 2 && pColor == blockList[x + 1, y].CurrentColor && pColor == blockList[x + 2, y].CurrentColor) return true;
            //top
            else if (y < height - 2 && pColor == blockList[x, y + 1].CurrentColor && pColor == blockList[x, y + 2].CurrentColor) return true;
            //bottom
            else if (y > 1 && pColor == blockList[x, y - 1].CurrentColor && pColor == blockList[x, y - 2].CurrentColor) return true;

            return false;
        }
        bool checkMatch(Block block)
        {
            Colors pColor = block.CurrentColor;
            int x = block._X;
            int y = block._Y;

            //left
            if (x > 1 && pColor == blockList[x - 1, y].CurrentColor && pColor == blockList[x - 2, y].CurrentColor) return true;
            //right
            else if (x < width - 2 && pColor == blockList[x + 1, y].CurrentColor && pColor == blockList[x + 2, y].CurrentColor) return true;
            //top
            else if (y < height - 2 && pColor == blockList[x, y + 1].CurrentColor && pColor == blockList[x, y + 2].CurrentColor) return true;
            //bottom
            else if (y > 1 && pColor == blockList[x, y - 1].CurrentColor && pColor == blockList[x, y - 2].CurrentColor) return true;

            return false;
        }


        #endregion

        #region -------Property-------


        #endregion

    }
}
