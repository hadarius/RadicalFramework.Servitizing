namespace Radical.Servitizing.Application
{
    public interface IApplicationSetup
    {
        IApplicationSetup UseDataServices();

        IApplicationSetup UseInternalProvider();

        IApplicationSetup UseDataMigrations();
    }
}