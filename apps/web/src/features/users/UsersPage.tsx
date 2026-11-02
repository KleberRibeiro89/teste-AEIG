import { Button, Collapse, Group, Paper, Select, Stack, Text, TextInput, Title } from '@mantine/core'
import { useDisclosure } from '@mantine/hooks'
import { useState, type FormEvent } from 'react'
import { useNavigate } from 'react-router'
import { contains, eq } from '../../lib/query.ts'
import type { PropertyFilter } from '../../types/crud.ts'
import { useUsers, type User } from './userCrud.ts'

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
          <Paper key={user.navigationId} component="li" p="md" radius="md" bg="dark.6" withBorder>
            <Text fw={600}>{user.nome}</Text>
            <Text size="sm" c="dimmed">
              {user.email}
            </Text>
            <Text size="sm" c={user.active ? 'orange' : 'dimmed'}>
              {user.active ? 'Ativo' : 'Inativo'}
            </Text>
          </Paper>
        ))}
      </Stack>

      {totalPages > 1 && (
        <Group gap="sm">
          <Button variant="default" disabled={pageNumber <= 0} onClick={() => users.setPage(pageNumber - 1)}>
            Anterior
          </Button>
          <Text size="sm">
            {pageNumber + 1} de {totalPages}
          </Text>
          <Button
            variant="default"
            disabled={pageNumber + 1 >= totalPages}
            onClick={() => users.setPage(pageNumber + 1)}
          >
            Próxima
          </Button>
        </Group>
      )}
    </Stack>
  )
}

function UserFilters({
  opened,
  onApply,
  onClear,
}: {
  opened: boolean
  onApply: (filters: PropertyFilter<User>[]) => void
  onClear: () => void
}) {
  const [nome, setNome] = useState('')
  const [email, setEmail] = useState('')
  const [active, setActive] = useState<string | null>(null)

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const filters: PropertyFilter<User>[] = []
    const trimmedNome = nome.trim()
    const trimmedEmail = email.trim()

    if (trimmedNome) filters.push(contains<User>('nome', trimmedNome))
    if (trimmedEmail) filters.push(contains<User>('email', trimmedEmail))
    if (active === 'true' || active === 'false') filters.push(eq<User>('active', active === 'true'))
    onApply(filters)
  }

  function clear() {
    setNome('')
    setEmail('')
    setActive(null)
    onClear()
  }

  return (
    <Collapse expanded={opened}>
      <Paper component="form" onSubmit={submit} p="md" radius="md" bg="dark.6" withBorder>
        <Stack gap="sm">
          <TextInput label="Nome" value={nome} onChange={(event) => setNome(event.currentTarget.value)} />
          <TextInput label="E-mail" value={email} onChange={(event) => setEmail(event.currentTarget.value)} />
          <Select
            label="Situação"
            placeholder="Todas"
            clearable
            value={active}
            onChange={setActive}
            data={[
              { value: 'true', label: 'Ativo' },
              { value: 'false', label: 'Inativo' },
            ]}
          />
          <Group justify="flex-end">
            <Button type="button" variant="default" onClick={clear}>
              Limpar
            </Button>
            <Button type="submit">Filtrar</Button>
          </Group>
        </Stack>
      </Paper>
    </Collapse>
  )
}
