namespace Radical.Servitizing.DTO
{
    public class LinkDTO : DTO
    {
        public virtual long LeftEntityId { get; set; }

        public virtual long RightEntityId { get; set; }
    }
}