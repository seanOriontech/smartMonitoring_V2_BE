using smartMonitoringBE.Domain.Entitities.Structure;
using smartMonitoringBE.Models.DTO.Workspace;

static class WorkspaceNodeMappers
{
    public static Address? ToEntity(this WorkspaceNodeAddressDto? dto)
    {
        if (dto is null) return null;

        // if everything blank, treat as null
        var any =
            !string.IsNullOrWhiteSpace(dto.Line1) ||
            !string.IsNullOrWhiteSpace(dto.Line2) ||
            !string.IsNullOrWhiteSpace(dto.City) ||
            !string.IsNullOrWhiteSpace(dto.Province) ||
            !string.IsNullOrWhiteSpace(dto.PostalCode) ||
            !string.IsNullOrWhiteSpace(dto.Country);

        if (!any) return null;

        return new Address
        {
            Line1 = dto.Line1?.Trim(),
            Line2 = dto.Line2?.Trim(),
            City = dto.City?.Trim(),
            Province = dto.Province?.Trim(),
            PostalCode = dto.PostalCode?.Trim(),
            Country = string.IsNullOrWhiteSpace(dto.Country) ? "ZA" : dto.Country.Trim()
        };
    }

    public static ContactDetails? ToEntity(this WorkspaceNodeContactDto? dto)
    {
        if (dto is null) return null;

        var any =
            !string.IsNullOrWhiteSpace(dto.ContactName) ||
            !string.IsNullOrWhiteSpace(dto.Phone) ||
            !string.IsNullOrWhiteSpace(dto.Email);

        if (!any) return null;

        return new ContactDetails
        {
            ContactName = dto.ContactName?.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim()
        };
    }

    public static void ApplyTo(this WorkspaceNodeAddressDto? dto, Address? entity)
    {
        if (entity is null || dto is null) return;

        entity.Line1 = dto.Line1?.Trim();
        entity.Line2 = dto.Line2?.Trim();
        entity.City = dto.City?.Trim();
        entity.Province = dto.Province?.Trim();
        entity.PostalCode = dto.PostalCode?.Trim();
        entity.Country = string.IsNullOrWhiteSpace(dto.Country) ? "ZA" : dto.Country.Trim();
    }

    public static void ApplyTo(this WorkspaceNodeContactDto? dto, ContactDetails? entity)
    {
        if (entity is null || dto is null) return;

        entity.ContactName = dto.ContactName?.Trim();
        entity.Phone = dto.Phone?.Trim();
        entity.Email = dto.Email?.Trim();
    }
    

        public static WorkspaceNodeDto ToDto(this WorkspaceNode n) =>
            new(
                n.Id,
                n.WorkspaceId,
                n.ParentId,
                n.Type,
                n.IconType,
                n.Name,
                n.Code,
                n.SortOrder,
                n.IsActive,
                n.Description,
                n.Lat,
                n.Lng,
                n.TimeZone,
                n.Address is null ? null : new WorkspaceNodeAddressDto(
                    n.Address.Line1,
                    n.Address.Line2,
                    n.Address.City,
                    n.Address.Province,
                    n.Address.PostalCode,
                    n.Address.Country
                ),
                n.Contact is null ? null : new WorkspaceNodeContactDto(
                    n.Contact.ContactName,
                    n.Contact.Phone,
                    n.Contact.Email
                )
            );
    }
