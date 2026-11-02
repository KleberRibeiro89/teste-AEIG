import { Button, Group, NumberInput, Paper, Stack, Text, TextInput, Title } from '@mantine/core'
import { useEffect, useState, type FormEvent } from 'react'
import { useNavigate, useParams } from 'react-router'
import { http } from '../../lib/http.ts'
import { cobrancaApi, pagarCobranca, type Cobranca } from './cobranca.ts'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

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

function formatMonth(value: string) {
  const [year, month] = value.split('-')
  const date = new Date(Number(year), Number(month) - 1, 1)
  if (Number.isNaN(date.getTime())) return value
  const label = date.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
  return label.charAt(0).toUpperCase() + label.slice(1)
}

export function BaixaCobrancaPage() {
  const { navigationId, cobrancaId } = useParams()
  const navigate = useNavigate()
  const [cobranca, setCobranca] = useState<Cobranca | null>(null)
  const [multaPercentual, setMultaPercentual] = useState(0)
  const [jurosDiario, setJurosDiario] = useState(0)
  const [dataPagamento, setDataPagamento] = useState(todayIso)
  const [juros, setJuros] = useState<number | string>(0)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  const back = () => navigate(`/associados/${navigationId}`)

  useEffect(() => {
    if (!cobrancaId) return
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    Promise.all([
      cobrancaApi.getById(cobrancaId, controller.signal),
      http<{ multaPercentual: number; jurosDiarioPercentual: number }>('/api/ParametroPagamento', {
        signal: controller.signal,
      }),
    ])
      .then(([item, parametro]) => {
        const hoje = todayIso()
        setCobranca(item)
        setMultaPercentual(parametro.multaPercentual)
        setJurosDiario(parametro.jurosDiarioPercentual)
        setDataPagamento(hoje)
        setJuros(suggestedJuros(item.valor, item.dataVencimento, hoje, parametro.jurosDiarioPercentual))
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar a cobrança.')
        setLoading(false)
      })

    return () => controller.abort()
  }, [cobrancaId])

  const dias = cobranca ? daysLate(cobranca.dataVencimento, dataPagamento) : 0
  const jurosNumero = typeof juros === 'number' ? juros : Number(String(juros).replace(',', '.'))
  const multa = cobranca && dias > 0 ? roundMoney(cobranca.valor * (multaPercentual / 100)) : 0
  const total = cobranca ? roundMoney(cobranca.valor + multa + (Number.isFinite(jurosNumero) ? jurosNumero : 0)) : 0
  const alreadyPaid = (cobranca?.nomeStatus ?? '').toLocaleLowerCase('pt-BR').startsWith('pago')

  function changeDate(value: string) {
    setDataPagamento(value)
    if (!cobranca) return
    setJuros(suggestedJuros(cobranca.valor, cobranca.dataVencimento, value, jurosDiario))
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!cobrancaId || !cobranca) return
    if (!dataPagamento) {
      setError('Informe a data de pagamento.')
      return
    }
    if (dataPagamento > todayIso()) {
      setError('A data de pagamento não pode ser futura.')
      return
    }
    if (!Number.isFinite(jurosNumero) || jurosNumero < 0) {
      setError('Informe os juros.')
      return
    }

    setSaving(true)
    setError(null)
    try {
      await pagarCobranca(cobrancaId, { dataPagamento, juros: roundMoney(jurosNumero) })
      back()
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Não foi possível registrar a baixa.')
      setSaving(false)
    }
  }

  return (
    <Stack gap="md" p="xl" maw={560}>
      <Title order={2}>Baixa manual</Title>
      {loading && <Text c="dimmed">Carregando...</Text>}
      {error && (
        <Text c="red" size="sm">
          {error}
        </Text>
      )}
      {cobranca && (
        <Paper component="form" onSubmit={submit} p="lg" radius="md" bg="dark.6" withBorder>
          <Stack gap="md">
            <div>
              <Text fw={600}>{formatMonth(cobranca.mesReferencia)}</Text>
              <Text size="sm" c="dimmed">
                Valor {money.format(cobranca.valor)}
              </Text>
              <Text size="sm" c="dimmed">
                Vencimento {cobranca.dataVencimento.split('-').reverse().join('/')}
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
              <Button variant="default" onClick={back} disabled={saving}>
                Cancelar
              </Button>
              {!alreadyPaid && (
                <Button type="submit" loading={saving} disabled={loading}>
                  Confirmar baixa
                </Button>
              )}
            </Group>
          </Stack>
        </Paper>
      )}
    </Stack>
  )
}
