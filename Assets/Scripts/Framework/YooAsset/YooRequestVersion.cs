
using Unity.Entities;
using YooAsset;

namespace To2.Framework.YooAsset
{
    public partial struct YooSystem
    {
        private void RequestVersion(RefRW<YooComponent> yoo)
        {
            if (yoo.ValueRW.Status == EOperationStatus.None)
            {
                var packageSetting = GetPackageSetting(yoo.ValueRW.PackageID);
                if (packageSetting == null)
                {
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                    return;
                }
                var package = YooAssets.GetPackage(packageSetting.Name);
                if (package == null)
                {
                    yoo.ValueRW.Status = EOperationStatus.Failed;
                    return;
                }
                var operation = package.RequestPackageVersionAsync();
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
                yoo.ValueRW.PackageStatus = YooStatus.UpdateManifest;
                yoo.ValueRW.Status = EOperationStatus.None;
            }
        }
    }
}