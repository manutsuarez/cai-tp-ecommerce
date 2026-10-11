namespace Cart.API.DTOs;

public class CartResponse
{
    public Guid UsuarioId { get; set; }
    public List<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
    public DateTime FechaActualizacion { get; set; }
}