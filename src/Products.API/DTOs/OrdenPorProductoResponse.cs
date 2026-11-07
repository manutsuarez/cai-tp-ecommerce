namespace Products.API.DTOs;

/// <summary>
/// Una orden que contiene el producto, tal como la devuelve Orders.API en su endpoint interno
/// (solo interesan el id y el estado).
/// </summary>
public class OrdenPorProductoResponse
{
    public Guid Id { get; set; }

    public string Estado { get; set; } = string.Empty;
}
