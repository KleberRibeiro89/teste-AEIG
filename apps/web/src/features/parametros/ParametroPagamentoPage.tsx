import { Button, Group, NumberInput, Paper, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState, type FormEvent } from 'react'
import { getParametroPagamento, updateParametroPagamento } from './parametroPagamento.ts'

function toNumber(value: number | string) {
  return typeof value === 'number' ? value : Number(String(value).replace(',', '.'))
}

export function ParametroPagamentoPage() {
  const [multa, setMulta] = useState<number | string>(0)
  const [juros, setJuros] = useState<number | string>(0)
  const [error, setError] = useState<string | null>(null)
  const [saved, setSaved] = useState(false)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)
    getParametroPagamento(controller.signal)
      .then((parametro) => {
        setMulta(parametro.multaPercentual)
        setJuros(parametro.jurosDiarioPercentual)
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar os parâmetros.')
        setLoading(false)
      })
    return () => controller.abort()
  }, [])

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const multaPercentual = toNumber(multa)
    const jurosDiarioPercentual = toNumber(juros)

    if (!Number.isFinite(multaPercentual) || multaPercentual < 0) {
      setError('Informe a multa.')
      setSaved(false)
      return
    }
    if (!Number.isFinite(jurosDiarioPercentual) || jurosDiarioPercentual < 0) {
      setError('Informe os juros diários.')
      setSaved(false)
      return
    }

    setSaving(true)
    setError(null)
    setSaved(false)
    try {
      const updated = await updateParametroPagamento({ multaPercentual, jurosDiarioPercentual })
      setMulta(updated.multaPercentual)
      setJuros(updated.jurosDiarioPercentual)
      setSaved(true)
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Não foi possível salvar os parâmetros.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <Stack gap="md" p="xl" maw={560}>
      <Title order={2}>Parametrização</Title>
      <Text c="dimmed" size="sm">
        Multa única e juros simples por dia de atraso, ambos sobre o valor original da cobrança.
      </Text>
      {loading && <Text c="dimmed">Carregando...</Text>}
      {!loading && (
        <Paper component="form" onSubmit={submit} p="lg" radius="md" bg="dark.6" withBorder>
          <Stack gap="md">
            {error && (
              <Text c="red" size="sm">
                {error}
              </Text>
            )}
            {saved && (
              <Text c="orange" size="sm">
                Parâmetros salvos.
              </Text>
            )}
            <NumberInput
              label="Multa"
              description="Percentual aplicado uma vez após o vencimento"
              value={multa}
              onChange={(value) => {
                setMulta(value)
                setSaved(false)
              }}
              min={0}
              decimalScale={6}
              suffix=" %"
              decimalSeparator=","
              thousandSeparator="."
              required
            />
            <NumberInput
              label="Juros diários"
              description="Percentual simples por dia corrido de atraso"
              value={juros}
              onChange={(value) => {
                setJuros(value)
                setSaved(false)
              }}
              min={0}
              decimalScale={6}
              suffix=" %"
              decimalSeparator=","
              thousandSeparator="."
              required
            />
            <Group justify="flex-end">
              <Button type="submit" loading={saving}>
                Salvar
              </Button>
            </Group>
          </Stack>
        </Paper>
      )}
    </Stack>
  )
}
