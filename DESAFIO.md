# Teste Prático — Programador(a) Full-Stack | AEIG

Oct 20, 2026

## Contexto e regras

Você vai construir uma pequena API de cobrança, inspirada no dia a dia da AEIG. A AEIG é uma associação de proteção veicular: cada associado paga uma mensalidade e, quando o banco confirma o pagamento, envia um aviso automático (webhook) ao nosso sistema.

| Regra | Detalhe |
| --- | --- |
| Tempo previsto | Até 2 horas de trabalho |
| Prazo de entrega | 48 horas: até [DATA E HORA] |
| Linguagem | TypeScript com Node.js (obrigatório). Framework livre |
| Banco de dados | Livre (SQLite, PostgreSQL ou outro) |
| Login na API | Não é necessário |
| Uso de IA | Permitido. Você deverá explicar qualquer trecho do código na apresentação |
| Envio e dúvidas | [E-MAIL] |

Se surgir uma dúvida não coberta aqui, tome a decisão que achar mais razoável e explique no README. Isso também faz parte da avaliação.

Preferimos uma entrega menor, que funcione e que você domine, a uma entrega maior que você não consiga explicar.

## Parte A — Mini-API de cobrança (cerca de 1h30)

Cadastro de associado — `POST /associados` com nome, CPF e valor da mensalidade.

- O CPF deve ter dígitos verificadores válidos e não pode se repetir.
- Erros devem retornar código HTTP adequado e mensagem clara.

Geração de cobrança — `POST /cobrancas` com o associado e o mês de referência (formato `AAAA-MM`).

- A cobrança tem valor igual à mensalidade, um "nosso número" único (formato livre) e status `PENDENTE`.
- Não pode haver duas cobranças do mesmo associado no mesmo mês.
- `GET /cobrancas/:nosso_numero` consulta uma cobrança.

Aviso de pagamento — `POST /webhooks/pagamento` recebe:

```json
{
  "id_evento": "evt_000123",
  "nosso_numero": "00012345",
  "data_pagamento": "2026-10-15"
}
```

- O aviso marca a cobrança como `PAGA`.
- O banco pode reenviar o mesmo aviso: o mesmo `id_evento` nunca pode ser processado duas vezes.
- Aviso com "nosso número" inexistente deve receber resposta adequada, sem derrubar o sistema.

Bônus, só se sobrar tempo: testes automatizados, ou aceitar o aviso apenas com assinatura HMAC-SHA256 válida no cabeçalho `X-Assinatura`.

## Parte B — Encontrar e corrigir defeitos (cerca de 30 min)

A função abaixo deveria calcular o valor de uma mensalidade paga com atraso, seguindo esta regra:

- Pagamento até o dia do vencimento: sem multa e sem juros.
- Após o vencimento: multa única de 2% mais juros simples de 0,033% por dia corrido de atraso, ambos sobre o valor original.
- Resultado final arredondado em centavos.

Ela foi usada em produção e associados reclamaram de valores errados.

```ts
// Deveria calcular o valor atualizado de uma cobrança:
// multa única de 2% + juros simples de 0,033% ao dia.
// Pagamento até o vencimento não tem multa nem juros.
function calcularValorAtualizado(
  valor: number,
  vencimento: string,
  dataPagamento: string
): number {
  const venc = new Date(vencimento);
  const pag = new Date(dataPagamento);
  const diasAtraso = Math.floor(
    (pag.getTime() - venc.getTime()) / 86400000
  );

  let total = valor;
  for (let i = 0; i <= diasAtraso; i++) {
    total = total + valor * 0.02 + total * 0.00033;
  }
  return total;
}
```

Em um arquivo `BUGS.md` do seu repositório:

- Liste cada defeito e explique, em linguagem simples, o efeito dele para o associado.
- Escreva a versão corrigida da função.
- Mostre o valor correto de uma mensalidade de R$ 189,90, com vencimento em 05/10/2026, paga em 01/10, 05/10, 06/10 e 15/10.

## Entrega e apresentação

Envie dentro do prazo:

- Link do repositório no GitHub (público, ou privado com acesso liberado para [USUÁRIO DO GITHUB]).
- `README.md` com: como rodar o projeto, as principais decisões, o que ficou faltando e onde usou IA.
- Exemplos de chamadas da API (Postman, Insomnia ou curl).
- O arquivo `BUGS.md` da Parte B.

Apresentação de 20 minutos ([PRESENCIAL EM GUARULHOS / ONLINE]): 5 minutos mostrando o projeto funcionando e 15 minutos de perguntas, com um pequeno ajuste feito ao vivo. Dominar o próprio código é requisito da vaga.
