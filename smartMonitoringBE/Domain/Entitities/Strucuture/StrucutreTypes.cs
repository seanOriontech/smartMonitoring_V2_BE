namespace smartMonitoringBE.Domain.Entitities.Structure;


public enum WorkspaceType
{
    Organisation = 1,
    Client = 2,
    Project = 3
}

public enum WorkspaceNodeType
{
    Folder = 0,
    Site = 1,
    Area = 2,
    Poi = 3,
    Device = 4,
    Dashboard = 5
}

public enum NodeIconType
{
    Default = 0,
    Factory = 1,
    Warehouse = 2,
    Office = 3,
    Farm = 4,
    Home = 5,
    Retail = 6
}