using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Common.Scripts.Configs
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(GameDatabase), menuName = nameof(GameDatabase))]
    public class GameDatabase : ScriptableObject
    {
        public List<GameInfo> GamesList;
    }
}
