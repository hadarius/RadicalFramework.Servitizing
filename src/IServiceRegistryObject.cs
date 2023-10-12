namespace Radical.Servitizing
{
    public interface IServiceRegistryObject<out T>
    {
        T Value { get; }
    }

    public interface IServiceRegistryObject
    {
        object Value { get; }
    }
}