import { SimpleGrid, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router'
import { money } from '../../lib/format.ts'
import { DashboardCard } from './components/DashboardCard.tsx'
import { getDashboard, type DashboardStatus } from './dashboard.ts'

function byName(cobrancas: DashboardStatus[], nome: string) {
  return cobrancas.find((item) => item.nome.localeCompare(nome, 'pt-BR', { sensitivity: 'accent' }) === 0)
}

function chargeLabel(quantidade: number) {
  return `${quantidade} cobrança${quantidade === 1 ? '' : 's'}`
}

export function DashboardPage() {
  const navigate = useNavigate()
  const [associados, setAssociados] = useState(0)
  const [cobrancas, setCobrancas] = useState<DashboardStatus[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError(null)
    getDashboard(controller.signal)
      .then((dashboard) => {
        setAssociados(dashboard.associados)
        setCobrancas(dashboard.cobrancas)
        setLoading(false)
      })
      .catch((caught: unknown) => {
        if (caught instanceof DOMException && caught.name === 'AbortError') return
        setError(caught instanceof Error ? caught.message : 'Não foi possível carregar o dashboard.')
        setLoading(false)
      })
    return () => controller.abort()
  }, [])

  const pendente = byName(cobrancas, 'Pendente')
  const emAtraso = byName(cobrancas, 'Em Atraso')
  const quantidadeAberto = (pendente?.quantidade ?? 0) + (emAtraso?.quantidade ?? 0)
  const valorAberto = (pendente?.valor ?? 0) + (emAtraso?.valor ?? 0)
  const valorAtraso = emAtraso?.valor ?? 0
  const recebido = cobrancas
    .filter((item) => item.nome.toLocaleLowerCase('pt-BR').startsWith('pago'))
    .reduce(
      (sum, item) => ({ quantidade: sum.quantidade + item.quantidade, valor: sum.valor + item.valorPago }),
      { quantidade: 0, valor: 0 },
    )

  return (
    <Stack gap="md" p="xl" maw={720}>
      <div>
        <Title order={2}>Dashboard</Title>
        <Text c="dimmed" size="sm">
          {loading ? 'Carregando...' : 'Resumo dos associados e das cobranças'}
        </Text>
      </div>

      {error && (
        <Text c="red" size="sm">
          {error}
        </Text>
      )}

      <SimpleGrid cols={{ base: 2, sm: 4 }} spacing="xs">
        <DashboardCard
          label="Associados"
          value={String(associados)}
          detail="Cadastros ativos"
          color="gray"
          onClick={() => navigate('/associados')}
        />
        <DashboardCard
          label="Valor em aberto"
          value={money.format(valorAberto)}
          detail={chargeLabel(quantidadeAberto)}
          color="orange"
        />
        <DashboardCard
          label="Valor recebido"
          value={money.format(recebido.valor)}
          detail={chargeLabel(recebido.quantidade)}
          color="green"
        />
        <DashboardCard
          label="Valores em atraso"
          value={money.format(valorAtraso)}
          detail={chargeLabel(emAtraso?.quantidade ?? 0)}
          color="red"
        />
      </SimpleGrid>
    </Stack>
  )
}
