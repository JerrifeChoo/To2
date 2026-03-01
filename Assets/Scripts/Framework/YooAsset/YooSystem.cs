using Unity.Collections;
using Unity.Entities;
using YooAsset;
using static Unity.Entities.SystemAPI;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem : ISystem
    {
        private static PackageSettings PackageSettings;

        public void OnUpdate(ref SystemState state)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            ComponentLookup<YooComponent> yooLookup = state.GetComponentLookup<YooComponent>();
            var entities = QueryBuilder().WithAll<YooComponent>().Build().ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                var component = yooLookup[entity];
                switch (component.PackageStatus)
                {
                    case YooStatus.None:                                                    break;
                    case YooStatus.InitializePackage:   InitPackage(ref component);         break;
                    case YooStatus.RequestVersion:      RequestVersion(ref component);      break;
                    case YooStatus.UpdateManifest:      UpdateManifest(ref component);      break;
                    case YooStatus.DownloadPackage:     DownloadPackage(ref component);     break;
                    case YooStatus.ClearCacheBundle:    ClearCacheBundle(ref component);    break;
                }
                yooLookup.SetComponentEnabled(entity, component.Status != EOperationStatus.Failed);
            }
            //watch.Stop();
        }

        public PackageSetting GetPackageSetting(int ID)
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
            string appVersion = PackageSettings.AppVersion;

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
    }
}