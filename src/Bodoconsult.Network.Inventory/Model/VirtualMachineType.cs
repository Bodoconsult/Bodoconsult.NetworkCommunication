using System.Runtime.Serialization;
namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public enum VirtualMachineType
{
    [EnumMemberAttribute]
    HyperV,
    [EnumMemberAttribute]
    VmWare
}