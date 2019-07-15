using System.ComponentModel;
using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class ReadonlyLinkedToMutableEntityDto : ILinkToEntity<NormalEntity>
    {
        [ReadOnly(true)]
        public int Id { get; set; }
    }
}