import { Text } from '@mantine/core'
import { formatMonth, money } from '../../../lib/format.ts'
import type { DashboardMes } from '../dashboard.ts'

function shortMonth(value: string) {
  const [year, month] = value.split('-')
  const date = new Date(Number(year), Number(month) - 1, 1)
  if (Number.isNaN(date.getTime())) return value
  const label = date.toLocaleDateString('pt-BR', { month: 'short' }).replace('.', '')
  return `${label}/${year.slice(2)}`
}

interface DashboardBarChartProps {
  items: DashboardMes[]
  color: 'green' | 'red'
  emptyLabel: string
}

export function DashboardBarChart({ items, color, emptyLabel }: DashboardBarChartProps) {
  if (items.length === 0) {
    return (
      <Text c="dimmed" size="sm">
        {emptyLabel}
      </Text>
    )
  }

  const max = Math.max(...items.map((item) => item.valor), 1)
  const slot = 72
  const width = Math.max(items.length * slot, 320)
  const height = 220
  const padTop = 12
  const padBottom = 36
  const inner = height - padTop - padBottom

  return (
    <div style={{ overflowX: 'auto' }}>
      <svg width={width} height={height} role="img" aria-label="Gráfico de barras">
        {items.map((item, index) => {
          const barHeight = (item.valor / max) * inner
          const x = index * slot + 16
          const y = padTop + inner - barHeight
          return (
            <g key={item.mes}>
              <title>{`${formatMonth(item.mes)}: ${money.format(item.valor)}`}</title>
              <rect
                x={x}
                y={y}
                width={40}
                height={item.valor > 0 ? Math.max(barHeight, 2) : 0}
                rx={4}
                fill={`var(--mantine-color-${color}-6)`}
              />
              <text x={x + 20} y={height - 12} textAnchor="middle" fill="var(--mantine-color-dark-1)" fontSize="12">
                {shortMonth(item.mes)}
              </text>
            </g>
          )
        })}
      </svg>
    </div>
  )
}
