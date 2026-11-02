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
      p="xs"
      radius="sm"
      withBorder
      style={{
        background: color === 'gray' ? undefined : `var(--mantine-color-${tone}-light)`,
        borderColor: `var(--mantine-color-${tone}-6)`,
        textAlign: 'left',
        cursor: onClick ? 'pointer' : undefined,
        width: '100%',
      }}
      bg={color === 'gray' ? 'dark.6' : undefined}
    >
      <Text size="xs" c="dimmed">
        {label}
      </Text>
      <Text fw={700} size="md" c={color === 'gray' ? undefined : color}>
        {value}
      </Text>
      <Text size="xs" c="dimmed">
        {detail}
      </Text>
    </Paper>
  )
}
