import { Button, Group, NumberInput, Paper, Stack, Text, TextInput } from '@mantine/core'
import { useState, type FormEvent } from 'react'
import { formatDate, formatMonth, money } from '../../../lib/format.ts'
import type { Cobranca } from '../cobranca.ts'

function todayIso() {
  const now = new Date()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  return `${now.getFullYear()}-${month}-${day}`
}

function daysLate(due: string, payment: string) {
  const start = Date.parse(`${due.slice(0, 10)}T00:00:00`)
  const end = Date.parse(`${payment}T00:00:00`)
  if (Number.isNaN(start) || Number.isNaN(end)) return 0
  return Math.max(0, Math.round((end - start) / 86_400_000))
}

function roundMoney(value: number) {
  return Math.round((value + Number.EPSILON) * 100) / 100
}

function suggestedJuros(valor: number, due: string, payment: string, dailyPercent: number) {
  const days = daysLate(due, payment)
  if (days <= 0) return 0
  return roundMoney(valor * (dailyPercent / 100) * days)
}

interface BaixaCobrancaFormProps {
  cobranca: Cobranca
  multaPercentual: number
  jurosDiario: number
  saving: boolean
  onCancel: () => void
  onSubmit: (payload: { dataPagamento: string; juros: number }) => Promise<void>
  onError: (message: string | null) => void
}

export function BaixaCobrancaForm({
  cobranca,
  multaPercentual,
  jurosDiario,
  saving,
  onCancel,
  onSubmit,
  onError,
}: BaixaCobrancaFormProps) {
  const [dataPagamento, setDataPagamento] = useState(() => todayIso())
  const [juros, setJuros] = useState<number | string>(() =>
    suggestedJuros(cobranca.valor, cobranca.dataVencimento, todayIso(), jurosDiario),
  )

  const dias = daysLate(cobranca.dataVencimento, dataPagamento)
  const jurosNumero = typeof juros === 'number' ? juros : Number(String(juros).replace(',', '.'))
  const multa = dias > 0 ? roundMoney(cobranca.valor * (multaPercentual / 100)) : 0
  const total = roundMoney(cobranca.valor + multa + (Number.isFinite(jurosNumero) ? jurosNumero : 0))
  const alreadyPaid = (cobranca.nomeStatus ?? '').toLocaleLowerCase('pt-BR').startsWith('pago')

  function changeDate(value: string) {
    setDataPagamento(value)
    setJuros(suggestedJuros(cobranca.valor, cobranca.dataVencimento, value, jurosDiario))
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!dataPagamento) {
      onError('Informe a data de pagamento.')
      return
    }
    if (dataPagamento > todayIso()) {
      onError('A data de pagamento não pode ser futura.')
      return
    }
    if (!Number.isFinite(jurosNumero) || jurosNumero < 0) {
      onError('Informe os juros.')
      return
    }

    onError(null)
    await onSubmit({ dataPagamento, juros: roundMoney(jurosNumero) })
  }

  return (
    <Paper component="form" onSubmit={submit} p="lg" radius="md" bg="dark.6" withBorder>
      <Stack gap="md">
        <div>
          <Text fw={600}>{formatMonth(cobranca.mesReferencia)}</Text>
          <Text size="sm" c="dimmed">
            Valor {money.format(cobranca.valor)}
          </Text>
          <Text size="sm" c="dimmed">
            Vencimento {formatDate(cobranca.dataVencimento)}
          </Text>
        </div>
        {alreadyPaid ? (
          <Text c="dimmed">Esta cobrança já está paga.</Text>
        ) : (
          <>
            <TextInput
              label="Data de pagamento"
              type="date"
              max={todayIso()}
              value={dataPagamento}
              onChange={(event) => changeDate(event.currentTarget.value)}
              required
            />
            <NumberInput
              label="Juros"
              value={juros}
              onChange={setJuros}
              min={0}
              decimalScale={2}
              fixedDecimalScale
              prefix="R$ "
              decimalSeparator=","
              thousandSeparator="."
              required
            />
            <Text size="sm" c="dimmed">
              Multa {money.format(multa)}
              {dias > 0 ? ` · ${dias} dia${dias === 1 ? '' : 's'} de atraso` : ''}
            </Text>
            <Text size="sm" c="orange">
              Total {money.format(total)}
            </Text>
          </>
        )}
        <Group justify="flex-end">
          <Button variant="default" onClick={onCancel} disabled={saving}>
            Cancelar
          </Button>
          {!alreadyPaid && (
            <Button type="submit" loading={saving}>
              Confirmar baixa
            </Button>
          )}
        </Group>
      </Stack>
    </Paper>
  )
}
