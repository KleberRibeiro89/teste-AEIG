import { Button, Group, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router'
import { asc, eq } from '../../lib/query.ts'
import { associadoApi, type Associado } from './associadoCrud.ts'
import { cobrancaApi, type Cobranca } from './cobranca.ts'
import { AssociadoResumo } from './components/AssociadoResumo.tsx'
import { CobrancaCard } from './components/CobrancaCard.tsx'

export function AssociadoDetalhePage() {
  const { navigationId } = useParams()
  const navigate = useNavigate()
  const [associado, setAssociado] = useState<Associado | null>(null)
  const [cobrancas, setCobrancas] = useState<Cobranca[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    if (!navigationId) return
    const controller = new AbortController()
    setLoading(true)
    setError(null)

    associadoApi
      .getById(navigationId, controller.signal)
      .then(async (item) => {
        setAssociado(item)
        const page = await cobrancaApi.getPaged(
          {
            pageNumber: 0,
            pageSize: 50,
            filter: [eq<Cobranca>('idAssociado', item.id)],
            orderBy: [asc<Cobranca>('mesReferencia')],
          },
          controller.signal,
        )
        setCobrancas(page.result)
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar o associado.')
        setLoading(false)
      })

    return () => controller.abort()
  }, [navigationId])

  return (
    <Stack gap="md" p="xl" maw={720}>
      <Group justify="space-between" align="flex-end">
        <div>
          <Title order={2}>{associado?.nome ?? 'Associado'}</Title>
          <Text c="dimmed" size="sm">
            {loading ? 'Carregando...' : `${cobrancas.length} cobrança${cobrancas.length === 1 ? '' : 's'}`}
          </Text>
        </div>
        <Button variant="default" onClick={() => navigate('/associados')}>
          Voltar
        </Button>
      </Group>

      {error && (
        <Text c="red" size="sm">
          {error}
        </Text>
      )}

      {associado && <AssociadoResumo associado={associado} />}

      {!loading && !error && cobrancas.length === 0 && (
        <Text c="dimmed">Nenhuma cobrança para este associado.</Text>
      )}

      <Stack gap="sm">
        {cobrancas.map((cobranca) => (
          <CobrancaCard
            key={cobranca.navigationId}
            cobranca={cobranca}
            onBaixa={() => navigate(`/associados/${navigationId}/cobrancas/${cobranca.navigationId}/baixa`)}
          />
        ))}
      </Stack>
    </Stack>
  )
}
