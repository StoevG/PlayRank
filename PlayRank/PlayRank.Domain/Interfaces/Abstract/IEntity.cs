namespace PlayRank.Domain.Interfaces.Abstract
{
    public interface IEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
