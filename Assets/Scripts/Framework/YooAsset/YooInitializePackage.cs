using Unity.Entities;
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void InitPackage(RefRW<YooComponent> yoo)
        {
            if (yoo.ValueRW.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(yoo.ValueRW.PackageID);
                if (packageSetting == null)
                {
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                    return;
                }
                //创建资源包裹类
                var package = YooAssets.TryGetPackage(packageSetting.Name);
                if (package == null)
                    package = YooAssets.CreatePackage(packageSetting.Name);
                if (package.InitializeStatus == EOperationStatus.Succeed)
                {
                    yoo.ValueRW.Status = package.InitializeStatus;
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
                yoo.ValueRW.Status = operation.Status;
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Processing)
            {
                var packageSetting = GetPackageSetting(yoo.ValueRW.PackageID);
                if (packageSetting.operation == null)
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                else
                    yoo.ValueRW.Status = packageSetting.operation.Status;
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Failed)
            {
                yoo.ValueRW.PackageStatus = YooStatus.Error;
            }
            else if (yoo.ValueRW.Status == EOperationStatus.Succeed)
            {
                yoo.ValueRW.PackageStatus = YooStatus.RequestVersion;
                yoo.ValueRW.Status = EOperationStatus.None;
            }
        }
    }
}
