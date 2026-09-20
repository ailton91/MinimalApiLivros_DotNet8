namespace MinimalApiLivros.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTimeOffset DataCriacao { get; set; }
        public DateTimeOffset? DataAtualizacao { get; set; } = null;
    }
}
