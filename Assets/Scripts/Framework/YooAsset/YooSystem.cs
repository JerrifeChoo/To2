using System.Collections;
using Unity.Entities;
using YooAsset;
using static Unity.Entities.SystemAPI;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem : SystemBase
    {
        private PackageSettings PackageSettings;

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            foreach (var component in Query<RefRW<YooComponent>>())
            {
                var valueRW = component.ValueRW;
                switch (valueRW.PackageStatus)
                {
                    case YooStatus.None:
                        break;
                    case YooStatus.InitializePackage:
                        InitPackage(valueRW);
                        break;
                    case YooStatus.RequestVersion:
                        break;
                    case YooStatus.UpdateManifest:
                        break;
                    case YooStatus.CreateDownloader:
                        break;
                    case YooStatus.DownloadPackage:
                        break;
                    case YooStatus.ClearCacheBundle:
                        break;
                }
            }
        }

        private PackageSetting GetPackage(int ID)
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

        private IEnumerator InitPackage(YooComponent componet)
        {
            if (componet.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackage(componet.PackageID);
                if (packageSetting == null)
                    yield return null;
                //创建资源包裹类
                var package = YooAssets.TryGetPackage(packageSetting.Name);
                if (package == null)
                    package = YooAssets.CreatePackage(packageSetting.Name);

                // 编辑器下的模拟模式
                InitializationOperation initializationOperation = null;
                if (PackageSettings.PlayMode == EPlayMode.EditorSimulateMode)
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(packageSetting.Name);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var createParameters = new EditorSimulateModeParameters();
                    createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // 单机运行模式
                if (PackageSettings.PlayMode == EPlayMode.OfflinePlayMode)
                {
                    var createParameters = new OfflinePlayModeParameters();
                    createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                // 联机运行模式
                if (PackageSettings.PlayMode == EPlayMode.HostPlayMode)
                {
                    string defaultHostServer = GetHostServerURL(packageSetting);
                    string fallbackHostServer = GetHostServerURL(packageSetting);
                    IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    var createParameters = new HostPlayModeParameters();
                    createParameters.BuildinFileSystemParameters = null;
                    createParameters.CacheFileSystemParameters = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                    initializationOperation = package.InitializeAsync(createParameters);
                }

                yield return initializationOperation;

                // 如果初始化失败弹出提示界面
                if (initializationOperation.Status != EOperationStatus.Succeed)
                {

                }
                else
                {
                }
            }
            yield return null;
        }
    }
}
