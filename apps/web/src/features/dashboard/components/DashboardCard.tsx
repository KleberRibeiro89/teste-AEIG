import { Paper, Text } from '@mantine/core'

interface DashboardCardProps {
  label: string
  value: string
  detail: string
  color: 'orange' | 'red' | 'green' | 'gray'
  onClick?: () => void
}

export function DashboardCard({ label, value, detail, color, onClick }: DashboardCardProps) {
  const tone = color === 'gray' ? 'dark' : color

  return (
    <Paper
      component={onClick ? 'button' : 'div'}
      type={onClick ? 'button' : undefined}
      onClick={onClick}
      p="lg"
      radius="md"
      withBorder
      style={{
        background: color === 'gray' ? undefined : `var(--mantine-color-${tone}-light)`,
        borderColor: `var(--mantine-color-${tone}-6)`,
        textAlign: 'left',
        cursor: onClick ? 'pointer' : undefined,
        width: '100%',
        minHeight: 128,
      }}
      bg={color === 'gray' ? 'dark.6' : undefined}
    >
      <Text size="md" c="dimmed">
        {label}
      </Text>
      <Text fw={700} fz={30} lh={1.2} c={color === 'gray' ? undefined : color}>
        {value}
      </Text>
      <Text size="sm" c="dimmed">
        {detail}
      </Text>
    </Paper>
  )
}
