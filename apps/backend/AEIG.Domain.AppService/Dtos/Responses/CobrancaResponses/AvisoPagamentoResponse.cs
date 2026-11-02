using System.Text.Json.Serialization;

namespace AEIG.Domain.AppService.Dtos.Responses.CobrancaResponses;

public class AvisoPagamentoResponse
{
    [JsonPropertyName("id_evento")]
    public string IdEvento { get; set; } = string.Empty;

    [JsonPropertyName("nosso_numero")]
    public string NossoNumero { get; set; } = string.Empty;

    [JsonPropertyName("data_pagamento")]
    public DateOnly? DataPagamento { get; set; }

    [JsonPropertyName("nome_status")]
    public string? NomeStatus { get; set; }

    [JsonPropertyName("valor_pago")]
    public decimal ValorPago { get; set; }

    [JsonPropertyName("ja_processado")]
    public bool JaProcessado { get; set; }

    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; } = string.Empty;
}

public class WebhookPagamentoResult
{
    public bool CobrancaEncontrada { get; set; }
    public AvisoPagamentoResponse Resposta { get; set; } = new();
}
