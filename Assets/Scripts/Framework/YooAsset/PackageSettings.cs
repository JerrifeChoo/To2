using System;
using UnityEngine;
using YooAsset;
namespace To2.Framework.YooAsset
{
    [Serializable]
    public class PackageSetting
    {
        public int ID;
        public string Name;
        public string Host;
        //[NonSerialized]
        //public string Version;
        [NonSerialized]
        public AsyncOperationBase operation;
    }

    [CreateAssetMenu(fileName = "PackageSettings", menuName = "YooAsset/Create Package Settings")]
    public class PackageSettings : ScriptableObject
    {
        public string DefaultHost = "127.0.0.1";
        public string AppVersion = "V1.0";
        public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        public PackageSetting[] Packages;
    }
}
