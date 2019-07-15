using System.ComponentModel;
using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class ReadonlyLinkedToImmutableEntityDto : ILinkToEntity<DddCompositeIntString>
    {
        [ReadOnly(true)]
        public int Id { get; set; }
    }
}