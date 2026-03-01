using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial class YooSystem
    {
        private partial void InitPackage(YooComponent componet)
        {
            if (componet.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(componet.PackageID);
                if (packageSetting == null)
                    return;
                //创建资源包裹类
                var package = YooAssets.TryGetPackage(packageSetting.Name);
                if (package == null)
                    package = YooAssets.CreatePackage(packageSetting.Name);

                // 编辑器下的模拟模式
                InitializationOperation operation = null;
                if (PackageSettings.PlayMode == EPlayMode.EditorSimulateMode)
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(packageSetting.Name);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var createParameters = new EditorSimulateModeParameters();
                    createParameters.EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    operation = package.InitializeAsync(createParameters);
                }

                // 单机运行模式
                if (PackageSettings.PlayMode == EPlayMode.OfflinePlayMode)
                {
                    var createParameters = new OfflinePlayModeParameters();
                    createParameters.BuildinFileSystemParameters = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    operation = package.InitializeAsync(createParameters);
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
                    operation = package.InitializeAsync(createParameters);
                }
                componet.Status = EOperationStatus.Processing;
                // 如果初始化失败弹出提示界面
                if (operation.Status != EOperationStatus.Succeed)
                {
                    componet.Status = operation.Status;
                }
                else
                {
                    componet.PackageStatus = YooStatus.RequestVersion;
                    componet.Status = EOperationStatus.None;
                }
            }
            else if (componet.Status == EOperationStatus.Processing)
            { 
                
            }
        }
    }
}
