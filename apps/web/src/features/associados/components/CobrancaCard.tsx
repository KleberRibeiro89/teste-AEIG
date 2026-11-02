import { ActionIcon, Group, Paper, Text, Tooltip } from '@mantine/core'
import { BaixaIcon, StatusIcon } from '../../../components/icons.tsx'
import { formatDate, formatMonth, money } from '../../../lib/format.ts'
import type { Cobranca } from '../cobranca.ts'

export type ChargeTone = 'warning' | 'error' | 'success'

export function chargeTone(status?: string | null): ChargeTone {
  const name = (status ?? 'Pendente').toLocaleLowerCase('pt-BR')
  if (name.startsWith('pago')) return 'success'
  if (name.includes('atraso')) return 'error'
  return 'warning'
}

function statusColor(kind: ChargeTone) {
  if (kind === 'success') return 'green'
  if (kind === 'error') return 'red'
  return 'orange'
}

interface CobrancaCardProps {
  cobranca: Cobranca
  onBaixa: () => void
}

export function CobrancaCard({ cobranca, onBaixa }: CobrancaCardProps) {
  const tone = chargeTone(cobranca.nomeStatus)
  const color = statusColor(tone)

  return (
    <Paper
      p="md"
      radius="md"
      withBorder
      style={{
        background: `var(--mantine-color-${color}-light)`,
        borderColor: `var(--mantine-color-${color}-6)`,
      }}
    >
      <Group justify="space-between" align="flex-start" wrap="nowrap">
        <Text fw={600}>{formatMonth(cobranca.mesReferencia)}</Text>
        <Group gap="xs" wrap="nowrap">
          {tone !== 'success' && (
            <Tooltip label="Baixa manual">
              <ActionIcon variant="light" color={color} aria-label="Baixa manual" onClick={onBaixa}>
                <BaixaIcon />
              </ActionIcon>
            </Tooltip>
          )}
          <Group gap={6} c={color} wrap="nowrap">
            <StatusIcon kind={tone} />
            <Text size="sm">{cobranca.nomeStatus ?? 'Pendente'}</Text>
          </Group>
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
}
