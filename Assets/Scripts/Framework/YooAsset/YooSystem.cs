using Unity.Entities;
using YooAsset;
using static Unity.Entities.SystemAPI;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem : SystemBase
    {
        PackageSettings PackageSettings;

        protected override void OnUpdate()
        {
            foreach (var component in Query<RefRW<YooComponent>>())
            {
                var valueRW = component.ValueRW;
                switch (valueRW.PackageStatus)
                {
                    case YooStatus.None:                                            break;
                    case YooStatus.InitializePackage:   InitPackage(valueRW);       break;
                    case YooStatus.RequestVersion:      RequestVersion(valueRW);    break;
                    case YooStatus.UpdateManifest:      UpdateManifest(valueRW);    break;
                    case YooStatus.DownloadPackage:     DownloadPackage(valueRW);   break;
                    case YooStatus.ClearCacheBundle:    ClearCacheBundle(valueRW);  break;
                }
            }
        }

        protected PackageSetting GetPackageSetting(int ID)
        {
            if (PackageSettings == null || PackageSettings.Packages == null || PackageSettings.Packages.Length == 0)
                return null;
            foreach (var package in PackageSettings.Packages)
            {
                if (package.ID == ID)
                    return package;
            }
            return null;
        }

        private string GetHostServerURL(PackageSetting setting)
        {
            string hostServerIP = setting.Host != null ? setting.Host : PackageSettings.DefaultHost;
            string appVersion = setting.Version;

#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                return $"{hostServerIP}/Android/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                return $"{hostServerIP}/IPhone/{appVersion}";
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
                return $"{hostServerIP}/WebGL/{appVersion}";
            else
                return $"{hostServerIP}/{appVersion}";
#else
        if (Application.platform == RuntimePlatform.Android)
            return $"{hostServerIP}/CDN/Android/{appVersion}";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            return $"{hostServerIP}/CDN/IPhone/{appVersion}";
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return $"{hostServerIP}/CDN/WebGL/{appVersion}";
        else
            return $"{hostServerIP}/CDN/PC/{appVersion}";
#endif
        }

        /// <summary>
        /// 远端资源地址查询服务类
        /// </summary>
        private class RemoteServices : IRemoteServices
        {
            private readonly string _defaultHostServer;
            private readonly string _fallbackHostServer;

            public RemoteServices(string defaultHostServer, string fallbackHostServer)
            {
                _defaultHostServer = defaultHostServer;
                _fallbackHostServer = fallbackHostServer;
            }
            string IRemoteServices.GetRemoteMainURL(string fileName)
            {
                return $"{_defaultHostServer}/{fileName}";
            }
            string IRemoteServices.GetRemoteFallbackURL(string fileName)
            {
                return $"{_fallbackHostServer}/{fileName}";
            }
        }

        private partial void InitPackage(YooComponent componet);
        private partial void RequestVersion(YooComponent componet);
        private partial void UpdateManifest(YooComponent componet);
        private partial void DownloadPackage(YooComponent componet);
        private partial void ClearCacheBundle(YooComponent componet);
    }
}