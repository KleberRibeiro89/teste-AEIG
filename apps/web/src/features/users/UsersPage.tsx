import { Button, Group, Stack, Text, Title } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { useNavigate } from 'react-router'
import { ListPagination } from '../../components/ListPagination.tsx'
import { useUsers } from './userCrud.ts'
import { UserCard } from './components/UserCard.tsx'
import { UserFilters } from './components/UserFilters.tsx'

export function UsersPage() {
  const users = useUsers()
  const navigate = useNavigate()
  const [filtersOpened, { toggle: toggleFilters }] = useDisclosure(false)
  const items = users.data.result
  const pageSize = users.data.pageSize || users.query.pageSize || 20
  const pageNumber = users.query.pageNumber ?? 0
  const totalPages = Math.max(1, Math.ceil(users.data.totalRows / pageSize))

  return (
    <Stack gap="md" p="xl" maw={720}>
      <Group justify="space-between" align="flex-end">
        <div>
          <Title order={2}>Usuários</Title>
          <Text c="dimmed" size="sm">
            {users.loading
              ? 'Carregando...'
              : `${users.data.totalRows} usuário${users.data.totalRows === 1 ? '' : 's'}`}
          </Text>
        </div>
        <Group gap="sm">
          <Button variant="default" onClick={toggleFilters} aria-expanded={filtersOpened}>
            Filtros
          </Button>
          <Button onClick={() => navigate('/usuarios/novo')}>Novo</Button>
        </Group>
      </Group>

      <UserFilters opened={filtersOpened} onApply={users.setFilters} onClear={() => users.setFilters([])} />

      {users.error && (
        <Text c="red" size="sm">
          {users.error}
        </Text>
      )}

      {!users.loading && !users.error && items.length === 0 && (
        <Text c="dimmed">Nenhum usuário cadastrado.</Text>
      )}

      <Stack component="ul" gap="sm" p={0} style={{ listStyle: 'none' }}>
        {items.map((user) => (
          <UserCard key={user.navigationId} user={user} />
        ))}
      </Stack>

      <ListPagination pageNumber={pageNumber} totalPages={totalPages} onPage={users.setPage} />
    </Stack>
  )
}
