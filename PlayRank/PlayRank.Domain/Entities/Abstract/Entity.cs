using PlayRank.Domain.Interfaces.Abstract;

namespace PlayRank.Domain.Entities.Abstract
{
    public abstract class Entity : IEntity
    {
        public DateTime CreatedDate { get; set ; }
        public DateTime? ModifiedDate { get; set ; }
        public DateTime? DeletedOn { get ; set; }
    }
}