using System;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class GameMgr: MonoBehaviour
	{
    private static GameMgr _instance;
        public static byte[] KEY = NormalizeString("hello aes", 32);
        public static byte[] IV = NormalizeString("1234567890");
        public static GameMgr Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameMgr();
                return _instance;
            }
        }

        public static byte[] NormalizeString(string orginal, int Length = 16)
        {
            string normalize;
            if (orginal == null)
                normalize = string.Empty.PadRight(Length);
            else
                normalize = orginal.PadRight(Length);
            byte[] result = new byte[normalize.Length];
            Array.Copy(Encoding.UTF8.GetBytes(normalize), 0, result, 0, Length);
            return result;
        }
    }
}