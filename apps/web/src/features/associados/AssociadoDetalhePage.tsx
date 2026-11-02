import { Button, Group, Paper, Stack, Text, Title } from '@mantine/core'
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router'
import { asc, eq } from '../../lib/query.ts'
import { associadoApi, type Associado } from './associadoCrud.ts'
import { cobrancaApi, type Cobranca } from './cobranca.ts'

const money = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })

function formatCpf(cpf: string) {
  const digits = cpf.replace(/\D/g, '')
  if (digits.length !== 11) return cpf
  return digits.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4')
}

function formatDate(value?: string | null) {
  if (!value) return '—'
  const [year, month, day] = value.slice(0, 10).split('-')
  if (!year || !month || !day) return value
  return `${day}/${month}/${year}`
}

function chargeTone(status?: string | null) {
  const name = (status ?? 'Pendente').toLocaleLowerCase('pt-BR')
  if (name.startsWith('pago')) return 'success'
  if (name.includes('atraso')) return 'error'
  return 'warning'
}

function StatusIcon({ kind }: { kind: 'warning' | 'error' | 'success' }) {
  const common = {
    width: 16,
    height: 16,
    viewBox: '0 0 24 24',
    fill: 'none',
    stroke: 'currentColor',
    strokeWidth: 2,
    strokeLinecap: 'round',
    strokeLinejoin: 'round',
    'aria-hidden': true,
  } as const
  if (kind === 'success') {
    return (
      <svg {...common}>
        <circle cx="12" cy="12" r="9" />
        <path d="M8 12.5 11 15.5 16 9" />
      </svg>
    )
  }
  if (kind === 'error') {
    return (
      <svg {...common}>
        <circle cx="12" cy="12" r="9" />
        <path d="M15 9 9 15M9 9l6 6" />
      </svg>
    )
  }
  return (
    <svg {...common}>
      <path d="M12 3.5 21.5 20.5H2.5Z" />
      <path d="M12 9v5" />
      <path d="M12 17.5h.01" />
    </svg>
  )
}

function statusColor(kind: 'warning' | 'error' | 'success') {
  if (kind === 'success') return 'green'
  if (kind === 'error') return 'red'
  return 'orange'
}

function formatMonth(value: string) {
  const [year, month] = value.split('-')
  const date = new Date(Number(year), Number(month) - 1, 1)
  if (Number.isNaN(date.getTime())) return value
  const label = date.toLocaleDateString('pt-BR', { month: 'long', year: 'numeric' })
  return label.charAt(0).toUpperCase() + label.slice(1)
}

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

      {associado && (
        <Paper p="md" radius="md" bg="dark.6" withBorder>
          <Text fw={600}>{associado.nome}</Text>
          <Text size="sm" c="dimmed">
            CPF {formatCpf(associado.cpf)}
          </Text>
          <Text size="sm" c="orange">
            Mensalidade {money.format(associado.valorMensalidade)}
          </Text>
          <Text size="sm" c="dimmed">
            Vencimento dia {associado.diaVencimento}
          </Text>
        </Paper>
      )}

      {!loading && !error && cobrancas.length === 0 && (
        <Text c="dimmed">Nenhuma cobrança para este associado.</Text>
      )}

      <Stack gap="sm">
        {cobrancas.map((cobranca) => {
          const tone = chargeTone(cobranca.nomeStatus)
          const color = statusColor(tone)
          return (
          <Paper
            key={cobranca.navigationId}
            p="md"
            radius="md"
            withBorder
            style={{
              background: `var(--mantine-color-${color}-light)`,
              borderColor: `var(--mantine-color-${color}-6)`,
            }}
          >
            <Group justify="space-between" align="flex-start">
              <Text fw={600}>{formatMonth(cobranca.mesReferencia)}</Text>
              <Group gap={6} c={color} wrap="nowrap">
                <StatusIcon kind={tone} />
                <Text size="sm">{cobranca.nomeStatus ?? 'Pendente'}</Text>
              </Group>
            </Group>
            <Text size="sm" c="dimmed">
              Vencimento {formatDate(cobranca.dataVencimento)}
            </Text>
            <Text size="sm">Valor {money.format(cobranca.valor)}</Text>
            <Text size="sm" c="dimmed">
              Pago {money.format(cobranca.valorPago)}
              {cobranca.dataPagamento ? ` em ${formatDate(cobranca.dataPagamento)}` : ''}
            </Text>
            <Text size="xs" c="dimmed">
              Nosso número {cobranca.nossoNumero}
            </Text>
          </Paper>
          )
        })}
      </Stack>
    </Stack>
  )
}
