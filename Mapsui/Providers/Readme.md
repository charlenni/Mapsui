## Providers

Providers are used to provide lists of features. These can either be stored locally or determined via internet queries. 

### Normal providers

Normal providers implement the IProvider interface. They deliver the features in a synchron way.

### Async providers

Asynchron providers implement the IAsyncProvider interface. They deliver the features with an async function.

### Wrapper as providers

These providers get another provider and do something with the retrived features.