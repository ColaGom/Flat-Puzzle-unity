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

namespace cbs.pang.game
{
    public partial class MatchContainer : MonoBehaviour
    {
#if UNITY_EDITOR
        
        int btnWitdh = 120;
        int btnHeight = 60;

        void OnGUI()
        {
            int index = 0;
            if (GUI.Button(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight), "OnContainer")) OnContainer();
            if (GUI.Button(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight), "OffContainer")) OffContainer();
            if (GUI.Button(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight), "AllBlockColorSetDelay")) StartCoroutine(AllBlockColorSetWithDelay(0));
            if (GUI.Button(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight), "AllBlockColorSet")) AllBlockColorSet();
            if (GUI.Button(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight), "AllBlockSpawn")) AllBlockSpawn();

            if (firBlock)
                GUI.Label(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight),
                string.Format("First OBJ_X:{0} Y:{1} COLOR:{2}", firBlock._X, firBlock._Y, firBlock.CurrentColor));
            if (secBlock)
                GUI.Label(new Rect(0, (btnHeight * index++), btnWitdh, btnHeight),
                    string.Format("Seconds OBJ_X:{0} Y:{1} COLOR:{2}", secBlock._X, secBlock._Y, secBlock.CurrentColor));

        }
        
#endif
    }
}
