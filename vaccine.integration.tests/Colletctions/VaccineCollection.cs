using vaccine.integration.tests.Fixtures;

namespace vaccine.integration.tests.Colletctions;

[CollectionDefinition(nameof(VaccineCollection))]
public class VaccineCollection : ICollectionFixture<InfraFixture>
{
}
