
namespace Chat.Persistence.Extensions
{
    public static class AutoMapperExtensions
    {
        public static TDestination MapCommonCreateFieldsWithoutSource<TDestination>(this TDestination destination)
         where TDestination : BaseEntity, IEntity
        {
            destination.Id = Guid.NewGuid();
            destination.IsDeleted = false;
            return destination;
        }
    }
}
