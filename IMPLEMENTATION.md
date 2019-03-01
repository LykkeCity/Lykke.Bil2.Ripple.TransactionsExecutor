# Blockchain integration transaction executor

This component of blockchain integration is intended for building and executing transactions.

To implement transaction executor component:

1. Consider implementing dedicated service to provide an abstraction of blockchain client API. Such approach helps to write well structured and testable component.

2. Implement `IAddressValidator` interface for validating deposit and withdraw addresses.

3. Implement `IHealthProvider` interface for checking services state.

4. Implement `IIntegrationInfoService` interface for checking blockchain state.

5. Implement `ITransactionEstimator` interface for estimating transaction cost.

6. Implement `ITransactionExecutor` interface for building and sending transactions.

7. Provide implementations to SDK code in `ConfigureServices()` method on startup.  Use `options.UseSettings` delegate to register additional services in DI container.

8. Provide integration name and other options in `Configure()` method on startup.