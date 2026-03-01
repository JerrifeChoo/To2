using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void InitPackage(ref YooComponent component)
        {
            if (component.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(component.PackageID);
                if (packageSetting == null)
                {
                    component.Status = EOperationStatus.Failed;
                    return;
                }
                //创建资源包裹类
                var package = YooAssets.TryGetPackage(packageSetting.Name);
                if (package == null)
                    package = YooAssets.CreatePackage(packageSetting.Name);
                if (package.InitializeStatus == EOperationStatus.Succeed)
                {
                    component.Status = package.InitializeStatus;
                    return;
                }
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
                packageSetting.operation = operation;
                component.Status = operation.Status;
            }
            else if (component.Status == EOperationStatus.Processing)
            {
                var packageSetting = GetPackageSetting(component.PackageID);
                if (packageSetting.operation == null)
                    component.Status = EOperationStatus.Failed;
                else
                    component.Status = packageSetting.operation.Status;
            }
            else if (component.Status == EOperationStatus.Failed)
            {
            }
            else if (component.Status == EOperationStatus.Succeed)
            {
                component.PackageStatus = YooStatus.RequestVersion;
                component.Status = EOperationStatus.None;
            }
        }
    }
}
