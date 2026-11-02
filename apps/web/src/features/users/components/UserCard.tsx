import { Paper, Text } from '@mantine/core'
import type { User } from '../userCrud.ts'

export function UserCard({ user }: { user: User }) {
  return (
    <Paper component="li" p="md" radius="md" bg="dark.6" withBorder>
      <Text fw={600}>{user.nome}</Text>
      <Text size="sm" c="dimmed">
        {user.email}
      </Text>
      <Text size="sm" c={user.active ? 'orange' : 'dimmed'}>
        {user.active ? 'Ativo' : 'Inativo'}
      </Text>
    </Paper>
  )
}
