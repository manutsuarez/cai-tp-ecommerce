namespace Products.API.DTOs;

/// <summary>
/// Respuesta de error (formato Problem Details con campos propios).
/// </summary>
public class ErrorResponse
{
    /// <summary>Referencia al tipo de error HTTP.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Nombre del error HTTP.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Código HTTP.</summary>
    public int Status { get; set; }

    /// <summary>Descripción general del error.</summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>Ruta del pedido que falló.</summary>
    public string? Instance { get; set; }

    /// <summary>Código de error propio del servicio (PRD-001 a PRD-005).</summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>Mensaje del error.</summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>Correlation ID del pedido, para buscarlo en los logs.</summary>
    public string? CorrelationId { get; set; }
}