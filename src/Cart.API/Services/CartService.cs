using Cart.API.Clients;
using Cart.API.Data;
using Cart.API.DTOs;
using Cart.API.Exceptions;
using Cart.API.Models;

namespace Cart.API.Services;

/// <summary>
/// Lógica de negocio del carrito. Consulta a Products.API para validar
/// que el producto exista (CRT-002) y que alcance el stock (CRT-003).
/// Cuando algo falla lanza la excepción de dominio; no devuelve códigos HTTP.
/// </summary>
public class CartService : ICartService
{
    private readonly CartRepository _repository;
    private readonly ProductsClient _productsClient;

    public CartService(CartRepository repository, ProductsClient productsClient)
    {
        _repository = repository;
        _productsClient = productsClient;
    }

    // ── GET ───────────────────────────────────────────────────────────────
    public async Task<CartResponse> ObtenerAsync(Guid usuarioId)
    {
        SimularErrorInterno(usuarioId);

        ShoppingCart carrito = await ObtenerCarritoExistenteAsync(usuarioId);
        return ConvertirAResponse(carrito);
    }

    // ── POST: agregar (se SUMA a lo que ya hay en el carrito) ─────────────
    public async Task<CartResponse> AgregarItemAsync(Guid usuarioId, AddCartItemRequest request)
    {
        ValidarCantidad(request.Cantidad);

        ProductResponse producto = await ObtenerProductoExistenteAsync(request.ProductoId);

        // El carrito puede no existir todavía: el primer POST lo crea
        ShoppingCart? carrito = await _repository.ObtenerPorUsuarioAsync(usuarioId);
        int cantidadActual = BuscarCantidadEnCarrito(carrito, request.ProductoId);
        int cantidadTotal = cantidadActual + request.Cantidad;

        // El stock se controla contra el total (lo que ya había + lo que se agrega)
        ValidarStock(producto, cantidadTotal);

        await _repository.GuardarItemAsync(usuarioId, request.ProductoId, cantidadTotal, DateTime.UtcNow);

        ShoppingCart actualizado = await ObtenerCarritoExistenteAsync(usuarioId);
        return ConvertirAResponse(actualizado);
    }

    // ── PUT: actualizar (se REEMPLAZA la cantidad) ────────────────────────
    public async Task<CartResponse> ActualizarItemAsync(Guid usuarioId, Guid productoId, UpdateCartItemRequest request)
    {
        ValidarCantidad(request.Cantidad);

        ShoppingCart carrito = await ObtenerCarritoExistenteAsync(usuarioId);
        ValidarProductoEnCarrito(carrito, productoId);

        ProductResponse producto = await ObtenerProductoExistenteAsync(productoId);
        ValidarStock(producto, request.Cantidad);

        await _repository.GuardarItemAsync(usuarioId, productoId, request.Cantidad, DateTime.UtcNow);

        ShoppingCart actualizado = await ObtenerCarritoExistenteAsync(usuarioId);
        return ConvertirAResponse(actualizado);
    }

    // ── DELETE: quitar un producto ────────────────────────────────────────
    // Decisión del equipo (README, 11/10): si el producto no está en el carrito → CRT-001
    public async Task QuitarItemAsync(Guid usuarioId, Guid productoId)
    {
        ShoppingCart carrito = await ObtenerCarritoExistenteAsync(usuarioId);

        if (BuscarCantidadEnCarrito(carrito, productoId) == 0)
        {
            throw new NotFoundException(ErrorCodes.CRT_001, "El producto no está en el carrito.");
        }

        await _repository.EliminarItemAsync(usuarioId, productoId, DateTime.UtcNow);
    }

    // ── DELETE: vaciar el carrito ─────────────────────────────────────────
    public async Task VaciarAsync(Guid usuarioId)
    {
        await ObtenerCarritoExistenteAsync(usuarioId);
        await _repository.VaciarAsync(usuarioId, DateTime.UtcNow);
    }

    // ── Métodos de apoyo ──────────────────────────────────────────────────

    // CRT-001: el usuario no tiene carrito
    private async Task<ShoppingCart> ObtenerCarritoExistenteAsync(Guid usuarioId)
    {
        ShoppingCart? carrito = await _repository.ObtenerPorUsuarioAsync(usuarioId);
        if (carrito == null)
        {
            throw new NotFoundException(ErrorCodes.CRT_001, "Carrito no encontrado.");
        }

        return carrito;
    }

    // CRT-002: el producto no existe en Products.API
    private async Task<ProductResponse> ObtenerProductoExistenteAsync(Guid productoId)
    {
        ProductResponse? producto = await _productsClient.ObtenerProductoAsync(productoId);
        if (producto == null)
        {
            throw new NotFoundException(ErrorCodes.CRT_002, "Producto no encontrado.");
        }

        return producto;
    }

    // CRT-002 (PUT): el producto no está en este carrito
    private static void ValidarProductoEnCarrito(ShoppingCart carrito, Guid productoId)
    {
        if (BuscarCantidadEnCarrito(carrito, productoId) == 0)
        {
            throw new NotFoundException(ErrorCodes.CRT_002, "El producto no está en el carrito.");
        }
    }

    // CRT-003: no alcanza el stock
    private static void ValidarStock(ProductResponse producto, int cantidadSolicitada)
    {
        if (cantidadSolicitada > producto.Stock)
        {
            throw new BusinessRuleException(ErrorCodes.CRT_003,
                "Stock insuficiente para '" + producto.Nombre + "'. Disponible: " + producto.Stock
                + ", solicitado: " + cantidadSolicitada + ".");
        }
    }

    // CRT-004: cantidad 0 o negativa (también lo frena la validación del pedido)
    private static void ValidarCantidad(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ValidationException(ErrorCodes.CRT_004, "Cantidad inválida. Debe ser mayor a 0.");
        }
    }

    // Cuántas unidades de ese producto hay en el carrito (0 si no está o no hay carrito)
    private static int BuscarCantidadEnCarrito(ShoppingCart? carrito, Guid productoId)
    {
        if (carrito == null)
        {
            return 0;
        }

        foreach (CartItem item in carrito.Items)
        {
            if (item.ProductoId == productoId)
            {
                return item.Cantidad;
            }
        }

        return 0;
    }

    // Pasa del modelo al DTO de respuesta
    private static CartResponse ConvertirAResponse(ShoppingCart carrito)
    {
        List<CartItemResponse> items = new List<CartItemResponse>();
        foreach (CartItem item in carrito.Items)
        {
            items.Add(new CartItemResponse
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad
            });
        }

        return new CartResponse
        {
            UsuarioId = carrito.UsuarioId,
            Items = items,
            FechaActualizacion = carrito.FechaActualizacion
        };
    }

    // Prueba del error 500 (CRT-005): se mantiene para la demo
    private static void SimularErrorInterno(Guid usuarioId)
    {
        if (usuarioId == Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"))
        {
            throw new Exception("Falla simulada para probar el error 500.");
        }
    }
}