using Pulumi;

namespace Infrastructure.Resources;

public abstract class AbstractResource
{
    private readonly ResourceGroupResource _resourceGroupResource;

    protected AbstractResource(ResourceGroupResource resourceGroupResource, string name, bool nameHasHyphens = true, bool nameHasEnvironment = true)
    {
        _resourceGroupResource = resourceGroupResource;

        Name = resourceGroupResource.GetResourceName(name, nameHasHyphens, nameHasEnvironment);
    }

    public string Location => _resourceGroupResource.Location;

    public string Name { get; }

    public Output<string> ResourceGroupName => _resourceGroupResource.ResourceGroup.Name;

    public string GetResourceName(string name, bool nameHasHyphens = true, bool nameHasEnvironment = true)
        => _resourceGroupResource.GetResourceName(name, nameHasHyphens, nameHasEnvironment);
}
