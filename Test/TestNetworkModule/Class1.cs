using System;
using System.Collections.Generic;
using Flow;

namespace TestNetworkModule
{
    public interface IUser
    {
        string FirstName { get; }
        IOrganisation Org { get; }
    }

    public interface IOrganisation
    {
        string Email { get; }
        IResourceSet Resources { get; }
    }

    public interface IResourceSet
    {
        IList<IResource> Resources { get; }
    }

    public interface IResource
    {
    }

    namespace Proxy
    {
        public interface IUserProxy
        {
            string FirstName { get; }

            IFuture<IOrganisation> Org { get; }
        }
    }
}

namespace TestNetworkModule
{
    public interface IFieldProxy<T>
    {
        Guid NetGuid { get; }
        IFuture<T> Future { get; }
    }
}

