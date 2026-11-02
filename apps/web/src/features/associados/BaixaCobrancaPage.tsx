import { Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router'
import { http } from '../../lib/http.ts'
import { cobrancaApi, pagarCobranca, type Cobranca } from './cobranca.ts'
import { BaixaCobrancaForm } from './components/BaixaCobrancaForm.tsx'

export function BaixaCobrancaPage() {
  const { navigationId, cobrancaId } = useParams()
  const navigate = useNavigate()
  const [cobranca, setCobranca] = useState<Cobranca | null>(null)
  const [multaPercentual, setMultaPercentual] = useState(0)
  const [jurosDiario, setJurosDiario] = useState(0)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)

  const back = () => navigate(`/associados/${navigationId}`)

  useEffect(() => {
    if (!cobrancaId) return
    const controller = new AbortController()
    setLoading(true)
    setError(null)
    setCobranca(null)

    Promise.all([
      cobrancaApi.getById(cobrancaId, controller.signal),
      http<{ multaPercentual: number; jurosDiarioPercentual: number }>('/api/ParametroPagamento', {
        signal: controller.signal,
      }),
    ])
      .then(([item, parametro]) => {
        setCobranca(item)
        setMultaPercentual(parametro.multaPercentual)
        setJurosDiario(parametro.jurosDiarioPercentual)
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar a cobrança.')
        setLoading(false)
      })

    return () => controller.abort()
  }, [cobrancaId])

  async function submit(payload: { dataPagamento: string; juros: number }) {
    if (!cobrancaId) return
    setSaving(true)
    setError(null)
    try {
      await pagarCobranca(cobrancaId, payload)
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
        <BaixaCobrancaForm
          key={cobranca.navigationId}
          cobranca={cobranca}
          multaPercentual={multaPercentual}
          jurosDiario={jurosDiario}
          saving={saving}
          onCancel={back}
          onError={setError}
          onSubmit={submit}
        />
      )}
    </Stack>
  )
}
