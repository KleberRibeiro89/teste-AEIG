using System.Text.Json.Serialization;

namespace AEIG.Domain.AppService.Dtos.Requests.CobrancaRequests;

public record AvisoPagamentoRequest
{
    [JsonPropertyName("id_evento")]
    public string IdEvento { get; set; } = string.Empty;

    [JsonPropertyName("nosso_numero")]
    public string NossoNumero { get; set; } = string.Empty;

    [JsonPropertyName("data_pagamento")]
    public DateOnly DataPagamento { get; set; }
}
