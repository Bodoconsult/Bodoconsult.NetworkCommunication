// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerFailoverRelationshipCollection : IEnumerable<IDhcpServerFailoverRelationship>
{
    IDhcpServer Server { get; }

    IDhcpServerFailoverRelationship GetRelationship(string relationshipName);
    void RemoveRelationship(IDhcpServerFailoverRelationship relationship);
}