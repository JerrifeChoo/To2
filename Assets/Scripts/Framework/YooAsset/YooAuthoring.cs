using Unity.Entities;
using UnityEngine;
using YooAsset;

namespace To2.Framework.YooAsset
{
    public enum YooStatus 
    {
        None,
        InitializePackage,
        RequestVersion,
        UpdateManifest,
        CreateDownloader,
        DownloadPackage,
        ClearCacheBundle,
    }

    public struct YooComponent : IComponentData, IEnableableComponent
    {
        public int PackageID;
        public YooStatus PackageStatus;
        public EOperationStatus Status;
    }

    public class YooAuthoring : MonoBehaviour
    {
        public int PackageID;
        public YooStatus PackageStatus;
        public EOperationStatus Status;

        class Baker : Baker<YooAuthoring>
        {
            public override void Bake(YooAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new YooComponent
                {
                    PackageID = authoring.PackageID,
                    PackageStatus = authoring.PackageStatus,
                    Status = authoring.Status,
                });
            }
        }
    }
}

