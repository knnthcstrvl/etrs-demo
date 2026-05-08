using Mapster;

namespace Financial
{
    public class MappingProfile
    {
        public static void RegisterMappings()
        {
            var config = TypeAdapterConfig.GlobalSettings;
            // No project-specific mappings required for trimmed ETRS repo.
            // Add explicit mappings here if/when new DTOs are added.
        }
    }
}
