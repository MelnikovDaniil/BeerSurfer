using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Mappers
{
    public static class BeerMapper
    {
        private const string MapperName = "Beer";

        public static void Add(int beerCount)
        {
            var count = PlayerPrefs.GetInt(MapperName, 0) + beerCount;
            PlayerPrefs.SetFloat(MapperName, count);
        }
    }
}
